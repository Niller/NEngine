using System.Collections.Generic;
using System.Linq;
using CodeInjection.Experimental;
using ECS.Experimental;
using Fody;
using Mono.Cecil;

// ReSharper disable AccessToDisposedClosure
namespace ECS.CodeInjection
{
    public class ECSComponentsWeaver : BaseModuleWeaver
    {
        internal const string ContextFieldName = "Context_generated";
        internal const string SourceEntityIdFieldName = "SourceEntityId_generated";
        internal const string TypeFieldName = "Type_generated";
        internal const string MarkDirtyMethodName = "MarkComponentDirty";

        public override void Execute()
        {
            var assembly = new Assembly(ModuleDefinition);
            var components = assembly.GetAllTypesByAttribute(assembly.Import<ComponentAttribute>());

            var contextType = assembly.Import<Context>();
            var intType = assembly.Import<int>();
            var systemType = assembly.Import<System.Type>();
            var voidType = assembly.Import(typeof(void));
            var interfaceType = assembly.Import<IComponent>();

            foreach (var component in components)
            {
                var contextField = component.AddField(ContextFieldName, contextType, FieldAttributes.Private, new This(component));
                var entityIdField = component.AddField(SourceEntityIdFieldName, intType, FieldAttributes.Private, new This(component));
                var typeField = component.AddField(TypeFieldName, systemType, FieldAttributes.Private, new This(component));

                var setContextMethod = component.AddMethod("SetContext", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType, contextType.ToParameterType("context"));
                var setEntityIdMethod = component.AddMethod("SetEntityId", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType, intType.ToParameterType("entityId"));
                var setTypeMethod = component.AddMethod("SetType", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType, systemType.ToParameterType("type"));

                var setContextMethodState = setContextMethod.GetState(Method.DefaultStates.MethodStart);
                setContextMethodState.AddFieldSet(contextField, setContextMethodState.GetArgument(0));

                var setEntityIdMethodState = setEntityIdMethod.GetState(Method.DefaultStates.MethodStart);
                setEntityIdMethodState.AddFieldSet(entityIdField, setEntityIdMethodState.GetArgument(0));

                var setTypeMethodState = setTypeMethod.GetState(Method.DefaultStates.MethodStart);
                setTypeMethodState.AddFieldSet(typeField, setTypeMethodState.GetArgument(0));

                component.ImplementInterface(interfaceType);

                var properties = component.GetProperties(new This(component))
                    .Where(p => p.HasAttribute(assembly.Import<NotifyPropertyChangedAttribute>()));

                foreach (var property in properties)
                {
                    var setMethod = property.GetPropertySetMethod();
                    if (setMethod == null)
                    {
                        continue;
                    }

                    var setDirtyMethod = assembly.Import(contextType.GetMethod(MarkDirtyMethodName,
                        intType.ToParameterType(), systemType.ToParameterType()));

                    var setMethodState = setMethod.GetState(Method.DefaultStates.MethodEnd);
                    setMethodState.Call(setDirtyMethod, null, contextField, entityIdField, typeField);
                }
            }

            var indexWeaver = new ECSIndexWeaver();
            indexWeaver.Execute(ModuleDefinition);
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}