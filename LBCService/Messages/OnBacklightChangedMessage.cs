using LBCService.Common;
using TinyMessenger;

namespace LBCService.Messages
{
    public class OnBacklightChangedMessage : TinyMessageBase
    {
        public LightLevel LightValue { get; private set; }

        public OnBacklightChangedMessage(BacklightControls sender, LightLevel lightValue) : base(sender)
        {
            LightValue = lightValue;
        }
    }
}
