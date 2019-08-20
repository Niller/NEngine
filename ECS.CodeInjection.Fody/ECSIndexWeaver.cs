using System.Collections.Generic;
using System.Linq;
using CodeInjection.Experimental;
using ECS.Experimental;
using Fody;
using Mono.Cecil;

namespace ECS.CodeInjection
{
    public class ECSIndexWeaver
    {
        public void Execute(ModuleDefinition moduleDefinition)
        {
            var assembly = new Assembly(moduleDefinition);
            var components = assembly.GetAllTypesByAttribute(assembly.Import<ComponentAttribute>());

            var componentAttributeType = assembly.Import<ComponentAttribute>();

            var voidType = assembly.Import(typeof(void));

            foreach (var component in components)
            {
                var properties = component.GetProperties(new This(component))
                    .Where(p => p.HasAttribute(assembly.Import<EntityIndexAttribute>())).ToList();

                var addEntityByIndexMethod = component.AddMethod("AddEntityByIndex", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType);
                var removeEntityByIndexMethod = component.AddMethod("RemoveEntityByIndex", MonoCecilUtilities.GetPublicInterfaceMethodAttributes(), voidType);

                var componentAttribute = component.GetAttribute(componentAttributeType);
                var contextType = ((TypeDefinition)componentAttribute.GetArgumentsValues().First()).ToWrapper();

                if (properties.Count <= 0)
                {
                    continue;
                }

                foreach (var property in properties)
                {
                    //co
                }

            }
        }
    }
}