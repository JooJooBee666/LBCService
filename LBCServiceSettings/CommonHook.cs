using System;

namespace LBCServiceSettings
{
    public abstract class CommonHook : IDisposable
    {
        private readonly object _lockObject = new object();
        private bool _isHooked;

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