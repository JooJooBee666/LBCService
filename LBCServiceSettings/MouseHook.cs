﻿using System;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class MouseHook : CommonHook
    {
        private IntPtr _mousehookId = IntPtr.Zero;
        private DateTime _lastStateSent = DateTime.Today;
        private Win32.HookProc _callback;
        
        public MouseHook(ITinyMessengerHub hub) : base(hub) { }

        protected override void EnableHookInternal()
        {
            _callback = HookCallback;
            if (_mousehookId == IntPtr.Zero)
            {
                _mousehookId = Win32.SetWindowsHookEx(Win32.HookType.WH_MOUSE_LL, _callback, Win32.GetModule(), 0);
            }
        }

        protected override void DisableHookInternal()
        {
            if (_mousehookId != IntPtr.Zero)
            {
                Win32.UnhookWindowsHookEx(_mousehookId);
                _mousehookId = IntPtr.Zero;
                _callback = null;
            }
        }

        private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0) return Win32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            // throttle down notifications to at least once per 200ms.
            if ((DateTime.Now - _lastStateSent).TotalMilliseconds > 200)
            {
                Hub.PublishAsync(new UserActiveMessage(this));
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