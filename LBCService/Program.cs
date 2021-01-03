using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LBCService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        static void Main()
        {
            StartService();
        }

        [HandleProcessCorruptedStateExceptions]
        static void StartService()
        {
            try
            {
                var ServicesToRun = new ServiceBase[]
                {
                    new LenovoBacklightControl(),
                };

                ServiceBase.Run(ServicesToRun);
            }
            catch (Win32Exception e)
            {
                Debug.WriteLine(e);
            }
            catch (AccessViolationException ex)
            {
                EventLog.WriteEntry("LenovoBacklightControl", "LenovoBackLight control Access Violation. Error: " + ex.Message, EventLogEntryType.Error, 50950);
                LenovoBacklightControl.WriteToDebugLog("LenovoBacklightControl service restarting...");
                Debug.WriteLine(ex);
                StartService();
            }
        }
    }
}
