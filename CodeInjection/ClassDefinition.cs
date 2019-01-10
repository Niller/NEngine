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

        public void InjectArray(Type type, string name)
        {
            TypeReference typeReference;
            if (type.FullName == null || !InjectionCache.Types.TryGetValue(type.FullName, out typeReference))
            {
                throw new Exception($"Cannot inject array because of type with name {type.FullName} not registered");
            }
            _typeDefinition.Fields.Add(new FieldDefinition(name, FieldAttributes.Private, new ArrayType(typeReference)));
        }
    }
}