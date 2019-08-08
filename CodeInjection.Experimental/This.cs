using System;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class This : MethodValue
    {
        public This(Type type) : base("this", 0, type)
        {
        }

        internal override Instruction ToStack()
        {
            return Instruction.Create(OpCodes.Ldarg, 0);
        }

        internal override Instruction FromStack()
        {
            throw new NotSupportedException();
        }
    }
}