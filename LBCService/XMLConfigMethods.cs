using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LBCService
{
    class XMLConfigMethods
    {
        private static string XMLPath = AppDomain.CurrentDomain.BaseDirectory + "LCBServiceConfig.xml";

        public class ConfigData
        {
            public string Keyboard_Core_Path { get; set; }
            public int Light_Level { get; set; }
            public int Timeout_Preference { get; set; }
        }

        /// <summary>
        /// Create new Config XML with default values if not present 
        /// </summary>
        public static bool CreateConfigXML(string KBCorePath, int LightLevel, int TimeoutPreference)
        {
            try
            {
                var configXML = new XDocument(
                    new XElement("configuration",
                        new XElement("Keyboard_Core_Path",
                            @"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll"),
                        new XElement("Light_Level", "2"), 
                        new XElement("Timeout_Preference","300")
                    )
                );
                configXML.Save(XMLPath);
                return true;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("LenovoBacklightControl", $"Error creating config XML: {e.Message}",EventLogEntryType.Error, 50915);
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
                if (!CreateConfigXML(@"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll",2,300))
                {
                    // if creation fails, return default values
                    return configData;
                }
            }

            var xmlConfigDocument = new XmlDocument();
            var TimeoutPreferenceFound = false;
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
                            TimeoutPreferenceFound = true;
                            break;
                    }
                }
                if (!TimeoutPreferenceFound)
                {
                    CreateConfigXML(configData.Keyboard_Core_Path, configData.Light_Level, 300);
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
