using System.ServiceProcess;
using TinyMessenger;

namespace LBCService.Messages
{
    public class OnStopMessage : TinyMessageBase
    {
        public OnStopMessage(ServiceBase sender) : base(sender)
        {
        }
    }
}