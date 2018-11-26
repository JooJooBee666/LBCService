using System;

using System.Timers;

namespace LBCServiceSettings
{
    class IdleTimerControl
    {
        public static object ThreadLocker;
        public static Timer IdleTimer;
        public static void RestartTimer()
        {
            lock (ThreadLocker)
            {
                IdleTimer.Stop();
                IdleTimer.Start();
            }
        }

        public static bool BackLightOn;

        public static void SetTimer(int TimeOut)
        {
            ThreadLocker = new object();

            // Create a timer
            IdleTimer = new System.Timers.Timer(TimeOut*1000);
            // Hook up the Elapsed event for the timer. 
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
                SettingsForm.EnableBacklight();
            }
        }
    }
}
