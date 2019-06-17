using System;
using TinyMessenger;

namespace LBCServiceSettings
{
    public abstract class CommonHook : IDisposable
    {
        protected readonly ITinyMessengerHub Hub;
        private readonly object _lockObject = new object();
        private bool _isHooked;

        protected CommonHook(ITinyMessengerHub hub)
        {
            Hub = hub;
        }

        public void EnableHook()
        {
            lock (_lockObject)
            {
                if (_isHooked) return;
                EnableHookInternal();
                _isHooked = true;
            }
        }

        public void DisableHook()
        {
            lock (_lockObject)
            {
                if (_isHooked)
                {
                    DisableHookInternal();
                    _isHooked = false;
                }
            }
        }

        protected abstract void EnableHookInternal();

        protected abstract void DisableHookInternal();

        public abstract void Dispose();
    }
}