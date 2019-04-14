using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace LBCService
{
    public partial class LenovoBacklightControl : ServiceBase
    {
        public static int BacklightPreference;
        public static BacklightControls BLC;
        public static ServiceBase LBCServiceBase;
        public static int UserTimeoutPreference;
        public static string KBCorePath;
        private static Thread NamedPipeThread;
        public static AutoResetEvent StopRequest = new AutoResetEvent(false);
        public static string DebugLogPath;
        public static bool EnableDebugLog;
        public static bool SaveBacklightState;

        public LenovoBacklightControl()
        {
            InitializeComponent();
            DebugMode();
            DebugLogPath = AppDomain.CurrentDomain.BaseDirectory + "DebugLog.txt";
            BLC = new BacklightControls();
            LoadConfig();
            LBCServiceBase = this;
            UserTimeoutPreference = 30;
            AutoLog = true;

            //
            // Register Event log source if it is not already
            //
            if (!EventLog.SourceExists("LenovoBacklightControl"))
            {
                try
                {
                    EventLog.CreateEventSource("LenovoBacklightControl", "System");
                }
                catch (Exception e)
                {
                    var error = $"Error creating DebugLogFile at {DebugLogPath}. {e.Message}";
                    WriteToDebugLog(error);
                }
            }
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service starting...", EventLogEntryType.Information, 50901);
            WriteToDebugLog("LenovoBacklightControl service starting...");
        }


        /// <summary>
        /// Load config data from XML
        /// </summary>
        public static void LoadConfig()
        {
            var configData = XMLConfigMethods.ReadConfigXML();
            BacklightPreference = configData.Light_Level;
            KBCorePath = configData.Keyboard_Core_Path;
        }

        /// <summary>
        /// Write to debug file if enabled
        /// </summary>
        /// <param name="message">The message to write to the debug file.</param>
        public static void WriteToDebugLog(string message)
        {
            //
            // Return if debug option is not enabled
            //
            if (!EnableDebugLog) return;

            //
            // Create Debug File if it doesn't exists
            //
            if (!File.Exists(DebugLogPath))
            {
                try
                {
                    File.Create(DebugLogPath).Dispose();
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("LenovoBacklightControl", $"Error creating DebugLogFile at {DebugLogPath}. {e.Message}", EventLogEntryType.Information, 50920);
                }
            }

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                using (StreamWriter sw = File.AppendText(DebugLogPath))
                {
                    sw.WriteLine($"{date} - {message}");
                }
            }
            catch (Exception e)
            {
                // Don't worry about this if this fails.
                Debug.WriteLine(e);
            }

        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("LenovoBacklightControl", "LenovoBacklightControl service started.", EventLogEntryType.Information, 50902);
            WriteToDebugLog("LenovoBacklightControl service started.");
            PowerMethods.RegisterServiceForPowerNotifications(this);
            NamedPipeThread = new Thread(NamedPipeServer.EnableNamedPipeServer);
            NamedPipeThread.IsBackground = true;
            NamedPipeThread.Start();
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
            WriteToDebugLog("Received stop.");
            NamedPipeServer.StopNamedPipe();
            NamedPipeThread.Join();
            WriteToDebugLog("Stop complete.");
        }
    }
}
