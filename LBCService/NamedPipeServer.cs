using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Timers;


namespace LBCService
{
    class NamedPipeServer
    {
        private static object ThreadLocker;
        public static int LastReportedIdleTime;
        private static NamedPipeServerStream PipeServer;

        public static void EnableNamedPipeServer()
        {
            ThreadLocker = new object();
            StartPipeServer();
        }

        /// <summary>
        ///     Start a new pipe server
        /// </summary>
        private static void StartPipeServer()
        {
            // Create a new pipe accessible by local authenticated users, disallow network
            var sidNetworkService = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
            var sidWorld = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var pipeSecurity = new PipeSecurity();

            // Alow Everyone to read/write to pipe
            var accessRule = new PipeAccessRule(sidWorld, PipeAccessRights.ReadWrite, AccessControlType.Allow);
            pipeSecurity.AddAccessRule(accessRule);

            // Deny network access to the named pipe
            accessRule = new PipeAccessRule(sidNetworkService, PipeAccessRights.ReadWrite, AccessControlType.Deny);
            pipeSecurity.AddAccessRule(accessRule);

            // Make current user (LOCAL SYSTEM) the owner
            //var sidOwner = WindowsIdentity.GetCurrent().Owner;
            //if (sidOwner != null)
            //{
            //    accessRule = new PipeAccessRule(sidOwner, PipeAccessRights.FullControl, AccessControlType.Allow);
            //    pipeSecurity.AddAccessRule(accessRule);
            //}

            // Create pipe and start the async connection waiting
            PipeServer = new NamedPipeServerStream(
                "LenovoBacklightControlPipe",
                PipeDirection.In,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                0,
                0,
                pipeSecurity);

            // Begin async and wait for connections
            PipeServer.BeginWaitForConnection(NamedPipeServerConnectionCallback, PipeServer);
        }

        /// <summary>
        ///     Function called when a client connects to the named pipe.
        /// </summary>
        /// <param name="iAsyncResult"></param>
        private static void NamedPipeServerConnectionCallback(IAsyncResult iAsyncResult)
        {
            try
            {
                // End waiting for the connection
                PipeServer.EndWaitForConnection(iAsyncResult);

                // Read data and prevent access to pip during threaded operations
                lock (ThreadLocker)
                {
                    // Read data from client
                    var reader = new StreamReader(PipeServer);
                    var status = reader.ReadLine();
                    switch (status)
                    {
                        case "LBC-EnableBacklight":
                            LenovoBacklightControl.BLC.ActivateBacklight(LenovoBacklightControl.BacklightPreference);
                            break;
                        case "LBC-DisableBacklight":
                            LenovoBacklightControl.BLC.ActivateBacklight(0);
                            break;
                        case "LBC-UpdateConfigData":
                            var currentBacklightPref = LenovoBacklightControl.BacklightPreference;
                            LenovoBacklightControl.LoadConfig();
                            if (currentBacklightPref != LenovoBacklightControl.BacklightPreference)
                            {
                                //backlight preference was changed, let's set it to the new value
                                LenovoBacklightControl.BLC.ActivateBacklight(LenovoBacklightControl.BacklightPreference);
                            }
                            break;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // EndWaitForConnection will cause exception when someone closes the pipe before connection was made
                // Don't create another pipe, just return
                // Occurs when app is closing and the pipe is closed/disposed
                return;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("LenovoBacklightControl", $"Problem parsing pipe data. Error:{e.Message}",
                    EventLogEntryType.Error, 50915);
            }
            finally
            {
                // Close the pipe so a new one can be created
                PipeServer.Dispose();
            }

            // Create a new pipe for next connection
            StartPipeServer();
        }

        public static void StopNamedPipe()
        {
            PipeServer.Dispose();
        }
    }
}
