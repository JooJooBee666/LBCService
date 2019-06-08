using System.ServiceProcess;

namespace LBCService
{
    public abstract class CustomServiceBase : ServiceBase
    {
        private readonly object _lockObj = new object();
        private bool _isRunning;

        protected CustomServiceBase()
        {
            AutoLog = true;
            CanStop = true;
            CanShutdown = true;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = true;
            CanHandleSessionChangeEvent = true;
        }

        #region API

        protected abstract void ServiceStart();

        protected abstract void ServiceStop();

        protected abstract void ServicePause();

        private void ServiceStartInternal()
        {
            lock (_lockObj)
            {
                if (_isRunning) return;
                ServiceStart();
                _isRunning = true;
            }
        }

        private void ServiceStopInternal()
        {
            lock (_lockObj)
            {
                if (!_isRunning) return;
                ServiceStop();
                _isRunning = false;
            }
        }

        private void ServicePauseInternal()
        {
            lock (_lockObj)
            {
                if (!_isRunning) return;
                ServicePause();
                _isRunning = false;
            }
        }

        #endregion

        #region Overrides

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch (powerStatus)
            {
                case PowerBroadcastStatus.QuerySuspend:
                    ServicePauseInternal();
                    break;
                case PowerBroadcastStatus.ResumeCritical:
                    ServiceStartInternal();
                    break;
                case PowerBroadcastStatus.ResumeSuspend:
                    ServiceStartInternal();
                    break;
                case PowerBroadcastStatus.Suspend:
                    ServicePauseInternal();
                    break;
            }
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    ServiceStartInternal();
                    break;
                case SessionChangeReason.SessionLogoff:
                    ServicePauseInternal();
                    break;
                case SessionChangeReason.SessionLock:
                    ServicePauseInternal();
                    break;
                case SessionChangeReason.SessionUnlock:
                    ServiceStartInternal();
                    break;
            }

            base.OnSessionChange(changeDescription);
        }

        protected override void OnContinue()
        {
            ServiceStartInternal();
            base.OnContinue();
        }

        protected override void OnPause()
        {
            ServicePauseInternal();
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            ServiceStopInternal();
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            ServiceStartInternal();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ServiceStopInternal();
            base.OnStop();
        }

        #endregion
    }
}