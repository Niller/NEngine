using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeInjection.Experimental
{
    public class MethodState
    {
        private int _freeStackPosition;

        private MethodVariable[] _variables;

        private int _instructionIndex;

        public MethodState AddVariable(string name, Type t)
        {
            throw new NotImplementedException();
        }

        public MethodState AddVariableSet(string name, MethodValue value)
        {
            throw new NotImplementedException();
        }

        public MethodState AddVariableSet(string name, int value)
        {
            throw new NotImplementedException();
        }

        public MethodState Call(Method method, params MethodValue[] parameters)
        {
            throw new NotImplementedException();
        }

        public MethodState Call(Method method, MethodValue returnValue = null, params MethodValue[] parameters)
        {
            throw new NotImplementedException();
        }

        public MethodState If(MethodValue condition)
        {
            throw new NotImplementedException();
        }

        public MethodState EndIf(MethodValue condition)
        {
            throw new NotImplementedException();
        }
    }
}
