using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;

namespace LBCService
{
    internal class XMLConfigMethods
    {
        private static readonly string XMLPath = AppDomain.CurrentDomain.BaseDirectory + "LCBServiceConfig.xml";

        /// <summary>
        ///    Our config data structure/class we use to easily pass things along
        /// </summary>
        public class ConfigData
        {
            public string Keyboard_Core_Path { get; set; }
            public int Light_Level { get; set; }
            public int Timeout_Preference { get; set; }
            public bool EnableDebugLog { get; set; }
        }

        /// <summary>
        ///    Create new Config XML with default values if not present 
        /// </summary>
        public static bool SaveConfigXML(string KBCorePath, int LightLevel, int TimeoutPreference, bool EnableDebugMode)
        {
            try
            {
                var configXML = new XDocument(
                    new XElement("configuration",
                        new XElement("Keyboard_Core_Path", KBCorePath),
                        new XElement("Light_Level", LightLevel),
                        new XElement("Timeout_Preference", TimeoutPreference),
                        new XElement("Enable_Debug_Log", EnableDebugMode)
                    )
                );
                configXML.Save(XMLPath);
                EnableFullControl(XMLPath);
                return true;
            }
            catch (Exception e)
            {
                var error = $"Error creating config XML: {e.Message}";
                EventLog.WriteEntry("LenovoBacklightControl", error ,EventLogEntryType.Error, 50915);
                LenovoBacklightControl.WriteToDebugLog(error);
                return false;
            }
        }

        /// <summary>
        ///    Read Config XML data
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
            if (!File.Exists(XMLPath))
            {
                if (!SaveConfigXML(@"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll",2,300,false))
                {
                    // if creation fails, return default values
                    return configData;
                }
            }

            var xmlConfigDocument = new XmlDocument();
            var timeoutPreferenceFound = false;
            var enableDebugLog = false;
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
                            timeoutPreferenceFound = true;
                            break;
                        case "Enable_Debug_Log":
                            configData.EnableDebugLog = bool.Parse(xmlElement.InnerText);
                            enableDebugLog = true;
                            LenovoBacklightControl.EnableDebugLog = true;
                            LenovoBacklightControl.WriteToDebugLog("Debug Logging Enabled.");
                            break;
                    }
                }
                if (!timeoutPreferenceFound)
                {
                    SaveConfigXML(configData.Keyboard_Core_Path, configData.Light_Level, 300, enableDebugLog);
                }
                return configData;
            }
            catch (Exception e)
            {
                var error = $"Error reading config XML: {e.Message}";
                EventLog.WriteEntry("LenovoBacklightControl", error, EventLogEntryType.Error, 50915);
                LenovoBacklightControl.WriteToDebugLog(error);
                return configData;
            }
        }

        public static void EnableFullControl(string fileName)
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
    }
}
