﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CodeInjection;

namespace ECS.CodeInjection
{
    class Program
    {
        private const string ComponentsArrayPrefix = "_components_";

        //args[0] - ECS.Core.dll;
        //args[1] - NEngine.Editor dll;
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
                    if (!(componentType.GetAttributeParameters(targetAttribute, 0) is string context))
                    {
                        throw new Exception(
                            $"Wrong attribute value in component attribute for component {componentType.FullName}");
                    }

                    if (!ECSInjectionCache.ComponentsForContexts.TryGetValue(context, out var components))
                    {
                        components = new List<string>();
                        ECSInjectionCache.ComponentsForContexts.Add(context, components);
                    }

                    componentType.InjectField("NotNull", typeof(bool));

                    components.Add(componentType.FullName);
                    ECSInjectionCache.Components.Add(componentType.FullName, componentType);
                }

                foreach (var componentsForContext in ECSInjectionCache.ComponentsForContexts)
                {
                    var context = nEngineEditor.AddClass("NEngine.ECS.Contexts",
                        componentsForContext.Key + "Context",
                        typeof(BaseContext));
                    var ctor = context.AddConstructor();
                    var resizeMethod =
                        context.InjectOverrideMethod(ECSInjectionCache.BaseContextType.GetMethod("Resize"), true);

                    foreach (var componentType in componentsForContext.Value)
                    {
                        var field = context.InjectArrayField(componentType,
                            ComponentsArrayPrefix + ECSInjectionUtilities.GetTypeName(componentType));
                        //TODO fix hardcode (100)
                        ctor.InjectArrayInitialization(field, 128, ctor.GetEndLineIndex(), InjectLineOrder.Before);
                        resizeMethod.InjectArrayResize(field, 128, Operation.Add, resizeMethod.GetEndLineIndex(),
                            InjectLineOrder.Before);
                        context.InjectHasEntityMethod(field, ECSInjectionCache.Components[componentType]);
                    }
                }
                nEngineEditor.Save();
            }
        }

    }
}
