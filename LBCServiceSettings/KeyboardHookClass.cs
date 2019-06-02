using System;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class KeyboardHookClass : CommonHook
    {
        private readonly ITinyMessengerHub _hub;
        private IntPtr _keyboardHookId = IntPtr.Zero;
        private DateTime _lastStateSent = DateTime.Today;
        private Win32.HookProc _callback;

        public KeyboardHookClass(ITinyMessengerHub hub)
        {
            _hub = hub;
        }

        protected override void EnableHookInternal()
        {
            _callback = HookCallback;
            if (_keyboardHookId == IntPtr.Zero)
            {
                _keyboardHookId =
                    Win32.SetWindowsHookEx(Win32.HookType.WH_KEYBOARD_LL, _callback, Win32.GetModule(), 0);
            }
        }

        protected override void DisableHookInternal()
        {
            if (_keyboardHookId != IntPtr.Zero)
            {
                Win32.UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
                _callback = null;
            }
        }

        private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0) return Win32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            // throttle down notifications to at least once per 200ms.
            if (wParam.ToInt32() == Win32.WM_KEYDOWN && (DateTime.Now - _lastStateSent).TotalMilliseconds > 200)
            {
                _hub.PublishAsync(new UserActiveMessage(this));
                _lastStateSent = DateTime.Now;
            }
            return Win32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public override void Dispose()
        {
            DisableHook();
        }
    }
}