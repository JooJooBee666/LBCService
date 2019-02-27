using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace LBCServiceSettings
{
    class XMLConfigMethods
    {
        private static string XMLPath = AppDomain.CurrentDomain.BaseDirectory + "LCBServiceConfig.xml";

        public class ConfigData
        {
            public string Keyboard_Core_Path { get; set; }
            public int Light_Level { get; set; }
            public int Timeout_Preference { get; set; }
            public bool EnableDebugLog { get; set; }
        }

        /// <summary>
        /// Create new Config XML with default values if not present 
        /// </summary>
        public static bool SaveConfigXML(string KBCorePath, int LightLevel, int TimeoutPreference, bool EnableDebugMode)
        {
            try
            {
                var configXML = new XDocument(
                    new XElement("configuration",
                        new XElement("Keyboard_Core_Path",
                            @"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll"),
                        new XElement("Light_Level", LightLevel),
                        new XElement("Timeout_Preference", TimeoutPreference),
                        new XElement("Enable_Debug_Log", EnableDebugMode)
                    )
                );
                configXML.Save(XMLPath);
                return true;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("LenovoBacklightControl", $"Error creating config XML: {e.Message}", EventLogEntryType.Error, 50915);
                return false;
            }
        }

        /// <summary>
        /// Read Config XML data
        /// </summary>
        public static ConfigData ReadConfigXML()
        {
            //
            // set default config data values
            //
            var configData = new ConfigData
            {
                Keyboard_Core_Path =
                    @"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll",
                Light_Level = 2
            };

            //
            // If config file is not found, create one
            //
            if (!System.IO.File.Exists(XMLPath))
            {
                if (!SaveConfigXML(@"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll", 2, 300, false))
                {
                    // if creation fails, return default values
                    return configData;
                }
            }

            var xmlConfigDocument = new XmlDocument();

            try
            {
                xmlConfigDocument.Load(XMLPath);
                foreach (XmlElement xmlElement in xmlConfigDocument.DocumentElement)
                {
                    switch (xmlElement.Name)
                    {
                        case "Keyboard_Core_Path":
                            configData.Keyboard_Core_Path = xmlElement.InnerText;
                            break;
                        case "Light_Level":
                            configData.Light_Level = int.Parse(xmlElement.InnerText);
                            break;
                        case "Timeout_Preference":
                            configData.Timeout_Preference = int.Parse(xmlElement.InnerText);
                            break;
                        case "Enable_Debug_Log":
                            configData.EnableDebugLog = bool.Parse(xmlElement.InnerText);
                            SettingsForm.EnableDebugLogging = true;
                            break;
                    }
                }

                return configData;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("LenovoBacklightControl", $"Error reading config XML: {e.Message}", EventLogEntryType.Error, 50915);
                return configData;
            }
        }
    }
}

