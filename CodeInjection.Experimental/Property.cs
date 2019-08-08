using System.Linq;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Property
    {
        private readonly PropertyDefinition _property;

        public Property(PropertyDefinition property)
        {
            _property = property;
        }

        public bool HasAttribute(Type attributeType)
        {
            return _property.CustomAttributes.Any(attr => attr.AttributeType.FullName == attributeType.FullName);
        }

        public Method GetPropertySetMethod()
        {
            return _property.SetMethod?.ToWrapper();
        }
    }
}