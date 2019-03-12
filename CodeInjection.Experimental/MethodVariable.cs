using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodVariable : MethodValue
    {
        private VariableDefinition _variable;

        public MethodVariable(string name, VariableDefinition variable) : base(name, 0, variable.VariableType.ToWrapper())
        {
            _variable = variable;
        }
    }
}