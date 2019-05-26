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
    }

    public static class StatusEx
    {
        private const string PowerKey = "LBCSettings-BackLightWasEnabledByPower";
        private const string CloseKey = "CLOSEPIPE";
        private const string EnableKey = "LBC-EnableBacklight";
        private const string DisableKey = "LBC-DisableBacklight";
        private const string UpdateConfigKey = "LBC-UpdateConfigData";


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
                default:
                    throw new NotSupportedException(status.ToString());
            }
        }
    }
}