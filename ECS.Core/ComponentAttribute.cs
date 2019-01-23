using System;
using System.Management.Instrumentation;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        public Type Context;

        public ComponentAttribute(Type context)
        {
            Context = context;
        }
    }
}
