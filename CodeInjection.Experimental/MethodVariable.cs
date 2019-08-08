using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodVariable : MethodValue
    {
        private readonly VariableDefinition _variable;

        public MethodVariable(string name, int index, VariableDefinition variable) : base(name, index, variable.VariableType.ToWrapper())
        {
            _variable = variable;
        }

        internal override IEnumerable<Instruction> ToStack()
        {
            yield return Instruction.Create(OpCodes.Ldloc, _variable);
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            yield return Instruction.Create(OpCodes.Stloc, _variable);
        }
    }
}