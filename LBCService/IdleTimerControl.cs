using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LBCService
{
    class IdleTimerControl
    {
        public static bool BackLightOn;
        private static object ThreadLocker;
        private static System.Timers.Timer IdleTimer;
        public static int LastReportedIdleTime;

        public static void RestartTimer()
        {
            lock (ThreadLocker)
            {
                IdleTimer.Stop();
                IdleTimer.Start();
            }
        }
        public static void SetTimer(int TimeOut)
        {

            // Create a timer
            IdleTimer = new System.Timers.Timer(TimeOut*1000);
            // Hook up the Elapsed event for the timer. 
            IdleTimer.Elapsed += TimeoutReached;
            IdleTimer.AutoReset = true;
            IdleTimer.Enabled = true;
            PipeServer();
        }

        private static void PipeServer()
        {
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var rule = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, AccessControlType.Allow);
            var sec = new PipeSecurity();
            sec.AddAccessRule(rule);
            var fBytes = new byte[64];
            while (true)
            {
                using (var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1,
                    PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, sec))
                {
                    pipeServer.WaitForConnection();

                    var read = 0;
                    var bytes = new byte[64];
                    while ((read = pipeServer.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        fBytes = bytes;
                    }
                }
                LastReportedIdleTime = BitConverter.ToInt32(fBytes, 0);
            }
        }

        private static void WaitLoop()
        {
               
        }

        /// <summary>
        /// Check the idle time and disable backlight if passed
        /// Re-enable if timer get's reset (i.e. user activity)
        /// </summary>
        public static void TimeoutReached(object source, ElapsedEventArgs e)
        {
            if (BackLightOn)
            {
                //LenovoBacklightControl.BLC.ActivateBacklight(0);
            }
        }
    }
}
