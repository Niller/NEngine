using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeInjection.Experimental;
using ECS.Experimental;
using Fody;
using Logger;
using Mono.Cecil;
using Type = CodeInjection.Experimental.Type;

// ReSharper disable AccessToDisposedClosure

namespace ECS.CodeInjection
{
    public class ECSComponentsWeaver : BaseModuleWeaver
    {
        private const string ContextFieldName = "Context_generated";
        private const string SourceEntityIdFieldName = "SourceEntityId_generated";
        private const string TypeFieldName = "Type_generated";
        private const string MarkDirtyMethodName = "MarkComponentDirty";

        public override void Execute()
        {
            var assembly = new Assembly(ModuleDefinition);
            var components = assembly.GetAllTypesByAttribute(assembly.Import<ComponentAttribute>());

            var contextType = assembly.Import<Context>();
            var intType = assembly.Import<int>();
            var systemType = assembly.Import<System.Type>();

            foreach (var component in components)
            {
                var contextField = component.AddField(ContextFieldName, contextType, FieldAttributes.Public);
                var entityIdField = component.AddField(SourceEntityIdFieldName, intType, FieldAttributes.Public);
                var typeField = component.AddField(TypeFieldName, systemType, FieldAttributes.Public);

                var properties = component.GetProperties()
                    .Where(p => p.HasAttribute(assembly.Import<NotifyPropertyChangedAttribute>()));

                foreach (var property in properties)
                {
                    var setMethod = property.GetPropertySetMethod();
                    if (setMethod == null)
                    {
                        continue;
                    }

                    var setDirtyMethod = assembly.Import(contextType.GetMethod(MarkDirtyMethodName, intType.ToParameterType(), systemType.ToParameterType()));

                    var setMethodState = setMethod.GetState(Method.DefaultStates.MethodEnd);
                    setMethodState.Call(setDirtyMethod, assembly.Import(contextField), entityIdField, typeField);
                }
            }

            //ExceptionLogger.Save();
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}