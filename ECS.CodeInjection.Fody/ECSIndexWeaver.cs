using System.Linq;
using CodeInjection.Experimental;
using ECS.Experimental;
using Mono.Cecil;

namespace ECS.CodeInjection
{
    public class ECSIndexWeaver
    {
        private const string AddEntityWithIndexMethodName = "AddEntityWithIndex";
        private const string RemoveEntityWithIndexMethodName = "RemoveEntityWithIndex";
        public void Execute(ModuleDefinition moduleDefinition)
        {
            var assembly = new Assembly(moduleDefinition);
            var components = assembly.GetAllTypesByAttribute(assembly.Import<ComponentAttribute>());

            var componentAttributeType = assembly.Import<ComponentAttribute>();

            var voidType = assembly.Import(typeof(void));
            var contextType = assembly.Import<Context>();
            var intType = assembly.Import<int>();
            var systemType = assembly.Import<System.Type>();

            var addContextMethod = assembly.Import(contextType.GetMethod(AddEntityWithIndexMethodName, 
                systemType.ToParameterType(), intType.ToParameterType(), intType.ToParameterType()));
            var removeContextMethod = assembly.Import(contextType.GetMethod(RemoveEntityWithIndexMethodName,
                systemType.ToParameterType(), intType.ToParameterType(), intType.ToParameterType()));

            foreach (var component in components)
            {
                var properties = component.GetProperties(new This(component))
                    .Where(p => p.HasAttribute(assembly.Import<EntityIndexAttribute>())).ToList();

                var addEntityByIndexMethod = component.AddMethod("AddEntityByIndex", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType);
                var removeEntityByIndexMethod = component.AddMethod("RemoveEntityByIndex", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType);

                if (properties.Count != 1)
                {
                    continue;
                }

                var indexProperty = properties.First();
                var getHashCodeMethod = assembly.Import(indexProperty.Type.GetMethod("GetHashCode", intType));

                var typeField = component.GetField(ECSComponentsWeaver.TypeFieldName, new This(component));
                var contextField = component.GetField(ECSComponentsWeaver.ContextFieldName, new This(component));
                var sourceEntityField = component.GetField(ECSComponentsWeaver.SourceEntityIdFieldName, new This(component));

                var addState = addEntityByIndexMethod.GetState(Method.DefaultStates.MethodStart);
                addState.
                    AddVariable(0, intType).
                    AddVariableSet(0, indexProperty).
                    AddVariable(1, intType).
                    Call(getHashCodeMethod, addState.GetVariable(1), addState.GetVariable(0, true)).
                    Call(addContextMethod, null, contextField, typeField, addState.GetVariable(1), sourceEntityField);

                var removeState = removeEntityByIndexMethod.GetState(Method.DefaultStates.MethodStart);
                removeState.
                    AddVariable(0, intType).
                    AddVariableSet(0, indexProperty).
                    AddVariable(1, intType).
                    Call(getHashCodeMethod, removeState.GetVariable(1), removeState.GetVariable(0, true)).
                    Call(removeContextMethod, null, contextField, typeField, removeState.GetVariable(1), sourceEntityField);

                foreach (var property in properties)
                {
                    var setMethod = property.GetPropertySetMethod();
                    if (setMethod == null)
                    {
                        continue;
                    }

                    var setMethodState = setMethod.GetState(Method.DefaultStates.MethodEnd);
                    setMethodState.
                        Call(removeEntityByIndexMethod, null, new This(component)).
                        Call(addEntityByIndexMethod, null, new This(component));
                }

            }

            //ExceptionLogger.Save();
        }
    }
}