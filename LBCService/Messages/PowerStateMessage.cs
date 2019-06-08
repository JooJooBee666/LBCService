using TinyMessenger;

namespace LBCService.Messages
{
    public class PowerStateMessage : TinyMessageBase
    {
        public bool IsDisplayOn { get; private set; }

        public PowerStateMessage(object sender, bool isDisplayOn) : base(sender)
        {
            IsDisplayOn = isDisplayOn;
        }
    }
}