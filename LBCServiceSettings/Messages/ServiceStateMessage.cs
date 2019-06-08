using System.ServiceProcess;
using TinyMessenger;

namespace LBCServiceSettings.Messages
{
    public class ServiceStateMessage : TinyMessageBase
    {
        public ServiceControllerStatus? Status { get; }

        public ServiceStateMessage(ServiceChecker sender, ServiceControllerStatus? status) : base(sender)
        {
            Status = status;
        }
    }
}