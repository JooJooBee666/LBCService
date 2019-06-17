namespace LBCService.Common
{
    /// <summary>
    ///    Our config data structure/class we use to easily pass things along
    /// </summary>
    public struct ConfigData
    {
        private const string DefaultKeyboardCorePath =
            @"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll";

        public string KeyboardCorePath { get; set; }

        public LightLevel LightLevel { get; set; }

        public int TimeoutPreference { get; set; }

        public bool EnableDebugLog { get; set; }

        public bool SaveBacklightState { get; set; }

        public LightLevel SavedState { get; set; }

        public bool MonitorDisplayState { get; set; } 

        public static ConfigData GetDefault()
        {
            return new ConfigData
            {
                KeyboardCorePath = DefaultKeyboardCorePath,
                LightLevel = LightLevel.High,
                TimeoutPreference = 300,
                EnableDebugLog = false,
                SaveBacklightState = true,
                SavedState = LightLevel.High,
                MonitorDisplayState = false
            };
        }
    }
}