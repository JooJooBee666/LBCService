using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace LBCService
{
    public partial class LenovoBacklightControl : ServiceBase
    {
        public static int BacklightPreference;
        public static BacklightControls BLC;
        public static ServiceBase LBCServiceBase;
        public static int UserTimeoutPreference;
        public static string KBCorePath;
        Thread IdleTimerThread;
        public static AutoResetEvent StopRequest = new AutoResetEvent(false);

        public LenovoBacklightControl()
        {
            InitializeComponent();
            DebugMode();
            BLC = new BacklightControls();
            LoadConfig();
            LBCServiceBase = this;
            UserTimeoutPreference = 1200;
            AutoLog = true;

            //
            // Register Event log source if it is not already
            //
            if (!EventLog.SourceExists("LenovoBacklightControl"))
            {
                EventLog.CreateEventSource("LenovoBacklightControl", "System");
            }
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service starting...",
                EventLogEntryType.Information, 50901);
        }

        private void LoadConfig()
        {
            var configData = XMLConfigMethods.ReadConfigXML();
            BacklightPreference = configData.Light_Level;
            KBCorePath = configData.Keyboard_Core_Path;
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service started.",
                EventLogEntryType.Information, 50902);
            PowerMethods.RegisterServiceForPowerNotifications(this);
            //IdleTimerThread = new Thread(() => IdleTimerControl.SetTimer(UserTimeoutPreference));
            //IdleTimerThread.Start();
        }

        [Conditional("DEBUG")]
        private static void DebugMode()
        {
            Debugger.Launch();
        }

        protected override void OnStop()
        {
            //EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service stopping....", EventLogEntryType.Information, 50903);
            //StopRequest.Set();

            //IdleTimerThread.Join();
        }
    }
}
