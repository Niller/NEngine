using System;

namespace CodeInjection.Experimental
{
    public class Method
    {
        public enum DefaultStates
        {
            MethodStart,
            MethodEnd,
        }

        public string Name
        {
            get;
        }

        public Type ReturnType
        {
            get;
        }

        public ParameterType[] ParameterTypes
        {
            get;
        }

        public MethodState GetState(DefaultStates state)
        {
            throw new NotImplementedException();
        }
    }
}