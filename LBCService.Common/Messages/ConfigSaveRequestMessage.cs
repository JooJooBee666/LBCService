using TinyMessenger;

namespace LBCService.Common.Messages
{
    public class ConfigSaveRequestMessage : TinyMessageBase
    {
        public ConfigData Data { get; }

        public ConfigSaveRequestMessage(object sender, ConfigData data) : base(sender)
        {
            Data = data;
        }
    }
}