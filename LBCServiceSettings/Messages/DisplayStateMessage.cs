using LBCService.Common;
using TinyMessenger;

namespace LBCServiceSettings.Messages
{
    public class DisplayStateMessage : TinyMessageBase
    {
        public DisplayState State { get; private set; }

        public DisplayStateMessage(DisplayHook sender, DisplayState state) : base(sender)
        {
            State = state;
        }
    }
}