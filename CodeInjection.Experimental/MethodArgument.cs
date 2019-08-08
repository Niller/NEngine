using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodArgument : MethodValue
    {
        private readonly ParameterDefinition _parameterDefinition;

        public MethodArgument(ParameterDefinition parameterDefinition) : base(parameterDefinition.Name, parameterDefinition.Index, parameterDefinition.ParameterType.ToWrapper())
        {
            _parameterDefinition = parameterDefinition;
        }

        internal override IEnumerable<Instruction> ToStack()
        {
            yield return Instruction.Create(OpCodes.Ldarg, _parameterDefinition);
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            throw new NotSupportedException();
        }
    }
}