using System;
using Mono.Cecil;

namespace CodeInjection
{
    public class TypeDefinitionWrapper
    {
        private readonly TypeDefinition _typeDefinition;

        public string FullName => _typeDefinition.FullName;

        public TypeDefinitionWrapper(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
        }

        public object GetAttributeParameters(Type targetAttribute, int index)
        {
            foreach (var customAttribute in _typeDefinition.CustomAttributes)
            {
                if (customAttribute.AttributeType.FullName == targetAttribute.FullName)
                {
                    if (customAttribute.ConstructorArguments.Count < index)
                    {
                        throw new Exception($"Attribute {targetAttribute.FullName} doesn't have parameter by index {index}");
                    }

                    return customAttribute.ConstructorArguments[index].Value;
                }
            }
            throw new Exception($"Attribute {targetAttribute.FullName} doesn't belong type {_typeDefinition.FullName}");
        }

        public void InjectArray(string type, string name)
        {
            _typeDefinition.Fields.Add(new FieldDefinition(name, FieldAttributes.Private, 
                new ArrayType(_typeDefinition.Module.ImportReference(InjectionCache.GetType(type)))));
        }
    }
}