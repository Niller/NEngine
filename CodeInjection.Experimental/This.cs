using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class This : MethodValue
    {
        public This(Type type) : base("this", 0, type)
        {
        }

        internal override IEnumerable<Instruction> ToStack()
        {
            yield return Instruction.Create(OpCodes.Ldarg_0);
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            throw new NotSupportedException();
        }
    }
}