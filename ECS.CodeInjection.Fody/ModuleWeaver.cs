using System;
using System.Collections.Generic;
using System.Linq;
using CodeInjection.Experimental;
using Fody;
using Mono.Cecil;

namespace ECS.CodeInjection
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            var assembly = new Assembly(ModuleDefinition);
            //var type1 = assembly.AddType("Test.TestClass", TypeAttributes.Class);
            var type1 = assembly.GetType("NEngine.Editor.Editor");
            var intType = assembly.ImportType<int>();

            type1.AddField("field1", intType, FieldAttributes.Private);
            type1.AddField("field2", type1, FieldAttributes.Private);

            var method1 = type1.AddMethod("method1", MethodAttributes.Public, intType,
                new ParameterType("arg1", intType), new ParameterType("arg2", intType));

            var method1State = method1.GetState(Method.DefaultStates.MethodStart);

            method1State.AddVariable("v1", intType)
                .MathOperation(MathOperation.Add, method1State.GetArgument(0), method1State.GetArgument(1))
                .AddVariableSet("v1").ReturnValue(method1State.GetVariable("v1"));

            //assembly.Save();
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}