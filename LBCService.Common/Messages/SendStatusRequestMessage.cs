using TinyMessenger;

namespace LBCService.Common.Messages
{
    public class SendStatusRequestMessage : TinyMessageBase
    {
        public Status Status { get; private set; }

        public SendStatusRequestMessage(object sender, Status status) : base(sender)
        {
            Status = status;
        }
    }
}