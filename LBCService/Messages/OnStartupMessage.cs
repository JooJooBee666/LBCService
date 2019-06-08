using System.ServiceProcess;
using TinyMessenger;

namespace LBCService.Messages
{
    public class OnStartupMessage : TinyMessageBase
    {
        public string ServiceName { get; private set; }

        public OnStartupMessage(ServiceBase sender) : base(sender)
        {
            ServiceName = sender.ServiceName;
        }
    }
}