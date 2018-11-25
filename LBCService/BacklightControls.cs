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
            //
            // TODO: add config file to make Keyboard_Core user defineable
            //
            var keyboardCoreDLL =
                @"C:\ProgramData\Lenovo\ImController\Plugins\ThinkKeyboardPlugin\x86\Keyboard_Core.dll";
            if (System.IO.File.Exists(keyboardCoreDLL))
            {
                Keyboard_Core =
                    Assembly.LoadFile(keyboardCoreDLL);
            }
            else
            {
                EventLog.WriteEntry("LenovoBacklightControl", "Unable to load Keyboard_Core.dll.  Service will stop.", EventLogEntryType.Error, 50903);
                LenovoBacklightControl.LBCServiceBase.Stop();
                return;
            }
            AssemblyType = Keyboard_Core.GetType("Keyboard_Core.KeyboardControl");
            KCInstance = Activator.CreateInstance(AssemblyType);

            //get internal method info for changing the KB Backlight status
            var setKeyboardBackLightStatusInfo = GetRuntimeMethodsExt(AssemblyType, "SetKeyboardBackLightStatus");

            //
            //TODO: add config file to make lightLevel (light level) user defineable
            //
            // 0 = off
            // 1 = low
            // 2 = high
            //
            object[] lightLevel = { lightValue };
            var output = (UInt32)setKeyboardBackLightStatusInfo.Invoke(KCInstance, lightLevel);
            IdleTimerControl.BackLightOn = lightValue != 0;
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
