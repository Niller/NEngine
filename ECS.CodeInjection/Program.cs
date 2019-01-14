using System;
using System.Collections.Generic;
using CodeInjection;

namespace ECS.CodeInjection
{
    class Program
    {
        private const string ComponentsArrayPrefix = "_components_";

        //args[0] - ECS.Core.dll;
        //args[1] - ECS.dll;
        //args[2] - ECS components source dll
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                args = new[]
                {
                    "G:\\projects\\NEngine\\ECS.Core\\bin\\Debug\\ECS.Core.dll",
                    "G:\\projects\\NEngine\\NEngine\\bin\\Debug\\NEngine.Editor.dll",
                };
            }

            InjectionCache.Initialize(args);


            using (var ecsCore = new ModuleDefinitionWrapper(args[0]))
            {
                ECSInjectionCache.BaseContextType = ecsCore.GetType(typeof(BaseContext));
            }

            using (var nEngineEditor = new ModuleDefinitionWrapper(args[1]))
            {
                //Find all components
                var targetAttribute = typeof(ComponentAttribute);
                var componentTypes = nEngineEditor.GetTypesByAttribute(targetAttribute);
                foreach (var componentType in componentTypes)
                {
                    var context = componentType.GetAttributeParameters(targetAttribute, 0) as string;
                    if (context == null)
                    {
                        throw new Exception($"Wrong attribute value in component attribute for component {componentType.FullName}");
                    }

                    List<string> components;
                    if (!ECSInjectionCache.ComponentsForContexts.TryGetValue(context, out components))
                    {
                        components = new List<string>();
                        ECSInjectionCache.ComponentsForContexts.Add(context, components);
                    }

                    components.Add(componentType.FullName);
                }

                //Add contexts
                foreach (var componentsForContext in ECSInjectionCache.ComponentsForContexts)
                {
                    var context = nEngineEditor.AddClass("NEngine.ECS.Contexts", componentsForContext.Key + "Context", typeof(BaseContext));
                    var ctor = context.AddConstructor();
                    var resizeMethod =
                        context.InjectOverrideMethod(ECSInjectionCache.BaseContextType.GetMethod("Resize"), true);

                    foreach (var componentType in componentsForContext.Value)
                    {
                        var field = context.InjectArrayField(componentType, ComponentsArrayPrefix + ECSInjectionUtilities.GetTypeName(componentType));
                        //TODO fix hardcode (100)
                        ctor.InjectArrayInitialization(field, 100, ctor.GetEndLineIndex(), InjectLineOrder.Before);
                        resizeMethod.InjectArrayResize(field, 100, Operation.Add, resizeMethod.GetEndLineIndex(), InjectLineOrder.Before);
                    }
                }
                nEngineEditor.Save();
            }
        }
    }
}
