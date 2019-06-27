using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LBCService.Common;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class DisplayHook : CommonHook
    {
        private IntPtr _handle;
        private Win32.DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS _recipient;

        public DisplayHook(ITinyMessengerHub hub) : base(hub)
        {
            _recipient = new Win32.DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS
            {
                Callback = HandlerCallback,
                Context = IntPtr.Zero
            };
        }

        private int HandlerCallback(IntPtr context, int eventType, IntPtr setting)
        {
            Console.WriteLine($@"HandlerCallback(context: {context}, type: {eventType}, setting: {setting})");
            if (eventType == Win32.PBT_POWERSETTINGCHANGE
                && Marshal.PtrToStructure(setting, typeof(Win32.POWERBROADCAST_SETTING)) is Win32.POWERBROADCAST_SETTING powersetting
                && powersetting.PowerSetting == Win32.GUID_CONSOLE_DISPLAY_STATE)
            {
                Hub.PublishAsync(new DisplayStateMessage(this, (DisplayState)powersetting.Data));

                switch ((DisplayState)powersetting.Data)
                {
                    case DisplayState.Off: // 0x0 - The display is off.
                        Trace.WriteLine("Display OFF");
                        break;
                    case DisplayState.On: // 0x1 - The display is on.
                        Trace.WriteLine("Display ON");
                        break;
                    case DisplayState.Dimmed: // 0x2 - The display is dimmed.
                        Trace.WriteLine("Display DIMMED");
                        break;
                }
            }

            return 0;
        }

        protected override void EnableHookInternal()
        {
            var registrationHandle = new IntPtr();
            var result = Win32.PowerSettingRegisterNotification(ref Win32.GUID_CONSOLE_DISPLAY_STATE, Win32.DEVICE_NOTIFY_CALLBACK, ref _recipient, ref registrationHandle);
            if (result == 0)
            {
                _handle = registrationHandle;
            }
            else
            {
                Trace.WriteLine($"Error hooking for power notifications: {result}");
            }
        }

        protected override void DisableHookInternal()
        {
            if (_handle != IntPtr.Zero)
            {
                Win32.PowerSettingUnregisterNotification(_handle);
            }
        }

        public override void Dispose()
        {
            DisableHookInternal();
        }
    }
}