using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LBCService.Common
{
    public class Logger : ILogger
    {
        private const string Source = "LenovoBacklightControl";
        private string _debugLogPath;
        private readonly bool _isInitialized;
        private readonly object _lockObject = new object();

        public bool EnableDebugLog { get; set; }

        public Logger()
        {
            if (!EventLog.SourceExists(Source))
            {
                try
                {
                    EventLog.CreateEventSource(Source, "System");
                    _isInitialized = true;
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
            else
            {
                _isInitialized = true;
            }
        }

        public void Start(string logPath)
        {
            lock (_lockObject)
            {
                _debugLogPath = logPath;
            }
        }

        public void Stop()
        {

        }

        public void Debug(string message)
        {
            WriteToDebugLog(message);
        }

        public void Info(string message, int eventId = 0)
        {
            Log(message, EventLogEntryType.Information, eventId);
        }

        public void Warn(string message, int eventId = 0)
        {
            Log(message, EventLogEntryType.Warning, eventId);
        }

        public void Error(string message, int eventId = 0)
        {
            Log(message, EventLogEntryType.Error, eventId);
        }

        public void Error(Exception ex, int eventId = 0)
        {
            Log($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", EventLogEntryType.Error, eventId);
        }

        public void Error(Exception ex, string message, int eventId = 0)
        {
            Log($"{message}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", EventLogEntryType.Error, eventId);
        }

        private void Log(string message, EventLogEntryType type, int eventId)
        {
            if (_isInitialized) EventLog.WriteEntry(Source, message, type, eventId);
            WriteToDebugLog(message);
        }

        private void WriteToDebugLog(string message)
        {
            //
            // Return if debug option is not enabled
            //
            if (!EnableDebugLog) return;

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                lock (_lockObject)
                {
                    File.AppendAllText(_debugLogPath, $"{date} - {message}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch (Exception e)
            {
                // Don't worry about this if this fails.
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}