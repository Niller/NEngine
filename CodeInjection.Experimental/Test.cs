using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInjection.Experimental
{
    class Test
    {
        public static void TestMethod()
        {
            Assembly assembly = null;
            var type1 = assembly.GetType("TestClass1");
            var type2 = assembly.GetType("TestClass2");
            var type3 = assembly.GetType("TestClass3");

            type1.AddField("field1", type2);
            type1.AddField("field2", type3);

            var method1 = type1.AddMethod("method1", type2, new ParameterType(type1), new ParameterType(type3));

            var method1State = method1.GetState(Method.DefaultStates.MethodStart);

            //method1State.AddVariable("v1", type3).
            
        }
    }
}
