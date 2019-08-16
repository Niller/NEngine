using System.Collections.Generic;
using System.Linq;
using CodeInjection.Experimental;
using ECS.Experimental;
using Fody;

namespace ECS.CodeInjection
{
    public class ECSIndexWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            var assembly = new Assembly(ModuleDefinition);
            var components = assembly.GetAllTypesByAttribute(assembly.Import<ComponentAttribute>());

            var componentAttributeType = assembly.Import<ComponentAttribute>();

            foreach (var component in components)
            {
                var properties = component.GetProperties(new This(component))
                    .Where(p => p.HasAttribute(assembly.Import<EntityIndexAttribute>())).ToList();

                if (properties.Count <= 0)
                {
                    continue;
                }

                var componentAttribute = component.GetAttribute(componentAttributeType);
                var contextType = assembly.Import((System.Type)componentAttribute.GetArgumentsValues().First());

                foreach (var property in properties)
                {
                    //co
                }

            }
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}