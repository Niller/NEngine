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

        public FieldDefinitionWrapper InjectArrayField(string type, string name)
        {
            var fieldDefinition = new FieldDefinition(name, FieldAttributes.Public,
                new ArrayType(_typeDefinition.Module.ImportReference(InjectionCache.GetType(type))));
            _typeDefinition.Fields.Add(fieldDefinition);
            return new FieldDefinitionWrapper(fieldDefinition);
        }

        public MethodDefinitionWrapper InjectOverrideMethod(MethodDefinitionWrapper baseMethod, bool callBase)
        {
            if (!baseMethod.GetDefinition().IsVirtual && !baseMethod.GetDefinition().IsAbstract)
            {
                throw new Exception(
                    $"Cannot override method {baseMethod.GetDefinition().Name} because it's not abstract or virtual");
            }

            var methodAttributes = (baseMethod.GetDefinition().Attributes & ~MethodAttributes.NewSlot) | MethodAttributes.ReuseSlot;
            var method = new MethodDefinition(baseMethod.GetDefinition().Name, methodAttributes, baseMethod.GetDefinition().ReturnType);

            foreach (var parameterDefinition in baseMethod.GetDefinition().Parameters)
            {
                method.Parameters.Add(parameterDefinition);
            }

            if (callBase)
            {
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));

                int index = 0;
                foreach (var parameterDefinition in baseMethod.GetDefinition().Parameters)
                {
                    switch (index)
                    {
                        case 0:
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                            break;
                        case 1:
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                            break;
                        case 2:
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                            break;
                        default:
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_S, parameterDefinition));
                            break;
                    }
                }

                method.Body.Instructions.Add(Instruction.Create(OpCodes.Call,
                    _typeDefinition.Module.ImportReference(baseMethod.GetDefinition())));

            }

            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            _typeDefinition.Methods.Add(method);

            return new MethodDefinitionWrapper(method);
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
            return GetMethod(".ctor", argumentTypes);
        }

        public MethodDefinitionWrapper GetMethod(string name, params Type[] argumentTypes)
        {
            foreach (var method in _typeDefinition.Methods)
            {
                if (method.Name != name)
                {
                    continue;
                }

                var index = 0;
                var isMatch = true;

                if (method.Parameters.Count != argumentTypes.Length)
                {
                    continue;
                }

                foreach (var parameter in method.Parameters)
                {
                    if (parameter.ParameterType.FullName != argumentTypes[index++].FullName)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return new MethodDefinitionWrapper(method);
                }
            }

            return null;
        }
    }
}