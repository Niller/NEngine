using System;
using Mono.Cecil;

namespace CodeInjection
{
    public class ClassDefinition
    {
        private readonly TypeDefinition _typeDefinition;

        public ClassDefinition(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
        }

        public void InjectArray(Type type, string name, FieldAttributes attributes)
        {
            _typeDefinition.Fields.Add(new FieldDefinition(name, attributes, new ArrayType(InjectionCache.GetType(type.FullName))));
        }
    }
}