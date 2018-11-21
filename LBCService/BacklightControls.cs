using System;
using System.Linq;
using System.Reflection;

namespace LBCService
{
    public class BacklightControls
    {
        public void ActivateBacklight()
        {
            //
            // TODO: add config file to make Keyboard_Core user defineable
            //
            Keyboard_Core = Assembly.LoadFile("C:\\ProgramData\\Lenovo\\ImController\\Plugins\\ThinkKeyboardPlugin\\x86\\Keyboard_Core.dll");
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
            object[] lightLevel = { 2 };
            var output = (UInt32)setKeyboardBackLightStatusInfo.Invoke(KCInstance, lightLevel);
        }

        private Assembly Keyboard_Core;
        private Type AssemblyType;
        private Object KCInstance;

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
