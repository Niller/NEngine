using System;

namespace CodeInjection.Experimental
{
    public class Type
    {
        public Field GetField(string name)
        {
            throw new NotImplementedException();
        }

        public Field AddField(string name, Type type)
        {
            throw new NotImplementedException();
        }

        public Method GetMethod(string name, params ParameterType[] parameters)
        {
            throw new NotImplementedException();
        }

        public Method AddMethod(string name, Type returnType, params ParameterType[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}