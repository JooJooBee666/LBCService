using TinyMessenger;

namespace LBCServiceSettings.Messages
{
    public class FormOpenedMessage : TinyMessageBase
    {
        public FormOpenedMessage(SettingsForm sender) : base(sender)
        {
        }
    }
}