using TinyMessenger;

namespace LBCServiceSettings.Messages
{
    public class FormClosingMessage : TinyMessageBase
    {
        public FormClosingMessage(SettingsForm sender) : base(sender)
        {
        }
    }
}