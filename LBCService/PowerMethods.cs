using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace LBCService
{
    public static class PowerMethods
    {
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
        private static ServiceBase _registeredServiceBase;

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
        //private static ServiceBase registeredService;

        internal static void RegisterServiceForPowerNotifications(ServiceBase serviceBase)
        {
            _registeredServiceBase = serviceBase;
            EventLog.WriteEntry("LenovoBacklightControl", "Registering Service for Power Notifications:" + serviceBase.ServiceName + ".", EventLogEntryType.Information, 50906);
            var serviceStatusHandle = RegisterServiceCtrlHandlerEx(serviceBase.ServiceName, HandlerCallback, IntPtr.Zero);

            hMonitorOn = RegisterPowerSettingNotification(serviceStatusHandle, ref GUID_MONITOR_POWER_ON, DEVICE_NOTIFY_SERVICE_HANDLE);
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

        private static IntPtr hMonitorOn;

        public static bool ConnectedStandby { get; private set; }

        public static void UnregisterFromPowerNotifications()
        {
#if DEBUG
            EventLog.WriteEntry("LenovoBacklightControl", "Unregistering from Power Notifications.", EventLogEntryType.Information, 50906);
#endif
            var retVal = UnregisterPowerSettingNotification(hMonitorOn);
        }

        private static void HandlerCallback(int control, int eventType, IntPtr eventData, IntPtr context)
        {
#if def
            EventLog.WriteEntry("LenovoBacklightControl", $"HandlerCallback(control:{control}, eventType:{eventType}, eventData:{eventData}, context: {context})", EventLogEntryType.Information, 50906);
#endif
            if (control == SERVICE_CONTROL_POWEREVENT)
            {
                if (eventData == null) return;

                if (eventType != PBT_POWERSETTINGCHANGE) return;
                var powersetting = Marshal.PtrToStructure(eventData, typeof(POWERBROADCAST_SETTING));

                if (powersetting != null)
                {
                    //Valid power event data, process it
                    UpdateStandbyState((POWERBROADCAST_SETTING) powersetting);
                }
                else
                {
#if DEBUG
                    EventLog.WriteEntry("LenovoBacklightControl", "ERROR: powersetting == null",EventLogEntryType.Information, 50906);
#endif
                }
            }
            else if (control == SERVICE_CONTROL_STOP)
            {
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl", "SERVICE_CONTROL_STOP received.", EventLogEntryType.Information, 50906);
#endif
                UnregisterFromPowerNotifications();
                _registeredServiceBase.Stop();
            }
        }

        public static IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != WM_POWERBROADCAST || wParam.ToInt32() != PBT_POWERSETTINGCHANGE) return IntPtr.Zero;

            // Extract data from message
            var powersetting = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(lParam, typeof(POWERBROADCAST_SETTING));
            var presultData = (IntPtr)(lParam.ToInt32() + Marshal.SizeOf(powersetting));
            UpdateStandbyState(powersetting);

            return IntPtr.Zero;
        }

        private static void UpdateStandbyState(POWERBROADCAST_SETTING powersetting)
        {
            // When the display is on (1), you are exiting standby
            if (powersetting.Data == 0) return;
#if DEBUG
            EventLog.WriteEntry("LenovoBacklightControl", "Detected system resume.  Activating backlight.", EventLogEntryType.Information, 50905);
#endif
            LenovoBacklightControl.BLC.ActivateBacklight();
            ConnectedStandby = false;
        }
    }
}
