using System;
using System.Reflection;
using ImpromptuInterface;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCService.Messages;
using TinyMessenger;

namespace LBCService
{
    public class BacklightControls : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;

        private IBacklightControl _kcInstance;
        private TinyMessageSubscriptionToken _subToConfig;
        private TinyMessageSubscriptionToken _subToActivate;
        private TinyMessageSubscriptionToken _subToDeactivate;
        private TinyMessageSubscriptionToken _subToStop;
        private TinyMessageSubscriptionToken _subToStart;

        public BacklightControls(ILogger logger, ITinyMessengerHub hub)
        {
            _logger = logger;
            _hub = hub;
            
            // React to config changes.
            _subToConfig = _hub.Subscribe<OnConfigLoadedMessage>(message =>
            {
                var data = message.Data;
                Clear();
                Initialize(data.KeyboardCorePath);

                var lightLevel = CheckBacklightStatus();
                if (!lightLevel.HasValue ||
                    (lightLevel.Value != LightLevel.Off && lightLevel.Value != data.LightLevel))
                {
                    // backlight preference has changed, let's set it to the new value.
                    ActivateBacklight(data.LightLevel);
                }

                // React to direct request to change backlight.
                _subToActivate = _hub.Subscribe<ActivateBacklightRequestMessage>(_ => ActivateBacklight(data.LightLevel));
                _subToDeactivate = _hub.Subscribe<DeactivateBacklightRequestMessage>(_ => ActivateBacklight(LightLevel.Off));
                // Save backlight state on stop.
                _subToStop = _hub.Subscribe<OnStopMessage>(_ =>
                {
                    data.SavedState = CheckBacklightStatus() ?? LightLevel.Off;

                    _hub.Publish(new ConfigSaveRequestMessage(this, data));
                });
                // Restore backlight state on start.
                _subToStart = _hub.Subscribe<OnStartupMessage>(_ =>
                {
                    if (data.SaveBacklightState)
                    {
                        ActivateBacklight(data.SavedState);
                    }
                });
            });
        }

        private void ActivateBacklight(LightLevel lightLevel)
        {
            if (_kcInstance != null)
            {
                try
                {
                    if (_kcInstance.SetKeyboardBackLightStatus((int)lightLevel) == 0)
                    {
                        _hub.PublishAsync(new OnBacklightChangedMessage(this, lightLevel));
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Unable to set backlight level to '{lightLevel}'");
                }
            }
        }

        private LightLevel? CheckBacklightStatus()
        {
            if (_kcInstance != null)
            {
                try
                {
                    if (_kcInstance.GetKeyboardBackLightStatus(out var lightValue) == 0)
                    {
                        return (LightLevel) lightValue;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Unable to get backlight level");
                }
            }

            return null;
        }

        private void Initialize(string kbCorePath)
        {
            if (_kcInstance == null)
            {
                if (!string.IsNullOrWhiteSpace(kbCorePath) && System.IO.File.Exists(kbCorePath))
                {
                    try
                    {
                        // Dynamicly load Keyboard_Core.dll and it's dependencies. Instantiate KeyboardControl class and cast it to a known interface. 
                        var assembly = Assembly.LoadFrom(kbCorePath);
                        var assemblyType = assembly.GetType("Keyboard_Core.KeyboardControl");
                        var instance = Activator.CreateInstance(assemblyType);
                        _kcInstance = instance.ActLike<IBacklightControl>();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Unable to load Keyboard_Core.dll. Error:", 50903);
                        //_hub.PublishAsync(new StopServiceRequestMessage(this)); // Simply ignore this
                        return;
                    }
                }
                else
                {
                    _logger.Error("Unable to locate Keyboard_Core.dll.", 50903);
                    //_hub.PublishAsync(new StopServiceRequestMessage(this)); // Simply ignore this
                    return;
                }
            }
        }

        private void Clear()
        {
            _kcInstance = null;

            _subToActivate?.Dispose();
            _subToActivate = null;
            _subToDeactivate?.Dispose();
            _subToDeactivate = null;
            _subToStop?.Dispose();
            _subToStop = null;
            _subToStart?.Dispose();
            _subToStart = null;
        }

        public void Dispose()
        {
            _subToConfig?.Dispose();
            _subToConfig = null;
            Clear();
        }
    }
}