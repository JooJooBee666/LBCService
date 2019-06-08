using TinyMessenger;

namespace LBCService.Common.Messages
{
    public class ConfigReloadRequestMessage : TinyMessageBase
    {
        public ConfigReloadRequestMessage(object sender) : base(sender)
        {
        }
    }
}