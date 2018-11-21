using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LBCService
{
    public class BacklightControls
    {
        public void ActivateBacklight()
        {
            Keyboard_Core = Assembly.LoadFile("C:\\ProgramData\\Lenovo\\ImController\\Plugins\\ThinkKeyboardPlugin\\x86\\Keyboard_Core.dll");
            AssemblyType = Keyboard_Core.GetType("Keyboard_Core.KeyboardControl");
            KCInstance = Activator.CreateInstance(AssemblyType);
            IEnumerable list = AssemblyType.GetMethods();

            //"c:\\Users\\All Users\\Lenovo\\ImController\\Plugins\\ThinkKeyboardPlugin\\x86\\Keyboard_Core.dll"

            //MethodInfo getKeyboardBackLightLevelInfo = myType.GetMethod("GetKeyboardBackLightLevel", new Type[] { typeof(Int32) });
            var getKeyboardBackLightLevelInfo = GetRuntimeMethodsExt(AssemblyType, "GetKeyboardBackLightLevel", new Type[] { });
            object[] outStatus = new object[1];
            //var getKeyboardBackLightStatus = GetRuntimeMethodsExt(AssemblyType, "GetKeyboardBackLightStatus", new Type[] { });
            var setKeyboardBackLightStatusInfo = GetRuntimeMethodsExt(AssemblyType, "SetKeyboardBackLightStatus", new Type[] { });

            //UInt32 output2 = (UInt32)getKeyboardBackLightStatus.Invoke(KCInstance, out2);
            object[] arguments = new object[] { 2 };
            UInt32 output = (UInt32)setKeyboardBackLightStatusInfo.Invoke(KCInstance, arguments);
        }

        private Assembly Keyboard_Core;
        private Type AssemblyType;
        private Object KCInstance;

        private MethodInfo GetRuntimeMethodsExt(Type type, string name, params Type[] types)
        {
            // https://stackoverflow.com/questions/21307845/runtimereflectionextensions-getruntimemethod-does-not-work-as-expected
            // Find potential methods with the correct name and the right number of parameters
            // and parameter names
            var potentials = (from ele in type.GetMethods()
                where ele.Name.Equals(name)
                //let param = ele.GetParameters()
                //where param.Length == types.Length
                //&& param.Select(p => p.ParameterType.Name).SequenceEqual(types.Select(t => t.Name))
                select ele);

            // Maybe check if we have more than 1? Or not?
            return potentials.FirstOrDefault();
        }
    }
}
