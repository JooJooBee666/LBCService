using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LBCServiceSettings
{
    class KeyboardHookClass
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc KBProc = HookCallback;
        private static IntPtr KeyboardHookID = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public void EnableKBHook()
        {
            KeyboardHookID = SetHook(KBProc);
        }

        public static void DisableKBHook()
        {
            UnhookWindowsHookEx(KeyboardHookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || wParam != (IntPtr) WM_KEYDOWN)
                return CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);

            // Keyboard input detected, restart the idle timer and enable the KB 
            // backlight if it was off due to previous timeout
            IdleTimerControl.RestartTimer();
            if (!IdleTimerControl.BackLightOn)
            {
                SettingsForm.EnableBacklight();
            }
            return CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }


    }
}
