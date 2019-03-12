using System;

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

        internal int ToStack()
        {
            throw new NotImplementedException();
        }

        internal int FromStack()
        {
            throw new NotImplementedException();
        }
    }
}