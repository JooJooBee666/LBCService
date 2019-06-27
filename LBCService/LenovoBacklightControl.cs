using System.Diagnostics;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCService.Messages;
using TinyMessenger;

namespace LBCService
{
    public partial class LenovoBacklightControl : CustomServiceBase
    {
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;
        private TinyMessageSubscriptionToken _subToStatus;
        private TinyMessageSubscriptionToken _subToConfig;
        private TinyMessageSubscriptionToken _subToStop;

        public LenovoBacklightControl(ILogger logger, ITinyMessengerHub hub)
        {
            _logger = logger;
            _hub = hub;

            InitializeComponent();
            
            DebugMode();
            
            _logger.Start();
            _logger.Info("LenovoBacklightControl service starting...", 50901);
        }

        [Conditional("DEBUG")]
        private static void DebugMode()
        {
            Debugger.Launch();
        }

        protected override void ServiceStart()
        {
            _logger.Start();
            _subToStatus = _hub.Subscribe<OnStatusReceivedMessage>(OnStatusReceived);
            _subToConfig = _hub.Subscribe<OnConfigLoadedMessage>(OnConfigLoaded);
            _subToStop = _hub.Subscribe<StopServiceRequestMessage>(_ => Stop());

            _hub.Publish(new ConfigReloadRequestMessage(this));
            _hub.Publish(new OnStartupMessage(this));
            _logger.Info("LenovoBacklightControl service started.", 50902);
        }

        protected override void ServiceStop()
        {
            _logger.Debug("Received stop.");
            
            _subToStatus?.Dispose();
            _subToStatus = null;
            _subToConfig?.Dispose();
            _subToConfig = null;
            _subToStop?.Dispose();
            _subToStop = null;

            _hub.Publish(new OnStopMessage(this));
            _logger.Debug("Stop complete.");
            _logger.Stop();
        }

        protected override void ServicePause()
        {
            // For now treat pause as stop.
            ServiceStop();
        }

        private void OnStatusReceived(OnStatusReceivedMessage message)
        {
            // Read status from client
            switch (message.Status)
            {
                case Status.Close: // Not used, can be removed.
                    Stop();
                    return;
                case Status.EnableBacklight:
                    _hub.PublishAsync(new ActivateBacklightRequestMessage(this));
                    break;
                case Status.DisableBacklight:
                    _hub.PublishAsync(new DeactivateBacklightRequestMessage(this));
                    break;
                case Status.UpdateConfig:
                    _hub.PublishAsync(new ConfigReloadRequestMessage(this));
                    break;
                case Status.RequestBacklightState:
                    _hub.PublishAsync(new GetBacklightStateRequestMessage(this));
                    break;
                default:
                    _logger.Error($"Status message '{message.Status}' not recognized.");
                    return;
            }
        }

        private void OnConfigLoaded(OnConfigLoadedMessage message)
        {
            _logger.EnableDebugLog = message.Data.EnableDebugLog;
            if (_logger.EnableDebugLog) _logger.Debug("Debug log enabled.");
        }
    }
}