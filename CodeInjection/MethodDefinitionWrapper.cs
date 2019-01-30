using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CodeInjection
{
    public class MethodDefinitionWrapper
    {
        private readonly MethodDefinition _methodDefinition;

        public MethodDefinitionWrapper(MethodDefinition methodDefinition)
        {
            _methodDefinition = methodDefinition;
        }

        internal MethodDefinition GetDefinition()
        {
            return _methodDefinition;
        }

        public void InjectDictionaryAdd(FieldDefinitionWrapper dictionaryField, Type baseContextType, TypeDefinitionWrapper contextType)
        {
            var dictionaryFieldDefinition = _methodDefinition.Module.ImportReference(dictionaryField.GetDefinition());

            var il = _methodDefinition.Body.GetILProcessor();
            var milestone = _methodDefinition.Body.Instructions.Last();

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, dictionaryFieldDefinition), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldtoken, contextType.GetDefinition()), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Call, _methodDefinition.Module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj, contextType.GetConstructor().GetDefinition()), milestone, InjectLineOrder.Before);

            //TODO Refactor GetMethod
            var addMethod = dictionaryFieldDefinition.FieldType.Resolve().AsWrapper().GetMethod("Add", typeof(Type), typeof(Type)).GetDefinition();
            var genericAddMethod =_methodDefinition.Module.ImportReference(
                addMethod.MakeGeneric(_methodDefinition.Module.ImportReference(typeof(Type)),
                    InjectionCache.GetType(baseContextType.FullName)));

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Callvirt, genericAddMethod), milestone, InjectLineOrder.Before);
        }

        public void InjectComponentsListInitialization(FieldDefinitionWrapper listField, TypeDefinitionWrapper itemType, int capacityItems, int capacityIds, int lineIndex, InjectLineOrder orderType)
        {
            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions[lineIndex];

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, capacityItems), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, capacityIds), milestone, InjectLineOrder.Before);

            var ctor = listField.GetDefinition().FieldType.Resolve().AsWrapper().GetConstructor(typeof(int), typeof(int)).GetDefinition();

            var genericMethod = ctor.MakeGeneric(_methodDefinition.Module.ImportReference(itemType.GetDefinition()));

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj, _methodDefinition.Module.ImportReference(genericMethod)), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stfld, listField.GetDefinition()), milestone, InjectLineOrder.Before);
        }

        public void InjectEntityGenericMethods(TypeDefinitionWrapper componentTypeWrapper, Type genericMethodsType)
        {
            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions.Last();
            var module = _methodDefinition.Module;

            FieldDefinitionWrapper genericMethodsDictField =
                module.ImportReference(_methodDefinition.DeclaringType.BaseType.AsWrapper().GetField("GenericMethods").GetDefinition()).Resolve().AsWrapper();

            var entityGenericMethodsType = module.ImportReference(module.ImportReference(genericMethodsType).Resolve().MakeGenericType(componentTypeWrapper.GetDefinition())).Resolve();
            var getComponentDelegateType = module.ImportReference(entityGenericMethodsType.NestedTypes.First(t => t.Name == "GetComponentDelegate").Resolve());
            var hasComponentDelegateType = module.ImportReference(entityGenericMethodsType.NestedTypes.First(t => t.Name == "HasComponentDelegate").Resolve());
            var addComponentVoidDelegateType = module.ImportReference(entityGenericMethodsType.NestedTypes.First(t => t.Name == "AddComponentVoidDelegate").Resolve());
            var addComponentDelegateType = module.ImportReference(entityGenericMethodsType.NestedTypes.First(t => t.Name == "AddComponentDelegate").Resolve());

            _methodDefinition.Body.InitLocals = true;
            var tempVar1 = new VariableDefinition(getComponentDelegateType);
            var tempVar2 = new VariableDefinition(hasComponentDelegateType);
            var tempVar3 = new VariableDefinition(addComponentVoidDelegateType);
            var tempVar4 = new VariableDefinition(addComponentDelegateType);
            _methodDefinition.Body.Variables.Add(tempVar1);
            _methodDefinition.Body.Variables.Add(tempVar2);
            _methodDefinition.Body.Variables.Add(tempVar3);
            _methodDefinition.Body.Variables.Add(tempVar4);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, module.ImportReference(genericMethodsDictField.GetDefinition())), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldtoken, module.ImportReference(componentTypeWrapper.GetDefinition())), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                _methodDefinition.DeclaringType.AsWrapper().GetMethod("GetComponent_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                module.ImportReference(getComponentDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar1), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                    _methodDefinition.DeclaringType.AsWrapper().GetMethod("HasComponent_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(module.ImportReference(hasComponentDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2)))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar2), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                    _methodDefinition.DeclaringType.AsWrapper().GetMethod("AddComponentVoid_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(module.ImportReference(addComponentVoidDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2)))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar3), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                    _methodDefinition.DeclaringType.AsWrapper().GetMethod("AddComponent_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(module.ImportReference(hasComponentDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2)))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar2), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar2), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar3), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(entityGenericMethodsType.Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 3))), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Callvirt,
                _methodDefinition.Module.ImportReference(genericMethodsDictField.GetDefinition().FieldType.AsWrapper().GetMethod("set_Item").GetDefinition().
                    MakeGeneric(_methodDefinition.Module.ImportReference(typeof(Type)), entityGenericMethodsType))), milestone, InjectLineOrder.Before);

        }

        public void InjectContextGenericMethods(TypeDefinitionWrapper componentTypeWrapper, Type genericMethodsType)
        {
            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions.Last();
            var module = _methodDefinition.Module;

            FieldDefinitionWrapper genericMethodsDictField =
                module.ImportReference(_methodDefinition.DeclaringType.BaseType.AsWrapper().GetField("GenericMethods").GetDefinition()).Resolve().AsWrapper();

            var contextGenericMethodsType = module.ImportReference(genericMethodsType).Resolve();
            var getEntityDelegateType = module.ImportReference(contextGenericMethodsType.NestedTypes.First(t => t.Name == "GetEntityDelegate").Resolve());
            var hasEntityDelegateType = module.ImportReference(contextGenericMethodsType.NestedTypes.First(t => t.Name == "HasEntityDelegate").Resolve());
            var getAllEntitiesDelegateType = module.ImportReference(contextGenericMethodsType.NestedTypes.First(t => t.Name == "GetAllEntitiesDelegate").Resolve());

            _methodDefinition.Body.InitLocals = true;
            var tempVar1 = new VariableDefinition(getEntityDelegateType);
            var tempVar2 = new VariableDefinition(hasEntityDelegateType);
            var tempVar3 = new VariableDefinition(getAllEntitiesDelegateType);
            _methodDefinition.Body.Variables.Add(tempVar1);
            _methodDefinition.Body.Variables.Add(tempVar2);
            _methodDefinition.Body.Variables.Add(tempVar3);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, module.ImportReference(genericMethodsDictField.GetDefinition())), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldtoken, module.ImportReference(componentTypeWrapper.GetDefinition())), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))), milestone, InjectLineOrder.Before);
             
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn, 
                _methodDefinition.DeclaringType.AsWrapper().GetMethod("GetEntity_"+componentTypeWrapper.GetDefinition().Name).GetDefinition()), 
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj, 
                module.ImportReference(getEntityDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar1), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                    _methodDefinition.DeclaringType.AsWrapper().GetMethod("HasEntity_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(module.ImportReference(hasEntityDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2)))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar2), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldftn,
                    _methodDefinition.DeclaringType.AsWrapper().GetMethod("GetAllEntities_" + componentTypeWrapper.GetDefinition().Name).GetDefinition()),
                milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj,
                _methodDefinition.Module.ImportReference(module.ImportReference(getAllEntitiesDelegateType.Resolve().Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 2)))), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar3), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar2), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar3), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newobj, 
                _methodDefinition.Module.ImportReference(contextGenericMethodsType.Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 3))), milestone, InjectLineOrder.Before);

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Callvirt,
                _methodDefinition.Module.ImportReference(genericMethodsDictField.GetDefinition().FieldType.AsWrapper().GetMethod("set_Item").GetDefinition().
                    MakeGeneric(_methodDefinition.Module.ImportReference(typeof(Type)), contextGenericMethodsType))), milestone, InjectLineOrder.Before);
                    
        }

        public void InjectComponentsListResize(FieldDefinitionWrapper listField, TypeDefinitionWrapper itemType, int value, Operation operation, int lineIndex, InjectLineOrder orderType)
        {
            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions[lineIndex];

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, listField.GetDefinition()), milestone, InjectLineOrder.Before);

            if (operation == Operation.Set)
            {
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, value), milestone, InjectLineOrder.Before);
            }
            else
            {
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone,
                    InjectLineOrder.Before);
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, listField.GetDefinition()),
                    milestone, InjectLineOrder.Before);
                var getLengthMethod = _methodDefinition.Module.ImportReference(listField.GetDefinition().FieldType.Resolve().AsWrapper().GetMethod("get_Length").GetDefinition()).Resolve();
                var genericGetLengthMethod = getLengthMethod.MakeGeneric(itemType.GetDefinition());
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(genericGetLengthMethod)), milestone, InjectLineOrder.Before);
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, value), milestone,
                    InjectLineOrder.Before);
                switch (operation)
                {
                    case Operation.Add:
                        CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Add), milestone,
                            InjectLineOrder.Before);
                        break;
                    case Operation.Sub:
                        CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Sub), milestone,
                            InjectLineOrder.Before);
                        break;
                    case Operation.Mul:
                        CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Mul), milestone,
                            InjectLineOrder.Before);
                        break;
                    case Operation.Div:
                        CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Div), milestone,
                            InjectLineOrder.Before);
                        break;
                }
            }

            var resizeMethod = _methodDefinition.Module.ImportReference(listField.GetDefinition().FieldType.Resolve().AsWrapper().GetMethod("ResizeIds", typeof(int)).GetDefinition()).Resolve();
            var genericResizeMethod = resizeMethod.MakeGeneric(itemType.GetDefinition());

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(genericResizeMethod)), milestone, InjectLineOrder.Before);
        }

        public void ReplaceHasComponentCall(Type baseContextType, Dictionary<string, string> componentContextMapping,
            Dictionary<string, TypeDefinitionWrapper> contexts, Type entityType)
        {
            var entityTypeWrapper = InjectionCache.GetType(entityType.FullName).AsWrapper();

            _methodDefinition.Body.SimplifyMacros();

            for (int i = 0; i < _methodDefinition.Body.Instructions.Count; ++i)
            {
                var instruction = _methodDefinition.Body.Instructions[i];
                if (instruction.OpCode != OpCodes.Call)
                {
                    continue;
                }

                var method = ((MethodReference) instruction.Operand);

                if (method.DeclaringType.FullName != entityType.FullName)
                {
                    continue;
                }

                var tempVar1 = new VariableDefinition(
                    _methodDefinition.Module.ImportReference(new ByReferenceType(entityTypeWrapper.GetDefinition())));
                _methodDefinition.Body.Variables.Add(tempVar1);


                if (method.Name == "HasComponent")
                {
                    if (!(method is GenericInstanceMethod genericInstanceMethod))
                    {
                        continue;
                    }

                    var componentType = genericInstanceMethod.GenericArguments.First();
                    var contextType = contexts[componentContextMapping[componentType.FullName]];
                    var newMethod = contextType.GetMethod("HasComponent" + "_" + componentType.Name, entityType)
                        .GetDefinition();
                    instruction.Operand = newMethod;

                    var il = _methodDefinition.Body.GetILProcessor();

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld,
                            _methodDefinition.Module.ImportReference(
                                entityTypeWrapper.GetField("CurrentContext").GetDefinition())), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il,
                        il.Create(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(newMethod)),
                        instruction,
                        InjectLineOrder.Before);

                    _methodDefinition.Body.Instructions.Remove(instruction);
                }
            }

            _methodDefinition.Body.SimplifyMacros();
        }

        public void ReplaceGetComponentCall(Type baseContextType, Dictionary<string, string> componentContextMapping,
            Dictionary<string, TypeDefinitionWrapper> contexts, Type entityType)
        {
            var entityTypeWrapper = InjectionCache.GetType(entityType.FullName).AsWrapper();

            _methodDefinition.Body.SimplifyMacros();

            for (int i = 0; i < _methodDefinition.Body.Instructions.Count; ++i)
            {
                var instruction = _methodDefinition.Body.Instructions[i];
                if (instruction.OpCode != OpCodes.Call)
                {
                    continue;
                }

                var method = ((MethodReference)instruction.Operand);

                if (method.DeclaringType.FullName != entityType.FullName)
                {
                    continue;
                }

                var tempVar1 = new VariableDefinition(
                    _methodDefinition.Module.ImportReference(new ByReferenceType(entityTypeWrapper.GetDefinition())));
                _methodDefinition.Body.Variables.Add(tempVar1);


                if (method.Name == "GetComponent")
                {
                    if (!(method is GenericInstanceMethod genericInstanceMethod))
                    {
                        continue;
                    }

                    var componentType = genericInstanceMethod.GenericArguments.First();
                    var contextType = contexts[componentContextMapping[componentType.FullName]];
                    var newMethod = contextType.GetMethod("GetComponent" + "_" + componentType.Name, entityType)
                        .GetDefinition();
                    instruction.Operand = newMethod;

                    var il = _methodDefinition.Body.GetILProcessor();

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld,
                            _methodDefinition.Module.ImportReference(
                                entityTypeWrapper.GetField("CurrentContext").GetDefinition())), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il,
                        il.Create(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(newMethod)),
                        instruction,
                        InjectLineOrder.Before);

                    _methodDefinition.Body.Instructions.Remove(instruction);
                }
            }

            _methodDefinition.Body.SimplifyMacros();
        }
        /*
        public void ReplaceGenericAddComponentVoidCall(Type baseContextType, Dictionary<string, string> componentContextMapping,
            Dictionary<string, TypeDefinitionWrapper> contexts, Type entityType)
        {
            var entityTypeWrapper = InjectionCache.GetType(entityType.FullName).AsWrapper();
            var voidType = _methodDefinition.Module.ImportReference(typeof(void));

            var genericInstanceMethod = new GenericInstanceMethod(_methodDefinition);

            !var componentType = genericInstanceMethod.GenericArguments.First();

            var contextType = contexts[componentContextMapping[componentType.FullName]];
            var newMethod = contextType.GetDefinition().Methods
                .First(m => m.Name == "AddComponentVoid" + "_" + componentType.Name);

            _methodDefinition.Body.Variables.Clear();

            _methodDefinition.Body.InitLocals = true;
            var tempVar1 = new VariableDefinition(
                _methodDefinition.Module.ImportReference(contextType.GetDefinition()));
            _methodDefinition.Body.Variables.Add(tempVar1);

            _methodDefinition.Body.Instructions.Clear();

            var il = _methodDefinition.Body.GetILProcessor();
            
            il.Emit(OpCodes.Ldarg_0);

            il.Emit(OpCodes.Ldfld, _methodDefinition.Module.ImportReference(entityTypeWrapper.GetField("CurrentContext").GetDefinition()));

            il.Emit(OpCodes.Castclass, contextType.GetDefinition());
            
            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Ldarg_1);

            il.Emit(OpCodes.Ldarg_0);

            il.Emit(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(newMethod));
        }
        */
        public void ReplaceAddComponentVoidCall(Type baseContextType, Dictionary<string, string> componentContextMapping,
            Dictionary<string, TypeDefinitionWrapper> contexts, Type entityType)
        {
            var entityTypeWrapper = InjectionCache.GetType(entityType.FullName).AsWrapper();
            var voidType = _methodDefinition.Module.ImportReference(typeof(void));

            _methodDefinition.Body.SimplifyMacros();

            for (int i = 0; i < _methodDefinition.Body.Instructions.Count; ++i)
            {
                var instruction = _methodDefinition.Body.Instructions[i];
                if (instruction.OpCode != OpCodes.Call)
                {
                    continue;
                }

                var method = ((MethodReference)instruction.Operand);

                if (method.DeclaringType.FullName != entityType.FullName)
                {
                    continue;
                }

                if (method.Name == "AddComponent")
                {
                    if (!(method is GenericInstanceMethod genericInstanceMethod))
                    {
                        continue;
                    }

                    if (method.ReturnType.FullName != typeof(void).FullName)
                    {
                        continue;
                    }

                    var componentType = genericInstanceMethod.GenericArguments.First();
                    var contextType = contexts[componentContextMapping[componentType.FullName]];
                    var newMethod = contextType.GetDefinition().Methods
                        .First(m => m.Name == "AddComponentVoid" + "_" + componentType.Name);

                    var tempVar1 = new VariableDefinition(
                        _methodDefinition.Module.ImportReference(new ByReferenceType(entityTypeWrapper.GetDefinition())));
                    var tempVar2 = new VariableDefinition(
                        _methodDefinition.Module.ImportReference(componentType));
                    var tempVar3 = new VariableDefinition(
                        _methodDefinition.Module.ImportReference(contextType.GetDefinition()));
                    _methodDefinition.Body.Variables.Add(tempVar1);
                    _methodDefinition.Body.Variables.Add(tempVar2);
                    _methodDefinition.Body.Variables.Add(tempVar3);

                    instruction.Operand = newMethod;

                    var il = _methodDefinition.Body.GetILProcessor();

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar2), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar2), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld,
                            _methodDefinition.Module.ImportReference(
                                entityTypeWrapper.GetField("CurrentContext").GetDefinition())), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Castclass, contextType.GetDefinition()), instruction, InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stloc, tempVar3), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar3), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar1), instruction,
                        InjectLineOrder.Before);


                    CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldloc, tempVar2), instruction,
                        InjectLineOrder.Before);

                    CodeInjectionUtilities.Inject(il,
                        il.Create(OpCodes.Callvirt, _methodDefinition.Module.ImportReference(newMethod)),
                        instruction,
                        InjectLineOrder.Before);

                    _methodDefinition.Body.Instructions.Remove(instruction);
                }
            }

            _methodDefinition.Body.SimplifyMacros();
        }

        public void ReplaceContextCalls(Type baseContextType, Dictionary<string, string> componentContextMapping, Dictionary<string, TypeDefinitionWrapper> contexts)
        {
            for (int i = 0, ilen = _methodDefinition.Body.Instructions.Count; i < ilen; ++i)
            {
                var instruction = _methodDefinition.Body.Instructions[i];
                if (instruction.OpCode != OpCodes.Callvirt)
                {
                    continue;
                }

                var method = ((MethodReference) instruction.Operand);

                if (method.DeclaringType.FullName != baseContextType.FullName)
                {
                    continue;
                }

                switch (method.Name)
                {
                    case "HasEntity":
                        ReplaceContextCall("HasEntity", componentContextMapping, contexts, method, instruction);
                        break;
                    case "GetEntity":
                        ReplaceContextCall("GetEntity", componentContextMapping, contexts, method, instruction);
                        break;
                    case "GetAllEntities":
                        ReplaceContextCall("GetAllEntities", componentContextMapping, contexts, method, instruction);
                        break;     
                }
;            }
        }

        private void ReplaceContextCall(string methodName, Dictionary<string, string> componentContextMapping, Dictionary<string, TypeDefinitionWrapper> contexts, MethodReference method,
            Instruction instruction)
        { 
            if (!(method is GenericInstanceMethod genericInstanceMethod))
            {
                return;
            }

            var componentType = genericInstanceMethod.GenericArguments.First();
            var contextType = contexts[componentContextMapping[componentType.FullName]];
            var newMethod = contextType.GetMethod(methodName + "_" + componentType.Name).GetDefinition();
            instruction.Operand = newMethod;
        }

        public int GetEndLineIndex()
        {
            return _methodDefinition.Body.Instructions.Count - 1;
        }
    }
}