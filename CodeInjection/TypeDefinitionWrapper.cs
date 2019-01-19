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

        public FieldDefinitionWrapper InjectField(string name, Type type)
        {
            var fieldDefinition = new FieldDefinition(name, FieldAttributes.Public,
                _typeDefinition.Module.ImportReference(type));
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

            var returnType = _typeDefinition.Module.ImportReference(baseMethod.GetDefinition().ReturnType);

            var methodAttributes = (baseMethod.GetDefinition().Attributes & ~MethodAttributes.NewSlot & ~MethodAttributes.Abstract) | MethodAttributes.ReuseSlot;
            var method = new MethodDefinition(baseMethod.GetDefinition().Name, methodAttributes, returnType);

            foreach (var parameterDefinition in baseMethod.GetDefinition().Parameters)
            {
                method.Parameters.Add(parameterDefinition);
            }

            foreach (var genericParameter in baseMethod.GetDefinition().GenericParameters)
            {
                method.GenericParameters.Add(genericParameter);
            }

            if (!baseMethod.GetDefinition().IsAbstract && callBase)
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

            if (method.ReturnType.Name != typeof(void).Name)
            {     
                method.Body.InitLocals = true;
                var tempVar1 = new VariableDefinition(returnType);
                var tempVar2 = new VariableDefinition(returnType);
                method.Body.Variables.Add(tempVar1);
                method.Body.Variables.Add(tempVar2);

                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloca_S, tempVar1));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Initobj, returnType));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc_1));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
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

        public void InjectHasEntityMethod(FieldDefinitionWrapper arrayField, TypeDefinitionWrapper componentTypeWrapper)
        {
            var module = _typeDefinition.Module;
            var intType = module.ImportReference(typeof(int));
            var boolType = module.ImportReference(typeof(bool));
            var componentType = module.ImportReference(componentTypeWrapper._typeDefinition);

            var methodAttributes = MethodAttributes.Public;
            var method = new MethodDefinition("HasEntity_" + componentType.Name, methodAttributes, boolType);

            method.Body.InitLocals = true;
            var tempVar1 = new VariableDefinition(intType);
            var tempVar2 = new VariableDefinition(boolType);
            var tempVar3 = new VariableDefinition(boolType);
            var tempVar4 = new VariableDefinition(boolType);
            method.Body.Variables.Add(tempVar1);
            method.Body.Variables.Add(tempVar2);
            method.Body.Variables.Add(tempVar3);
            method.Body.Variables.Add(tempVar4);

            var il = method.Body.GetILProcessor();

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_0);
            var label1 = il.Body.Instructions.Last();

            il.Emit(OpCodes.Ldarg_0);
            var label6 = il.Body.Instructions.Last();
            il.Emit(OpCodes.Ldfld, arrayField.GetDefinition());
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldelema, componentType);
            il.Emit(OpCodes.Ldfld, _typeDefinition.Module.ImportReference(componentTypeWrapper.GetField("NotNull").GetDefinition()));
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            var label2 = il.Body.Instructions.Last();

            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Stloc_2);
            var label3 = il.Body.Instructions.Last();

            il.Emit(OpCodes.Ldloc_0);
            var label4 = il.Body.Instructions.Last();
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Ldloc_0);
            var label5 = il.Body.Instructions.Last();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, _typeDefinition.Module.ImportReference(
                new TypeDefinitionWrapper(_typeDefinition.BaseType.Resolve()).GetField("CurrentEntityPoolSize").GetDefinition()));
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Stloc_3);

            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Brtrue_S, label6);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_2);

            il.Emit(OpCodes.Ldloc_2);
            var label7 = il.Body.Instructions.Last();
            il.Emit(OpCodes.Ret);


            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Br_S, label5), label1,
                InjectLineOrder.After);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Brfalse_S, label4), label2,
                InjectLineOrder.After);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Br_S, label7), label3,
                InjectLineOrder.After);


            _typeDefinition.Methods.Add(method);
        }

        public MethodDefinitionWrapper GetConstructor(params Type[] argumentTypes)
        {
            return GetMethod(".ctor", argumentTypes);
        }

        public FieldDefinitionWrapper GetField(string name)
        {
            foreach (var field in _typeDefinition.Fields)
            {
                if (field.Name != name)
                {
                    continue;
                }

                return new FieldDefinitionWrapper(field);
            }

            return null;
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