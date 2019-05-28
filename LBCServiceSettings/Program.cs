using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using LBCService.Common;

namespace LBCServiceSettings
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        static void Main()
        {
            // Building IoC container. All modules as singleton services since we do not consume them directly making use of pub-sub architecture with loosely couled modules.
            var builder = new ContainerBuilder();
            builder.RegisterModule(new CommonModule(false, AppDomain.CurrentDomain.BaseDirectory + "SettingsLog.txt"));
            builder.RegisterType<SettingsForm>().AsSelf().SingleInstance();
            builder.RegisterType<ServiceChecker>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<SysTray>().AsSelf().SingleInstance();
            builder.RegisterType<IdleTimerControl>().AsSelf().SingleInstance();
            builder.RegisterType<KeyboardHookClass>().AsSelf().SingleInstance();
            builder.RegisterType<MouseHookClass>().AsSelf().SingleInstance();
            // builder.RegisterType<NamedPipeServer>().AsSelf().SingleInstance().AutoActivate(); -- for now we are not listening to service.
            builder.RegisterType<NamedPipeClient>().WithParameter("pipeName", NamedPipes.ServerPipe).AsSelf().SingleInstance().AutoActivate();

            var container = builder.Build();
            
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.Resolve<SettingsForm>());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled UI Exception");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception");
        }
    }
}
