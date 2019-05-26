using TinyMessenger;

namespace LBCService.Common.Messages
{
    public class OnConfigLoadedMessage : TinyMessageBase
    {
        public ConfigData Data { get; }

        public OnConfigLoadedMessage(IConfig sender, ConfigData data) : base(sender)
        {
            Data = data;
        }
    }
}