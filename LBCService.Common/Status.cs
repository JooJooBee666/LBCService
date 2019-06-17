using System;

namespace LBCService.Common
{
    public enum Status
    {
        BackLightWasEnabledByPower,
        Close,
        EnableBacklight,
        DisableBacklight,
        UpdateConfig,
        BacklightStateOff,
        BacklightStateLow,
        BacklightStateHigh,
        RequestBacklightState
    }

    public static class StatusEx
    {
        private const string PowerKey = "LBCSettings-BackLightWasEnabledByPower";
        private const string CloseKey = "CLOSEPIPE";
        private const string EnableKey = "LBC-EnableBacklight";
        private const string DisableKey = "LBC-DisableBacklight";
        private const string UpdateConfigKey = "LBC-UpdateConfigData";
        private const string BacklightStateOffKey = "LBC-BacklightStateOff";
        private const string BacklightStateLowKey = "LBC-BacklightStateLow";
        private const string BacklightStateHighKey = "LBC-BacklightStateHigh";
        private const string RequestBacklightStateKey = "LBC-RequestBacklightState";


        public static Status ParseStatus(this string s)
        {
            switch (s.Trim())
            {
                case PowerKey:
                    return Status.BackLightWasEnabledByPower;
                case CloseKey:
                    return Status.Close;
                case EnableKey:
                    return Status.EnableBacklight;
                case DisableKey:
                    return Status.DisableBacklight;
                case UpdateConfigKey:
                    return Status.UpdateConfig;
                case BacklightStateOffKey:
                    return Status.BacklightStateOff;
                case BacklightStateLowKey:
                    return Status.BacklightStateLow;
                case BacklightStateHighKey:
                    return Status.BacklightStateHigh;
                case RequestBacklightStateKey:
                    return Status.RequestBacklightState;
                default:
                    throw new NotSupportedException(s);
            }
        }

        public static string ConvertToString(this Status status)
        {
            switch (status)
            {
                case Status.BackLightWasEnabledByPower:
                    return PowerKey;
                case Status.Close:
                    return CloseKey;
                case Status.EnableBacklight:
                    return EnableKey;
                case Status.DisableBacklight:
                    return DisableKey;
                case Status.UpdateConfig:
                    return UpdateConfigKey;
                case Status.BacklightStateOff:
                    return BacklightStateOffKey;
                case Status.BacklightStateLow:
                    return BacklightStateLowKey;
                case Status.BacklightStateHigh:
                    return BacklightStateHighKey;
                case Status.RequestBacklightState:
                    return RequestBacklightStateKey;
                default:
                    throw new NotSupportedException(status.ToString());
            }
        }
    }
}