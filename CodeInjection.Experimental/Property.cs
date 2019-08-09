using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class Property : MethodValue
    {
        private readonly PropertyDefinition _definition;
        private readonly PropertyReference _reference;
        private readonly MethodValue _source;

        public Property(PropertyReference reference, MethodValue source) : base(reference.Name, 0, reference.PropertyType.ToWrapper())
        {
            _definition = reference.Resolve();
            _reference = reference;
            _source = source;
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

        internal override IEnumerable<Instruction> ToStack()
        {
            foreach (var instruction in _source.ToStack())
            {
                yield return instruction;
            }

            yield return _definition.GetMethod.ToWrapper().Call();
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            foreach (var instruction in _source.ToStack())
            {
                yield return instruction;
            }

            yield return _definition.SetMethod.ToWrapper().Call();
        }
    }
}