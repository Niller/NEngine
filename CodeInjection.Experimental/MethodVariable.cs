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

        internal override Instruction ToStack()
        {
            return Instruction.Create(OpCodes.Ldloc, _variable);
        }

        internal override Instruction FromStack()
        {
            return Instruction.Create(OpCodes.Stloc, _variable);
        }
    }
}