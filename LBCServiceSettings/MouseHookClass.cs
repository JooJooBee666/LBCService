using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace LBCServiceSettings
{
    internal class MouseHookClass
    {
        private static LowLevelMouseProc MouseProc = HookCallback;

        private static IntPtr MousehookID = IntPtr.Zero;

        private const int WH_MOUSE_LL = 14;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public void EnableMouseHook()
        {
            MousehookID = SetHook(MouseProc);
        }

        public static void DisableHook()
        {
            UnhookWindowsHookEx(MousehookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,GetModuleHandle(currentModule.ModuleName), 0);
            }
        }


        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(MousehookID, nCode, wParam, lParam);

            // mouse input detected, restart the idle timer and enable the KB 
            // backlight if it was off due to previous timeout
            IdleTimerControl.RestartTimer();
            if (!IdleTimerControl.BackLightOn)
            {
                SettingsForm.EnableBacklight();
            }
            return CallNextHookEx(MousehookID, nCode, wParam, lParam);
        }
    }
}
