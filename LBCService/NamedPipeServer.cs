using LBCService.Common;
using LBCService.Messages;
using TinyMessenger;

namespace LBCService
{
    public class NamedPipeServer : NamedPipeServerBase
    {
        private TinyMessageSubscriptionToken _subToStart;
        private TinyMessageSubscriptionToken _subToStop;

        public NamedPipeServer(ILogger logger, ITinyMessengerHub hub) : base(logger, hub, NamedPipes.ServerPipe)
        {
            _subToStart = hub.Subscribe<OnStartupMessage>(_ => StartServer());
            _subToStop = hub.Subscribe<OnStopMessage>(_ => StopServer());
        }

        public override void Dispose()
        {
            _subToStart?.Dispose();
            _subToStart = null;
            _subToStop?.Dispose();
            _subToStop = null;

            base.Dispose();
        }
    }
}