﻿using System;
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

        public void ReplaceAddComponentVoidCall(Type baseContextType, Dictionary<string, string> componentContextMapping,
            Dictionary<string, TypeDefinitionWrapper> contexts, Type entityType)
        {
            var entityTypeWrapper = InjectionCache.GetType(entityType.FullName).AsWrapper();
            var voidType = _methodDefinition.Module.ImportReference(typeof(void));

            //_methodDefinition.Body.SimplifyMacros();

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

            //_methodDefinition.Body.SimplifyMacros();
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