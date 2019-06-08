using System;
using System.ServiceProcess;
using Autofac;
using LBCService.Common;

namespace LBCService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // Building IoC container. All modules as singleton services since we do not consume them directly making use of pub-sub architecture with loosely couled modules.
            var builder = new ContainerBuilder();
            builder.RegisterModule(new CommonModule(true, AppDomain.CurrentDomain.BaseDirectory + "DebugLog.txt"));
            builder.RegisterType<LenovoBacklightControl>().AsSelf().SingleInstance();
            builder.RegisterType<BacklightControls>().AsSelf().SingleInstance().AutoActivate();
            //builder.RegisterType<PowerManagement>().AsSelf().SingleInstance().AutoActivate(); -- using service power states instead.
            builder.RegisterType<NamedPipeServer>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<NamedPipeClient>().WithParameter("pipeName", NamedPipes.ClientPipe).AsSelf().SingleInstance().AutoActivate();

            var container = builder.Build();

            var servicesToRun = new ServiceBase[]
            {
                container.Resolve<LenovoBacklightControl>()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}