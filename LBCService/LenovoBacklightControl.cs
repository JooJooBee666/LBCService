using System;
using System.Diagnostics;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCService.Messages;
using TinyMessenger;

namespace LBCService
{
    public partial class LenovoBacklightControl : CustomServiceBase
    {
        private static readonly string DebugLogPath = AppDomain.CurrentDomain.BaseDirectory + "DebugLog.txt";
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;
        private readonly IConfig _config;
        //private TinyMessageSubscriptionToken _subToPower;
        private TinyMessageSubscriptionToken _subToStatus;
        private TinyMessageSubscriptionToken _subToConfig;
        private TinyMessageSubscriptionToken _subToStop;

        public LenovoBacklightControl(ILogger logger, ITinyMessengerHub hub, IConfig config)
        {
            _logger = logger;
            _hub = hub;
            _config = config;

            InitializeComponent();
            
            DebugMode();
            
            _logger.Start(DebugLogPath);
            _logger.Info("LenovoBacklightControl service starting...", 50901);
        }

        [Conditional("DEBUG")]
        private static void DebugMode()
        {
            Debugger.Launch();
        }

        protected override void ServiceStart()
        {
            _logger.Start(DebugLogPath);
            //_subToPower = _hub.Subscribe<PowerStateMessage>(OnPowerStateChanged);
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
            
            //_subToPower?.Dispose();
            //_subToPower = null;
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

        //private void OnPowerStateChanged(PowerStateMessage message)
        //{
        //    if (_config.Data == null) return;
        //    if (!message.IsDisplayOn || !_config.Data.SaveBacklightState) // TODO: Do we need this check?
        //    {
        //        _logger.Debug("Detected system resume but backlight state option enabled, not activating.");
        //        return;
        //    }
        //    //
        //    // Notify the settings app that the service started the backlight on it's own if option to track is enabled
        //    //
        //    if (_config.Data.SaveBacklightState)
        //    {
        //        _logger.Debug("Detected system resume. Activating backlight.");
        //        _hub.PublishAsync(new ActivateBacklightRequestMessage(this));
        //        _hub.PublishAsync(new SendStatusRequestMessage(this, Status.BackLightWasEnabledByPower));
        //    }
        //}

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