using Autofac;
using TinyMessenger;

namespace LBCService.Common
{
    /// <summary>
    /// Common module registers pub-sub hub, logger and config manager.
    /// </summary>
    public class CommonModule : Module
    {
        private readonly bool _useEventLog;
        private readonly string _logFile;

        public CommonModule(bool useEventLog, string logFile)
        {
            _useEventLog = useEventLog;
            _logFile = logFile;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TinyMessengerHub>().As<ITinyMessengerHub>().SingleInstance().AutoActivate();
            builder.RegisterType<Logger>().WithParameter("useEventLog", _useEventLog).WithParameter("logFile", _logFile)
                .As<ILogger>().SingleInstance().AutoActivate();
            builder.RegisterType<Config>().As<IConfig>().SingleInstance().AutoActivate();
        }
    }
}