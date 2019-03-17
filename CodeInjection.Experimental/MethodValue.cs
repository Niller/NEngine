using System;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public abstract class MethodValue
    {
        public string Name
        {
            get;
        }

        public Type Type
        {
            get;
        }

        public int Index
        {
            get;
        }

        protected MethodValue(string name, int index, Type type)
        {
            Name = name;
            Index = index;
            Type = type;
        }

        internal abstract Instruction ToStack();
        internal abstract Instruction FromStack();
    }
}