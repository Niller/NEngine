using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class ConstantIntValue : MethodValue
    {
        private readonly int _value;

        public ConstantIntValue(int value) : base(string.Empty, -1, null)
        {
            _value = value;
        }

        internal override IEnumerable<Instruction> ToStack()
        {
            yield return Instruction.Create(OpCodes.Stloc, _value);
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            throw new NotSupportedException();
        }
    }
}