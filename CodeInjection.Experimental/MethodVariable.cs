using System;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodVariable : MethodValue
    {
        private VariableDefinition _variable;

        public MethodVariable(string name, int index, VariableDefinition variable) : base(name, index, variable.VariableType.ToWrapper())
        {
            _variable = variable;
        }

        internal override int ToStack()
        {


            throw new NotImplementedException();
        }

        internal override int FromStack()
        {
            throw new NotImplementedException();
        }
    }
}