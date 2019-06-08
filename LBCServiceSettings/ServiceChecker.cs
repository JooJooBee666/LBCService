using System;
using System.ServiceProcess;
using System.Threading;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class ServiceChecker : IDisposable
    {
        private const string ServiceName = "LenovoBacklightControl";
        private readonly ITinyMessengerHub _hub;
        private TinyMessageSubscriptionToken _subToOpen;
        private TinyMessageSubscriptionToken _subToClose;
        private Thread _thread;
        private CancellationTokenSource _cts;

        public ServiceChecker(ITinyMessengerHub hub)
        {
            _hub = hub;
            _subToOpen = _hub.Subscribe<FormOpenedMessage>(StartChecker);
            _subToClose = _hub.Subscribe<FormClosingMessage>(StopChecker);
        }

        private void StartChecker(FormOpenedMessage message)
        {
            _cts = new CancellationTokenSource();
            _thread = new Thread(CheckService)
            {
                Name = "Checker Thread",
                IsBackground = true
            };
            _thread.Start(_cts.Token);
        }

        private void StopChecker(FormClosingMessage message)  
        {
            _cts?.Cancel();
            _thread?.Abort();
            _thread = null;
            _cts?.Dispose();
            _cts = null;
        }

        private void CheckService(object parameter)
        {
            if (!(parameter is CancellationToken token)) return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var sc = new ServiceController(ServiceName);
                    _hub.PublishAsync(new ServiceStateMessage(this, sc.Status));
                }
                catch (OperationCanceledException)
                {
                    // This is ok.
                    return;
                }
                catch (ThreadAbortException)
                {
                    // This is ok.
                    return;
                }
                catch (Exception)
                {
                    _hub.PublishAsync(new ServiceStateMessage(this, null));
                }
                Thread.Sleep(500);
            }
        }

        public void Dispose()
        {
            _subToOpen?.Dispose();
            _subToOpen = null;
            
            _subToClose?.Dispose();
            _subToClose = null;
        }
    }
}