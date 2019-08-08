using System;

namespace ECS.Experimental
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        public Type Context;

        public ComponentAttribute(Type contextType)
        {
            Context = contextType;
        }
    }
}