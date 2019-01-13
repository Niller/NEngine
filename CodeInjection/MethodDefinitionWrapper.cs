using System;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CodeInjection
{
    public enum Operation
    {
        Set,
        Add,
        Sub,
        Mul,
        Div
    }

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

        public void InjectArrayInitialization(FieldDefinitionWrapper arrayField, int capacity, int lineIndex, InjectLineOrder orderType)
        {
            if (!arrayField.GetDefinition().FieldType.IsArray)
            {
                throw new Exception($"Field {arrayField.GetDefinition().Name} is not array");
            }

            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions[lineIndex];

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, capacity), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newarr, arrayField.GetDefinition().FieldType.GetElementType()), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stfld, arrayField.GetDefinition()), milestone, InjectLineOrder.Before);
        }

        public void InjectArrayResize(FieldDefinitionWrapper arrayField, int value, Operation operation, int lineIndex, InjectLineOrder orderType)
        {
            if (!arrayField.GetDefinition().FieldType.IsArray)
            {
                throw new Exception($"Field {arrayField.GetDefinition().Name} is not array");
            }

            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions[lineIndex];

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldflda, arrayField.GetDefinition()), milestone, InjectLineOrder.Before);

            if (operation == Operation.Set)
            {
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, value), milestone, InjectLineOrder.Before);
            }
            else
            {
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone,
                    InjectLineOrder.Before);
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldfld, arrayField.GetDefinition()),
                    milestone, InjectLineOrder.Before);
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldlen), milestone, InjectLineOrder.Before);
                CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Conv_I4), milestone,
                    InjectLineOrder.Before);
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

            var resizeMethod = _methodDefinition.Module.ImportReference(typeof(Array).GetMethod("Resize")).Resolve();

            var genericMethod = new GenericInstanceMethod(resizeMethod);
            genericMethod.GenericArguments.Add(arrayField.GetDefinition().FieldType.GetElementType());

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Call, _methodDefinition.Module.ImportReference(genericMethod)), milestone, InjectLineOrder.Before);
        }

        public int GetEndLineIndex()
        {
            return _methodDefinition.Body.Instructions.Count - 1;
        }
    }
}