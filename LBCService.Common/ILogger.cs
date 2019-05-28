using System;

namespace LBCService.Common
{
    public interface ILogger
    {
        bool EnableDebugLog { get; set; }

        void Start();

        void Stop();

        void Debug(string message);

        void Info(string message, int eventId = 0);

        void Warn(string message, int eventId = 0);

        void Error(string message, int eventId = 0);

        void Error(Exception ex, int eventId = 0);

        void Error(Exception ex, string message, int eventId = 0);
    }
}