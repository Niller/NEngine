using System;
using System.Linq;
using Logger;
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

        private readonly MethodDefinition _definition;
        private MethodReference _reference;

        public string Name => _definition.Name;

        public Type ReturnType
        {
            get;
        }

        public ParameterType[] ParameterTypes
        {
            get;
        }

        public Method(MethodReference reference)
        {
            _definition = reference.Resolve();
            _reference = reference;
            ReturnType = _definition.ReturnType.ToWrapper();

            ParameterTypes = new ParameterType[_definition.Parameters.Count];
            var i = 0;
            foreach (var parameter in _definition.Parameters)
            {
                ParameterTypes[i++] = new ParameterType(parameter.Name, parameter.ParameterType.ToWrapper(),
                    parameter.Name[0] == '&', parameter.Attributes);
            }
        }

        public MethodDefinition GetDefinition()
        {
            return _definition;
        }

        public MethodReference GetReference()
        {
            return _reference;
        }

        public Instruction Call()
        {
            return Instruction.Create(_definition.DeclaringType.IsValueType ? OpCodes.Call : OpCodes.Callvirt, _reference);
        }

        public MethodState GetState(DefaultStates state)
        {
            switch (state)
            {
                case DefaultStates.MethodStart:
                    return new MethodState(_definition, _definition.Body.Instructions.First());
                case DefaultStates.MethodEnd:
                    //Before RET
                    return new MethodState(_definition, _definition.Body.Instructions[_definition.Body.Instructions.Count - 1]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}