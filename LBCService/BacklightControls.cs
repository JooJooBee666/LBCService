using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LBCService
{
    public class BacklightControls
    {
        public void ActivateBacklight(int lightValue)
        {
            if (System.IO.File.Exists(LenovoBacklightControl.KBCorePath))
            {
                try
                {
                    Keyboard_Core = Assembly.LoadFile(LenovoBacklightControl.KBCorePath);
                }
                catch (Exception e)
                {
                    var error = $"Unable to load Keyboard_Core.dll.  Service will stop.  Error: {e.Message}";
                    EventLog.WriteEntry("LenovoBacklightControl", error, EventLogEntryType.Error, 50903);
                    LenovoBacklightControl.WriteToDebugLog(error);
                    LenovoBacklightControl.LBCServiceBase.Stop();
                    return;
                }

            }
            else
            {
                const string error = "Unable to load Keyboard_Core.dll. Service will stop.";
                EventLog.WriteEntry("LenovoBacklightControl", error, EventLogEntryType.Error, 50903);
                LenovoBacklightControl.WriteToDebugLog(error);
                LenovoBacklightControl.LBCServiceBase.Stop();
                return;
            }
            AssemblyType = Keyboard_Core.GetType("Keyboard_Core.KeyboardControl");
            KCInstance = Activator.CreateInstance(AssemblyType);

            //get internal method info for changing the KB Backlight status
            var setKeyboardBackLightStatusInfo = GetRuntimeMethodsExt(AssemblyType, "SetKeyboardBackLightStatus");

            object[] lightLevel = { lightValue };
            var output = (UInt32)setKeyboardBackLightStatusInfo.Invoke(KCInstance, lightLevel);
        }

        private Assembly Keyboard_Core;
        private Type AssemblyType;
        private object KCInstance;

        private MethodInfo GetRuntimeMethodsExt(Type type, string name, params Type[] types)
        {
            // https://stackoverflow.com/questions/21307845/runtimereflectionextensions-getruntimemethod-does-not-work-as-expected
            // Find potential methods with the correct name and the right number of parameters
            // and parameter names
            var potentials = (from ele in type.GetMethods() where ele.Name.Equals(name) select ele);

            return potentials.FirstOrDefault();
        }
    }
}
