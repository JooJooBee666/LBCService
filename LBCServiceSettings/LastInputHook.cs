using System.Threading;
using LBCServiceSettings.Messages;
using TinyMessenger;

namespace LBCServiceSettings
{
    /// <summary>
    /// Checks for last user activity every <see cref="Period" /> ms and reports when it detects activity.
    /// </summary>
    public class LastInputHook : CommonHook
    {
        private const int Period = 300;
        private readonly ITinyMessengerHub _hub;
        private Timer _timer;

        private void Callback(object state)
        {
            var inactiveTime = Win32.GetLastInputTime();
            if (Period > inactiveTime) // Check whether user was active since last timer tick.
            {
                _hub.PublishAsync(new UserActiveMessage(this));
            }
        }

        public LastInputHook(ITinyMessengerHub hub)
        {
            _hub = hub;
        }

        protected override void EnableHookInternal()
        {
            _timer = new Timer(Callback);
            _timer.Change(Period, Period);
        }

        protected override void DisableHookInternal()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }
    }
}