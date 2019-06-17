using LBCService.Common;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class NamedPipeServer : NamedPipeServerBase
    {
        public NamedPipeServer(ILogger logger, ITinyMessengerHub hub) : base(logger, hub, NamedPipes.ClientPipe)
        {
        }

        public void Start()
        {
            base.StartServer();
        }

        public void Stop()
        {
            base.StopServer();
        }
    }
}