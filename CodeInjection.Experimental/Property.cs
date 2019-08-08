using System.Linq;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Property
    {
        private readonly PropertyDefinition _definition;
        private readonly PropertyReference _reference;

        public Property(PropertyReference reference)
        {
            _definition = reference.Resolve();
            _reference = reference;
        }

        public PropertyDefinition GetDefinition()
        {
            return _definition;
        }

        public PropertyReference GetReference()
        {
            return _reference;
        }

        public bool HasAttribute(Type attributeType)
        {
            return _definition.CustomAttributes.Any(attr => attr.AttributeType.FullName == attributeType.FullName);
        }

        public Method GetPropertySetMethod()
        {
            return _definition.SetMethod?.ToWrapper();
        }
    }
}