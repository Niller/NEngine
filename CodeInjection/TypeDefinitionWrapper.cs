using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

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

        public FieldDefinitionWrapper InjectArray(string type, string name)
        {
            var fieldDefinition = new FieldDefinition(name, FieldAttributes.Public,
                new ArrayType(_typeDefinition.Module.ImportReference(InjectionCache.GetType(type))));
            _typeDefinition.Fields.Add(fieldDefinition);
            return new FieldDefinitionWrapper(fieldDefinition);
        }

        public MethodDefinitionWrapper AddConstructor(params Type[] argumentTypes)
        {
            if (GetConstructor(argumentTypes) != null)
            {
                throw new Exception("Constructor with such signature already exists");
            }

            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", methodAttributes, _typeDefinition.Module.TypeSystem.Void);

            foreach (var parameterDefinition in argumentTypes.Select(t => new ParameterDefinition("args",
                ParameterAttributes.None, _typeDefinition.Module.ImportReference(t))))
            {
                method.Parameters.Add(parameterDefinition);
            }

            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, _typeDefinition.Module.ImportReference(typeof(object).GetConstructor(new Type[0]))));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            _typeDefinition.Methods.Add(method);
            return new MethodDefinitionWrapper(method);
        }

        public MethodDefinitionWrapper GetConstructor(params Type[] argumentTypes)
        {  
            foreach (var constructor in _typeDefinition.GetConstructors())
            {
                var index = 0;
                var isMatch = true;
                foreach (var constructorParameter in constructor.Parameters)
                {
                    if (constructorParameter.ParameterType.FullName != argumentTypes[index++].FullName)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return new MethodDefinitionWrapper(constructor);
                }
            }

            return null;
        }
    }
}