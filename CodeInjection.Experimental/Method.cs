using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class Method
    {
        public enum DefaultStates
        {
            MethodStart,
            MethodEnd,
        }

        private readonly MethodDefinition _method;

        public string Name => _method.Name;

        public Type ReturnType
        {
            get;
        }

        public ParameterType[] ParameterTypes
        {
            get;
        }

        public Method(MethodDefinition methodDefinition)
        {
            _method = methodDefinition;
            ReturnType = methodDefinition.ReturnType.ToWrapper();

            ParameterTypes = new ParameterType[methodDefinition.Parameters.Count];
            var i = 0;
            foreach (var parameter in methodDefinition.Parameters)
            {
                ParameterTypes[i++] = new ParameterType(parameter.Name, parameter.ParameterType.ToWrapper(),
                    parameter.Name[0] == '&', parameter.Attributes);
            }
        }

        public Instruction Call()
        {
            return Instruction.Create(_method.DeclaringType.IsClass ? OpCodes.Callvirt : OpCodes.Call, _method);
        }

        public MethodDefinition GetDefinition()
        {
            return _method;
        }

        public MethodState GetState(DefaultStates state)
        {
            switch (state)
            {
                case DefaultStates.MethodStart:
                    return new MethodState(_method, _method.Body.Instructions.First());
                case DefaultStates.MethodEnd:
                    //Before RET
                    return new MethodState(_method, _method.Body.Instructions[_method.Body.Instructions.Count - 2]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
    }
}