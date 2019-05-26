using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using LBCService.Common.Messages;
using TinyMessenger;

namespace LBCService.Common
{
    public abstract class NamedPipeServerBase : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;
        private readonly string _pipeName;
        private Thread _thread;
        private CancellationTokenSource _cts;

        protected NamedPipeServerBase(ILogger logger, ITinyMessengerHub hub, string pipeName)
        {
            _logger = logger;
            _hub = hub;
            _pipeName = pipeName;
        }

        protected void StartServer()
        {
            _logger.Debug("Starting NamedPipe server");
            _cts = new CancellationTokenSource();
            _thread = new Thread(ProcessMessages)
            {
                Name = $"{_pipeName}Thread",
                IsBackground = true
            };
            _thread.Start(_cts.Token);
        }

        protected void StopServer()
        {
            _logger.Debug("Stopping NamedPipe server");
            _cts?.Cancel();
            _thread?.Abort();
            _thread = null;
            _cts?.Dispose();
            _cts = null;
        }

        private async void ProcessMessages(object parameter)
        {
            if (!(parameter is CancellationToken token)) return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var server = CreateServer(_pipeName))
                    {
                        await server.WaitForConnectionAsync(token);
                        if (token.IsCancellationRequested) return;
                        string line;
                        // Read data from client
                        using (var reader = new StreamReader(server))
                        {
                            line = await reader.ReadToEndAsync();
                        }

                        var status = line.ParseStatus();
                        _hub.PublishAsync(new OnStatusReceivedMessage(this, status));

                        if (server.IsConnected) server.Disconnect();
                    }
                }
                catch (OperationCanceledException)
                {
                    // This is ok.
                    return;
                }
                catch (ThreadAbortException)
                {
                    // This is ok.
                    return;
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Error reading from NamedPipe '{_pipeName}'", 50915);
                }
            }
        }

        private static NamedPipeServerStream CreateServer(string pipeName)
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

            // Create pipe
            return new NamedPipeServerStream(
                pipeName,
                PipeDirection.In,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                0,
                0,
                pipeSecurity);
        }

        public virtual void Dispose()
        {
            StopServer();
        }
    }
}
