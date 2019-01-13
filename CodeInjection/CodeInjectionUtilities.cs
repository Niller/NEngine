using System;
using Mono.Cecil.Cil;

namespace CodeInjection
{
    public static class CodeInjectionUtilities
    {
        public static void Inject(ILProcessor il, Instruction instruction, Instruction milestone, InjectLineOrder orderType)
        {
            switch (orderType)
            {
                case InjectLineOrder.After:
                    il.InsertAfter(milestone, instruction);
                    break;
                case InjectLineOrder.Before:
                    il.InsertBefore(milestone, instruction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null);
            }
        }
    }
}