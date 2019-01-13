using System;
using System.ComponentModel;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection
{
    public class MethodDefinitionWrapper
    {
        private readonly MethodDefinition _methodDefinition;

        public MethodDefinitionWrapper(MethodDefinition methodDefinition)
        {
            _methodDefinition = methodDefinition;
        }

        public void InjectArrayInitialization(FieldDefinitionWrapper field, int capacity, int lineIndex, InjectLineOrder orderType)
        {
            if (!field.GetDefinition().FieldType.IsArray)
            {
                throw new Exception($"Field {field.GetDefinition().Name} is not array time");
            }

            var il = _methodDefinition.Body.GetILProcessor();

            var milestone = _methodDefinition.Body.Instructions[lineIndex];

            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldarg_0), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Ldc_I4, capacity), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Newarr, field.GetDefinition().FieldType.GetElementType()), milestone, InjectLineOrder.Before);
            CodeInjectionUtilities.Inject(il, il.Create(OpCodes.Stfld, field.GetDefinition()), milestone, InjectLineOrder.Before);
        }

        public int GetEndLineIndex()
        {
            return _methodDefinition.Body.Instructions.Count - 1;
        }
    }
}