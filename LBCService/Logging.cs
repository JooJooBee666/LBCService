using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LBCService
{
    public static class Logging
    {
        public static void Initialize(string fileName)
        {
            Trace.AutoFlush = true;

            var fi = new FileInfo(fileName);

            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            var logWriter = new TextWriterTraceListener(fi.FullName);
            if (logWriter != null)
            {
                Trace.Listeners.Add(logWriter);
            }

            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var configAttribute = executingAssembly.GetCustomAttributes(false).SingleOrDefault(v => v.GetType() == typeof(System.Reflection.AssemblyConfigurationAttribute)) as System.Reflection.AssemblyConfigurationAttribute;
            var configurationString = string.Empty;
            if (configAttribute != null)
            {
                configurationString = configAttribute.Configuration;
            }

            var startMessage = $"Log File Started - {executingAssembly.GetName().Version} - {configurationString} - {System.Environment.MachineName}";

            Trace.WriteLine(new string('=', startMessage.Length));
            Trace.Flush();

            // Will always be printed verbose.
            LogMessage(startMessage);
        }

        public static void LogMessage(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1)
        {
            var fi = filePath == null ? null : new FileInfo(filePath);

            if (string.IsNullOrEmpty(message))
            {
                message = "BEGIN";
            }

            var currentThread = Thread.CurrentThread;
            var threadInfoString = string.Format(CultureInfo.InvariantCulture, "T{0:D3}", currentThread.ManagedThreadId);
            var callerInfo = string.Format("F:{0},M:{1},L:{2}", fi == null ? "NULL" : fi.Name, memberName, lineNumber);
            var logString = string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}|{3}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff"), threadInfoString, callerInfo, message);
            Trace.WriteLine(logString);
            Trace.Flush();
        }
    }
}
