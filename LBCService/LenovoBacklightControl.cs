using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace LBCService
{
    public partial class LenovoBacklightControl : ServiceBase
    {

        public static BacklightControls BLC;
        public LenovoBacklightControl()
        {
            InitializeComponent();
            BLC = new BacklightControls();

            if (!EventLog.SourceExists("LenovoBacklightControl"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "LenovoBacklightControl", "System");
            }

            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service starting...", EventLogEntryType.Information, 50901);
        }

        protected override void OnStart(string[] args)
        {
            DebugMode();

            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service started.", EventLogEntryType.Information, 50902);
            NativeMethods.RegisterServiceForPowerNotifications(ServiceName);

            //Activate KB backlights on boot
            BLC.ActivateBacklight();    
        }


        [Conditional("DEBUG")]
        private static void DebugMode()
        {
            Debugger.Break();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service stopping....", EventLogEntryType.Information, 50903);
            Logging.LogMessage("OnStart completed successfully.");
        }
    }
}
