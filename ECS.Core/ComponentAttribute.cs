using System;
using System.Management.Instrumentation;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        public string Context;

        public ComponentAttribute(string context)
        {
            Context = context;
        }
    }
}
