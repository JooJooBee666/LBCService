using TinyMessenger;

namespace LBCService.Messages
{
    public class StopServiceRequestMessage : TinyMessageBase
    {
        public StopServiceRequestMessage(object sender) : base(sender)
        {
        }
    }
}