using TinyMessenger;

namespace LBCService.Common.Messages
{
    public class OnStatusReceivedMessage : TinyMessageBase
    {
        public Status Status { get; }

        public OnStatusReceivedMessage(NamedPipeServerBase sender, Status status) : base(sender)
        {
            Status = status;
        }
    }
}