using System;
using System.Timers;
using LBCService.Common;
using LBCService.Common.Messages;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    public class IdleTimerControl : IDisposable
    {
        private readonly ITinyMessengerHub _hub;
        private readonly KeyboardHookClass _keyboardHook;
        private readonly MouseHookClass _mouseHook;
        private readonly LastInputHook _inputHook;
        private Timer _idleTimer;

        private bool _backLightOn;
        private TinyMessageSubscriptionToken _subToStatus;
        private TinyMessageSubscriptionToken _subToActivity;

        public IdleTimerControl(ITinyMessengerHub hub, KeyboardHookClass keyboardHook, MouseHookClass mouseHook, LastInputHook inputHook)
        {
            _hub = hub;
            _keyboardHook = keyboardHook;
            _mouseHook = mouseHook;
            _inputHook = inputHook;
            _subToStatus = _hub.Subscribe<SendStatusRequestMessage>(OnSendStatus);
            _subToActivity = _hub.Subscribe<UserActiveMessage>(_ => RestartTimer());
        }

        /// <summary>
        /// Track backlight status.
        /// </summary>
        private void OnSendStatus(SendStatusRequestMessage message)
        {
            switch (message.Status)
            {
                case Status.EnableBacklight:
                    _backLightOn = true;
                    break;
                case Status.DisableBacklight:
                    _backLightOn = false;
                    break;
            }
        }

        /// <summary>
        /// Restarts the Idletimer and resets the interval (in case it has changed).
        /// </summary>
        private void RestartTimer()
        {
            if (_idleTimer == null) return;
            _idleTimer.Stop();
            _idleTimer.Start();

            //enable backlight if it was off
            if (!_backLightOn)
            {
                // Send the message to enable the backlight using a thread
                _hub.PublishAsync(new SendStatusRequestMessage(this, Status.EnableBacklight));
            }
        }

        public void SetTimer(int timeOut)
        {
            if (_idleTimer != null)
            {
                _idleTimer.Stop();
                _idleTimer.Dispose();
                _idleTimer = null;
            }
            if (timeOut < 1) return;

            // Send the message to enable the backlight using a thread
            _hub.PublishAsync(new SendStatusRequestMessage(this, Status.EnableBacklight));

            //
            // Enable mouse and Keyboard hooks. We don't actually look to what was typed 
            // or where the mouse, we just fire an event if there was any activity at all
            //
            //_keyboardHook.EnableHook();
            //_mouseHook.EnableHook();
            _inputHook.EnableHook();

            // Create a timer
            _idleTimer = new Timer(timeOut * 1000);
            // Hook up the Elapsed event for the timer. 
            _idleTimer.Elapsed += TimeoutReached;
            _idleTimer.AutoReset = true;
            _idleTimer.Start();
        }

        /// <summary>
        /// Stop timer and disable user activity tracking.
        /// </summary>
        public void StopTimer()
        {
            if (_idleTimer != null)
            {
                _idleTimer.Stop();
                _idleTimer.Dispose();
                _idleTimer = null;
            }
            //_keyboardHook.DisableHook();
            //_mouseHook.DisableHook();
            _inputHook.DisableHook();
        }

        /// <summary>
        /// Check the idle time and disable backlight if passed
        /// Re-enables if timer gets reset (i.e. user activity)
        /// </summary>
        private void TimeoutReached(object source, ElapsedEventArgs e)
        {
            if (_backLightOn)
            {
                // Send the message to disable the backlight using a thread
                _hub.PublishAsync(new SendStatusRequestMessage(this, Status.DisableBacklight));
            }
        }

        public void Dispose()
        {
            StopTimer();

            _subToStatus?.Dispose();
            _subToStatus = null;
            _subToActivity?.Dispose();
            _subToActivity = null;

            _keyboardHook?.Dispose();
            _mouseHook?.Dispose();
            _inputHook?.Dispose();
        }
    }
}
