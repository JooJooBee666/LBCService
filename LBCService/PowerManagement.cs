using System;
using System.Runtime.InteropServices;
using LBCService.Common;
using LBCService.Messages;
using TinyMessenger;

namespace LBCService
{
    public class PowerManagement : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;
        private IntPtr? _hMonitorOn;
        private TinyMessageSubscriptionToken _subToStart;
        private TinyMessageSubscriptionToken _subToStop;

        #region Win32

        private delegate void HandlerEx(int control, int eventType, IntPtr eventData, IntPtr context);

        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification",
                 CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern IntPtr RegisterServiceCtrlHandlerEx(string lpServiceName, HandlerEx cbex, IntPtr context);

        // 02731015-4510-4526-99E6-E5A17EBD1AEA
        private static Guid GUID_MONITOR_POWER_ON = new Guid(0x02731015, 0x4510, 0x4526, 0x99, 0xE6, 0xE5, 0xA1, 0x7E, 0xBD, 0x1A, 0xEA);
        private const int WM_POWERBROADCAST = 0x0218;
        private const int SYSTEM_POWER_CAPABILITIES_LEVEL = 0x0004;
        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        private const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001;
        private const int PBT_POWERSETTINGCHANGE = 0x8013; // DPPE
        private const int SERVICE_CONTROL_POWEREVENT = 0x0000000D;
        private const int SERVICE_CONTROL_STOP = 0x00000001;

        //
        // Structure is used the PBT_POWERSETTINGSCHANGE message is sent.
        //
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        internal struct SYSTEM_POWER_CAPABILITIES
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool PowerButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SleepButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool LidPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS1;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS2;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS4;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS5;
            [MarshalAs(UnmanagedType.U1)]
            public bool HiberFilePresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool FullWake;
            [MarshalAs(UnmanagedType.U1)]
            public bool VideoDimPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ApmPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool UpsPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ThermalControl;
            [MarshalAs(UnmanagedType.U1)]
            public bool ProcessorThrottle;
            public byte ProcessorMinThrottle;
            public byte ProcessorMaxThrottle;
            [MarshalAs(UnmanagedType.U1)]
            public bool FastSystemS4;
            [MarshalAs(UnmanagedType.U1)]
            public bool Hiberboot;
            [MarshalAs(UnmanagedType.U1)]
            public bool WakeAlarmPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool AoAc;
            [MarshalAs(UnmanagedType.U1)]
            public bool DiskSpinDown;
            public byte HiberFileType;
            [MarshalAs(UnmanagedType.U1)]
            public bool AoAcConnectivitySupported;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            private readonly byte[] spare3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemBatteriesPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool BatteriesAreShortTerm;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public BATTERY_REPORTING_SCALE[] BatteryScale;
            public SYSTEM_POWER_STATE AcOnLineWake;
            public SYSTEM_POWER_STATE SoftLidWake;
            public SYSTEM_POWER_STATE RtcWake;
            public SYSTEM_POWER_STATE MinDeviceWakeState;
            public SYSTEM_POWER_STATE DefaultLowLatencyWake;
        }

        internal struct BATTERY_REPORTING_SCALE
        {
            public ulong Granularity;
            public ulong Capacity;
        }

        internal enum SYSTEM_POWER_STATE
        {
            PowerSystemUnspecified = 0,
            PowerSystemWorking = 1,
            PowerSystemSleeping1 = 2,
            PowerSystemSleeping2 = 3,
            PowerSystemSleeping3 = 4,
            PowerSystemHibernate = 5,
            PowerSystemShutdown = 6,
            PowerSystemMaximum = 7
        }

        #endregion

        //public bool ConnectedStandby { get; private set; }

        public PowerManagement(ILogger logger, ITinyMessengerHub hub)
        {
            _logger = logger;
            _hub = hub;
            _subToStart = _hub.Subscribe<OnStartupMessage>(_ => RegisterServiceForPowerNotifications(_.ServiceName));
            _subToStop = _hub.Subscribe<OnStopMessage>(_ => UnregisterFromPowerNotifications());
        }
        
        /// <summary>
        ///    Resgister's our service to receive events when the power state changes
        /// </summary>
        private void RegisterServiceForPowerNotifications(string serviceName)
        {
            var message = $"Registering Service for Power Notifications:{serviceName}.";
            _logger.Info(message, 50906);

            var serviceStatusHandle = RegisterServiceCtrlHandlerEx(serviceName, HandlerCallback, IntPtr.Zero);

            _hMonitorOn = RegisterPowerSettingNotification(serviceStatusHandle, ref GUID_MONITOR_POWER_ON, DEVICE_NOTIFY_SERVICE_HANDLE);
        }

        /// <summary>
        ///    Unregisters our service from power events
        /// </summary>
        private void UnregisterFromPowerNotifications()
        {
            _logger.Debug("Unregistering from Power Notifications.");
            if (_hMonitorOn.HasValue) UnregisterPowerSettingNotification(_hMonitorOn.Value);
        }

        /// <summary>
        ///    Callback method used when power state changes and event is fired
        /// </summary>
        /// <param name="control"></param>
        /// <param name="eventType"></param>
        /// <param name="eventData"></param>
        /// <param name="context"></param>
        private void HandlerCallback(int control, int eventType, IntPtr eventData, IntPtr context)
        {
            _logger.Debug($"HandlerCallback(control:{control}, eventType:{eventType}, eventData:{eventData}, context: {context})");

            if (control == SERVICE_CONTROL_POWEREVENT)
            {
                if (eventType != PBT_POWERSETTINGCHANGE) return;
                var powersetting = Marshal.PtrToStructure(eventData, typeof(POWERBROADCAST_SETTING));

                if (powersetting != null)
                {
                    //Valid power event data, process it
                    UpdateStandbyState((POWERBROADCAST_SETTING) powersetting);
                }
                else
                {
                    _logger.Error("ERROR: powersetting == null", 50906);
                }
            }
            else if (control == SERVICE_CONTROL_STOP)
            {
                _logger.Info("SERVICE_CONTROL_STOP received.", 50906);
                UnregisterFromPowerNotifications();
                _hub.PublishAsync(new StopServiceRequestMessage(this));
            }
        }

        /*public IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != WM_POWERBROADCAST || wParam.ToInt32() != PBT_POWERSETTINGCHANGE) return IntPtr.Zero;

            // Extract data from message
            var powersetting = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(lParam, typeof(POWERBROADCAST_SETTING));
            var presultData = (IntPtr)(lParam.ToInt32() + Marshal.SizeOf(powersetting));
            UpdateStandbyState(powersetting);

            return IntPtr.Zero;
        }*/

        /// <summary>
        ///   Detect when system resumes from standy and enable the backlight
        /// </summary>
        /// <param name="powersetting"></param>
        private void UpdateStandbyState(POWERBROADCAST_SETTING powersetting)
        {
            _hub.PublishAsync(new PowerStateMessage(this, powersetting.Data != 0));
        }

        public void Dispose()
        {
            _subToStart?.Dispose();
            _subToStart = null;

            _subToStop?.Dispose();
            _subToStop = null;
        }
    }
}
