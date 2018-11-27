using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace LBCServiceSettings
{
    class IdleTimerControl
    {
        public static object ThreadLocker;
        public static Timer IdleTimer;
        public static int UserTimeout;
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public static void RestartTimer()
        {
            IdleTimer.Stop();
            IdleTimer.Interval = UserTimeout * 1000;
            IdleTimer.Start();

            //enable backlight if it was off
            if (!BackLightOn)
            {
                // Send the message to enable the backlight using a thread
                var t = new Thread(() => SettingsForm.LBCServiceUpdateNotify("LBC-EnableBacklight"));
                t.Start();
                BackLightOn = true;
            }
        }

        public static bool BackLightOn;

        public static void SetTimer(int TimeOut)
        {
            ThreadLocker = new object();
            BackLightOn = true;
            UserTimeout = TimeOut;

            //enable mouse and Keyboard hooks
            KeyboardHookClass.EnableKBHook();
            MouseHookClass.EnableMouseHook();

            // Create a timer
            IdleTimer = new Timer(TimeOut*1000);
            // Hook up the Elapsed event for the timer. 
            IdleTimer.Elapsed += TimeoutReached;
            IdleTimer.AutoReset = true;
            IdleTimer.Enabled = true;
        }

        public static void ResetTimer(int TimeOut)
        {
            IdleTimer.Stop();
            IdleTimer.Dispose();
            IdleTimer = new Timer(TimeOut * 1000);
            IdleTimer.Elapsed += TimeoutReached;
            IdleTimer.AutoReset = true;
            IdleTimer.Enabled = true;
        }

        /// <summary>
        /// Check the idle time and disable backlight if passed
        /// Re-enable if timer get's reset (i.e. user activity)
        /// </summary>
        public static void TimeoutReached(object source, ElapsedEventArgs e)
        {
            if (BackLightOn)
            {
                // Send the message to disable the backlight using a thread
                var t = new Thread(() => SettingsForm.LBCServiceUpdateNotify("LBC-DisableBacklight"));
                t.Start();
                BackLightOn = false;
            }
        }

        public static int IdleTime() //In seconds
        {
            LASTINPUTINFO lastinputinfo = new LASTINPUTINFO();
            lastinputinfo.cbSize = (uint)Marshal.SizeOf(lastinputinfo);
            GetLastInputInfo(ref lastinputinfo);
            return (int)((((Environment.TickCount & int.MaxValue) - (lastinputinfo.dwTime & int.MaxValue)) & int.MaxValue) / 1000);
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            var lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            var lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }
    }
}
