using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace LBCService
{
    public static class PowerMethods
    {
        private delegate void HandlerEx(int control, int eventType, IntPtr eventData, IntPtr context);

        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification",
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid,
            Int32 Flags);

        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification",
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern IntPtr RegisterServiceCtrlHandlerEx(string lpServiceName, HandlerEx cbex, IntPtr context);

        // 02731015-4510-4526-99E6-E5A17EBD1AEA
        private static Guid GUID_MONITOR_POWER_ON =
            new Guid(0x02731015, 0x4510, 0x4526, 0x99, 0xE6, 0xE5, 0xA1, 0x7E, 0xBD, 0x1A, 0xEA);

        private const int WM_POWERBROADCAST = 0x0218;
        private const int SYSTEM_POWER_CAPABILITIES_LEVEL = 0x0004;
        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        private const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001;
        private const int PBT_POWERSETTINGCHANGE = 0x8013; // DPPE
        private const int SERVICE_CONTROL_POWEREVENT = 0x0000000D;
        private const int SERVICE_CONTROL_STOP = 0x00000001;

        private static IntPtr hMonitorOn;

        //
        // Structure is used the PBT_POWERSETTINGSCHANGE message is sent.
        //
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal class POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        /// <summary>
        ///    Resgister's our service to receive events when the power state changes
        /// </summary>
        /// <param name="serviceBase"></param>
        [HandleProcessCorruptedStateExceptions]
        internal static void RegisterServiceForPowerNotifications(ServiceBase serviceBase)
        {
            var message = $"Registering Service for Power Notifications:{serviceBase.ServiceName}.";
            EventLog.WriteEntry("LenovoBacklightControl", message, EventLogEntryType.Information, 50906);
            LenovoBacklightControl.WriteToDebugLog(message);
            try
            {
                var serviceStatusHandle =
                    RegisterServiceCtrlHandlerEx(serviceBase.ServiceName, HandlerCallback, IntPtr.Zero);
                hMonitorOn = RegisterPowerSettingNotification(serviceStatusHandle, ref GUID_MONITOR_POWER_ON,
                    DEVICE_NOTIFY_SERVICE_HANDLE);
            }
            catch (AccessViolationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl",
                    "Failed to register for Power Notificaitons. Error: " + e.Message, EventLogEntryType.Information,
                    50906);
#endif
                LenovoBacklightControl.WriteToDebugLog(
                    "Failed to register for Power Notificaitons. Error: " + e.Message);
            }

        }

        public static bool ConnectedStandby { get; private set; }

        /// <summary>
        ///    Unregisters our service from power events
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public static void UnregisterFromPowerNotifications()
        {
            try
            {
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl", "Unregistering from Power Notifications.",
                    EventLogEntryType.Information, 50906);
#endif
                LenovoBacklightControl.WriteToDebugLog("Unregistering from Power Notifications.");
                var retVal = UnregisterPowerSettingNotification(hMonitorOn);
            }
            catch (AccessViolationException e)
            {
                Debug.WriteLine(e);
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl",
                    "Unregistering from Power Notifications failed.  Error: " + e.Message, EventLogEntryType.Error,
                    50919);
#endif
                LenovoBacklightControl.WriteToDebugLog("Unregistering from Power Notifications.");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl",
                    "Unregistering from Power Notifications failed.  Error: " + e.Message, EventLogEntryType.Error,
                    50919);
#endif
                LenovoBacklightControl.WriteToDebugLog("Unregistering from Power Notifications.");
                var retVal = UnregisterPowerSettingNotification(hMonitorOn);
            }
        }

        /// <summary>
        ///    Callback method used when power state changes and event is fired
        /// </summary>
        /// <param name="control"></param>
        /// <param name="eventType"></param>
        /// <param name="eventData"></param>
        /// <param name="context"></param>
        [HandleProcessCorruptedStateExceptions]
        private static void HandlerCallback(int control, int eventType, IntPtr eventData, IntPtr context)
        {
            try
            {
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl",
                    $"HandlerCallback(control:{control}, eventType:{eventType}, eventData:{eventData}, context: {context})",
                    EventLogEntryType.Information, 50906);
#endif
                LenovoBacklightControl.WriteToDebugLog(
                    $"HandlerCallback(control:{control}, eventType:{eventType}, eventData:{eventData}, context: {context})");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }


            switch (control)
            {
                case SERVICE_CONTROL_POWEREVENT when eventType != PBT_POWERSETTINGCHANGE:
                    return;
                case SERVICE_CONTROL_POWEREVENT:
                    try
                    {
                        //var powerSetting = new POWERBROADCAST_SETTING();
                        var powerSetting = Marshal.PtrToStructure(eventData, typeof(POWERBROADCAST_SETTING));
                        if (!powerSetting.Equals(default(POWERBROADCAST_SETTING)))
                        {
                            //Valid power event data, process it
                            UpdateStandbyState((POWERBROADCAST_SETTING) powerSetting);
                        }
                        else
                        {
#if DEBUG
                            EventLog.WriteEntry("LenovoBacklightControl", "ERROR: powersetting == null",
                                EventLogEntryType.Error, 50906);
#endif
                            LenovoBacklightControl.WriteToDebugLog("ERROR: powerSetting == null");
                        }
                    }
                    catch (AccessViolationException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    catch (Exception e)
                    {
                        EventLog.WriteEntry("LenovoBacklightControl",
                            "PowerSetting Interop failed. Error: " + e.Message, EventLogEntryType.Error, 50906);
                        LenovoBacklightControl.WriteToDebugLog("Powersetting Interop failed. Error: " + e.Message);
                    }

                    break;
                case SERVICE_CONTROL_STOP:
#if DEBUG
                    EventLog.WriteEntry("LenovoBacklightControl", "SERVICE_CONTROL_STOP received.",
                        EventLogEntryType.Information, 50906);
#endif
                    LenovoBacklightControl.WriteToDebugLog("SERVICE_CONTROL_STOP received.");
                    UnregisterFromPowerNotifications();
                    LenovoBacklightControl.LBCServiceBase.Stop();
                    break;
            }
        }

        public static IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != WM_POWERBROADCAST || wParam.ToInt32() != PBT_POWERSETTINGCHANGE) return IntPtr.Zero;

            // Extract data from message
            var powersetting = new POWERBROADCAST_SETTING();
            Marshal.PtrToStructure<POWERBROADCAST_SETTING>(lParam, powersetting);
            var presultData = (IntPtr)(lParam.ToInt32() + Marshal.SizeOf(powersetting));
            UpdateStandbyState(powersetting);

            return IntPtr.Zero;
        }

        /// <summary>
        ///   Detect when system resumes from standy and enable the backlight
        /// </summary>
        /// <param name="powersetting"></param>
        private static void UpdateStandbyState(POWERBROADCAST_SETTING powersetting)
        {
            // When the display is on (1), you are exiting standby
            if (powersetting.Data == 0 || LenovoBacklightControl.SaveBacklightState)
            {
                LenovoBacklightControl.WriteToDebugLog("Detected system resume but backlight state option enabled, not activating.");
                return;
            }
#if DEBUG
            EventLog.WriteEntry("LenovoBacklightControl", "Activating backlight.", EventLogEntryType.Information, 50905);
#endif
            LenovoBacklightControl.WriteToDebugLog("Detected system resume.  Activating backlight.");
            LenovoBacklightControl.BLC.ActivateBacklight(LenovoBacklightControl.BacklightPreference);
            //
            // Notify the settings app that the service started the backlight on it's own if option to track is enabled
            //
            if (LenovoBacklightControl.SaveBacklightState)
            {
                try
                {
                    var client = new NamedPipeClientStream(".", "LBCSettingsNamedPipe", PipeDirection.Out);
                    client.Connect();
                    var writer = new StreamWriter(client);
                    writer.WriteLine("LBCSettings-BackLightWasEnabledByPower");
                    writer.Flush();
                    client.Dispose();
                }
                catch (Exception e)
                {
                    var message = $"Error sending status update to LBCSettings app. Error: {e.Message}";
#if DEBUG
                EventLog.WriteEntry("LenovoBacklightControl", message, EventLogEntryType.Information, 50905);
#endif
                    LenovoBacklightControl.WriteToDebugLog(message);
                    LenovoBacklightControl.BLC.ActivateBacklight(LenovoBacklightControl.BacklightPreference);
                }
            }
            ConnectedStandby = false;
        }
    }
}
