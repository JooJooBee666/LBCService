using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceProcess;

namespace LBCService
{
    public partial class LenovoBacklightControl : ServiceBase
    {

        public static BacklightControls BLC;
        public LenovoBacklightControl()
        {
            InitializeComponent();
            BLC = new BacklightControls();

            //
            // Register Event log source if it is not already
            //
            if (!EventLog.SourceExists("LenovoBacklightControl"))
            {
                EventLog.CreateEventSource("LenovoBacklightControl", "System");
            }

            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service starting...", EventLogEntryType.Information, 50901);
        }

        protected override void OnStart(string[] args)
        {
            DebugMode();

            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service started.", EventLogEntryType.Information, 50902);
            PowerMethods.RegisterServiceForPowerNotifications(this);

            //Activate KB Backlights on boot
            //BLC.ActivateBacklight();    
        }

        [Conditional("DEBUG")]
        private static void DebugMode()
        {
            Debugger.Launch();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service stopping....", EventLogEntryType.Information, 50903);
        }
    }
}
