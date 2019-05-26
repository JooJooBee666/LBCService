using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;
using LBCService.Common.Messages;
using TinyMessenger;

namespace LBCService.Common
{
    public class Config : IConfig, IDisposable
    {
        private static readonly string XMLPath = AppDomain.CurrentDomain.BaseDirectory + "LCBServiceConfig.xml";
        private const string KeyboardCorePathKey = "Keyboard_Core_Path";
        private const string LightLevelKey = "Light_Level";
        private const string TimeoutPreferenceKey = "Timeout_Preference";
        private const string EnableDebugLogKey = "Enable_Debug_Log";
        private const string SaveBacklightStateKey = "Save_Backlight_State";
        private const string SavedStateKey = "Saved_State";
        private readonly ILogger _logger;
        private readonly ITinyMessengerHub _hub;
        private TinyMessageSubscriptionToken _subToReload;
        private TinyMessageSubscriptionToken _subToSave;
        
        public Config(ILogger logger, ITinyMessengerHub hub)
        {
            _logger = logger;
            _hub = hub;
            _subToReload = _hub.Subscribe<ConfigReloadRequestMessage>(_ => Load());
            _subToSave = _hub.Subscribe<ConfigSaveRequestMessage>(message => Save(message.Data));
        }

        public bool Save(ConfigData data)
        {
            try
            {
                var configXML = new XDocument(
                    new XElement("configuration",
                        new XElement(KeyboardCorePathKey, data.KeyboardCorePath),
                        new XElement(LightLevelKey, (int)data.LightLevel),
                        new XElement(TimeoutPreferenceKey, data.TimeoutPreference),
                        new XElement(EnableDebugLogKey, data.EnableDebugLog),
                        new XElement(SaveBacklightStateKey, data.SaveBacklightState),
                        new XElement(SavedStateKey, (int)data.SavedState)
                    )
                );
                configXML.Save(XMLPath);
                EnableFullControl(XMLPath);

                return true;
            }
            catch (Exception e)
            {
                var error = "Error creating config XML:";
                _logger.Error(e, error, 50915);
                return false;
            }
        }

        public ConfigData Load()
        {
            var configData = ConfigData.GetDefault();
            //
            // If config file is not found, return default data.
            //
            if (!File.Exists(XMLPath))
            {
                _hub.Publish(new OnConfigLoadedMessage(this, configData));
                return configData;
            }

            var xmlConfigDocument = new XmlDocument();
            try
            {
                xmlConfigDocument.Load(XMLPath);
                if (xmlConfigDocument.DocumentElement != null)
                {
                    foreach (XmlElement xmlElement in xmlConfigDocument.DocumentElement)
                    {
                        switch (xmlElement.Name)
                        {
                            case KeyboardCorePathKey:
                                configData.KeyboardCorePath = xmlElement.InnerText;
                                break;
                            case LightLevelKey:
                                configData.LightLevel = (LightLevel)int.Parse(xmlElement.InnerText);
                                break;
                            case TimeoutPreferenceKey:
                                configData.TimeoutPreference = int.Parse(xmlElement.InnerText);
                                break;
                            case EnableDebugLogKey:
                                configData.EnableDebugLog = bool.Parse(xmlElement.InnerText);
                                break;
                            case SaveBacklightStateKey:
                                configData.SaveBacklightState = bool.Parse(xmlElement.InnerText);
                                break;
                            case SavedStateKey:
                                configData.SavedState = (LightLevel)int.Parse(xmlElement.InnerText);
                                break;
                        }
                    }
                }
                _hub.Publish(new OnConfigLoadedMessage(this, configData));
                return configData;
            }
            catch (Exception e)
            {
                var error = "Error reading config XML:";
                _logger.Error(e, error, 50915);
                return configData;
            }
        }

        private void EnableFullControl(string fileName)
        {
            try
            {
                // Read the current ACL permission for the file
                var fileSecurity = File.GetAccessControl(fileName);

                // Create a new rule set based on "Everyone"
                var fileAccessRule = new FileSystemAccessRule(new NTAccount("", "Everyone"),
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                // Append the new rule set to the file
                fileSecurity.AddAccessRule(fileAccessRule);

                // Save it to the file system
                File.SetAccessControl(fileName, fileSecurity);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error setting access for '{fileName}'");
            }
        }

        public void Dispose()
        {
            _subToReload?.Dispose();
            _subToReload = null;

            _subToSave?.Dispose();
            _subToSave = null;
        }
    }
}