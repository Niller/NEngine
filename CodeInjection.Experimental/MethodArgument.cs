using System;
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

        internal override Instruction ToStack()
        {
            return Instruction.Create(OpCodes.Ldarg, _parameterDefinition);
        }

        internal override Instruction FromStack()
        {
            throw new NotSupportedException();
        }
    }
}