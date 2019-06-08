using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using LBCService.Common.Messages;
using TinyMessenger;

namespace LBCService.Common
{
    public class NamedPipeClient : IDisposable
    {
        private const string LocalMachine = ".";
        private const int Timeout = 1 * 1000; // 1 sec in ms
        private readonly ILogger _logger;
        private readonly string _pipeName;
        private TinyMessageSubscriptionToken _subToSend;

        public NamedPipeClient(ILogger logger, ITinyMessengerHub hub, string pipeName)
        {
            _logger = logger;
            _pipeName = pipeName;

            _subToSend = hub.Subscribe<SendStatusRequestMessage>(OnSendStatusMessage);
        }

        private void OnSendStatusMessage(SendStatusRequestMessage message)
        {
            Send(message.Status);
        }

        private void Send(Status status)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var client = new NamedPipeClientStream(LocalMachine, _pipeName, PipeDirection.Out))
                    {
                        client.Connect(Timeout);
                        using (var writer = new StreamWriter(client))
                        {
                            writer.WriteLine(status.ConvertToString());
                            writer.Flush();
                            if (client.IsConnected) client.WaitForPipeDrain();
                        }
                        
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Error sending '{status}' to '{_pipeName}'");
                }
            });
        }

        public void Dispose()
        {
            _subToSend?.Dispose();
            _subToSend = null;
        }
    }
}