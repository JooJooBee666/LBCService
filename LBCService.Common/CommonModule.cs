using Autofac;
using TinyMessenger;

namespace LBCService.Common
{
    /// <summary>
    /// Common module registers pub-sub hub, logger and config manager.
    /// </summary>
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TinyMessengerHub>().As<ITinyMessengerHub>().SingleInstance().AutoActivate();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance().AutoActivate();
            builder.RegisterType<Config>().As<IConfig>().SingleInstance().AutoActivate();
        }
    }
}