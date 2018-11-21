using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LBCService
{
    public static class NativeMethods
    {
        private delegate void HandlerEx(int control, int eventType, IntPtr eventData, IntPtr context);

        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification",
                 CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern UInt32 CallNtPowerInformation(Int32 InformationLevel, IntPtr lpInputBuffer, UInt32 nInputBufferSize, out SYSTEM_POWER_CAPABILITIES lpOutputBuffer, UInt32 nOutputBufferSize);

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

        private static ServiceBase registeredService;

        // This structure is sent when the PBT_POWERSETTINGSCHANGE message is sent.
        // It describes the power setting that has changed and contains data about the change
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
            public byte ProcessorMaxThrottle;    // Also known as ProcessorThrottleScale before Windows XP
            [MarshalAs(UnmanagedType.U1)]
            public bool FastSystemS4;   // Ignore if earlier than Windows XP
            [MarshalAs(UnmanagedType.U1)]
            public bool Hiberboot;  // Ignore if earlier than Windows XP
            [MarshalAs(UnmanagedType.U1)]
            public bool WakeAlarmPresent;   // Ignore if earlier than Windows XP
            [MarshalAs(UnmanagedType.U1)]
            public bool AoAc;   // Ignore if earlier than Windows XP
            [MarshalAs(UnmanagedType.U1)]
            public bool DiskSpinDown;
            public byte HiberFileType;  // Ignore if earlier than Windows 10 (10.0.10240.0)
            [MarshalAs(UnmanagedType.U1)]
            public bool AoAcConnectivitySupported;  // Ignore if earlier than Windows 10 (10.0.10240.0)
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

        internal static void RegisterServiceForPowerNotifications(string serviceName)
        {
            Logging.LogMessage($"Registering Service for Power Notifications: {serviceName}");
            var serviceStatusHandle = RegisterServiceCtrlHandlerEx(serviceName, DoHandlerCallback, IntPtr.Zero);

            hMonitorOn = RegisterPowerSettingNotification(serviceStatusHandle, ref GUID_MONITOR_POWER_ON, DEVICE_NOTIFY_SERVICE_HANDLE);

            //GetAlwaysOnAlwaysConnectedCapability();
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

        //public static bool SupportsConnectedStandby { get; private set; }

        public static void UnregisterFromPowerNotifications()
        {
            EventLog.WriteEntry("LenovoBacklightControl", "Unregistering from Power Notifications.", EventLogEntryType.Information, 50906);
            var retVal = UnregisterPowerSettingNotification(hMonitorOn);
        }

        private static void DoHandlerCallback(int control, int eventType, IntPtr eventData, IntPtr context)
        {
            EventLog.WriteEntry("LenovoBacklightControl", $"DoHandlerCallback(control:{control}, eventType:{eventType}, eventData:{eventData}, context: {context})", EventLogEntryType.Information, 50906);

            if (control != SERVICE_CONTROL_POWEREVENT)
            {
                if (control != SERVICE_CONTROL_STOP) return;

                EventLog.WriteEntry("LenovoBacklightControl", "SERVICE_CONTROL_STOP received.", EventLogEntryType.Information, 50906);
                UnregisterFromPowerNotifications();
                registeredService.Stop();
            }
            else
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
                    EventLog.WriteEntry("LenovoBacklightControl", "ERROR: powersetting == null", EventLogEntryType.Information, 50906);
                }
            }
        }

        //private static void GetAlwaysOnAlwaysConnectedCapability()
        //{
        //    SYSTEM_POWER_CAPABILITIES cap;
        //    var retVal = CallNtPowerInformation(SYSTEM_POWER_CAPABILITIES_LEVEL, IntPtr.Zero, 0, out cap, (uint)Marshal.SizeOf(typeof(SYSTEM_POWER_CAPABILITIES)));

        //    if (retVal == 0)
        //    {
        //        // Get the connected standby support value
        //        if (cap.AoAc)
        //        {
        //            Logging.LogMessage("This system supports Connected Standby.");
        //            SupportsConnectedStandby = true;
        //        }
        //        else
        //        {
        //            Logging.LogMessage("WARNING: This system does not support Connected Standby.");
        //        }
        //    }
        //}

        public static IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != WM_POWERBROADCAST || wParam.ToInt32() != PBT_POWERSETTINGCHANGE) return IntPtr.Zero;

            // Extract data from message
            var powersetting =
                (POWERBROADCAST_SETTING)Marshal.PtrToStructure(
                    lParam, typeof(POWERBROADCAST_SETTING));
            var pData = (IntPtr)(lParam.ToInt32() + Marshal.SizeOf(powersetting));  // (*1)
            UpdateStandbyState(powersetting);

            return IntPtr.Zero;
        }

        private static void UpdateStandbyState(POWERBROADCAST_SETTING powersetting)
        {
            // When the display is on (1), you are exiting standby
            if (powersetting.Data == 0) return;
            EventLog.WriteEntry("LenovoBacklightControl", "Detected system resume.  Activating backlight.", EventLogEntryType.Information, 50905);
            LenovoBacklightControl.BLC.ActivateBacklight();
            ConnectedStandby = false;
        }
    }
}
