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