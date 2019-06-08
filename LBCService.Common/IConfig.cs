namespace LBCService.Common
{
    public interface IConfig
    {
        bool Save(ConfigData data);

        ConfigData Load();
    }
}