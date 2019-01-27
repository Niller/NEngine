using System;
using System.Collections.Generic;
using System.Linq;
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
                var componentContextMapping = new Dictionary<string, string>();
                var contexts = new Dictionary<string, TypeDefinitionWrapper>();

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

                    components.Add(componentType.FullName);
                    ECSInjectionCache.Components.Add(componentType.FullName, componentType);
                    componentContextMapping.Add(componentType.FullName, context);

                    
                }

                var managerAttribute = typeof(ECSManagerAttribute);
                var ecsManagers = nEngineEditor.GetTypesByAttribute(managerAttribute);

                if (ecsManagers.Count > 1)
                {
                    throw new NotImplementedException();
                }

                var manager = ecsManagers.First();
                var managerCtor = manager.GetConstructor();
                
                foreach (var componentsForContext in ECSInjectionCache.ComponentsForContexts)
                {
                    var context = nEngineEditor.GetType(componentsForContext.Key);

                    contexts[context.FullName] = context;

                    var ctor = context.GetConstructor();
                    var resizeMethod =
                        context.InjectOverrideMethod(ECSInjectionCache.BaseContextType.GetMethod("Resize"), true);

                    foreach (var componentType in componentsForContext.Value)
                    {
                        var field = context.InjectComponentsListField(typeof(ComponentsList<>), componentType,
                            ComponentsArrayPrefix + ECSInjectionUtilities.GetTypeName(componentType));
                        //TODO fix hardcode (16, 128)
                        ctor.InjectComponentsListInitialization(field, ECSInjectionCache.Components[componentType], 16, 128, ctor.GetEndLineIndex(), InjectLineOrder.Before);
                        resizeMethod.InjectComponentsListResize(field, ECSInjectionCache.Components[componentType], 128, Operation.Add, resizeMethod.GetEndLineIndex(),
                            InjectLineOrder.Before);
                        context.InjectHasEntityMethod(field, ECSInjectionCache.Components[componentType]);
                        context.InjectGetEntityMethod(field, ECSInjectionCache.Components[componentType], typeof(Entity).FullName);
                        context.InjectGetAllEntitiesMethod(field, ECSInjectionCache.Components[componentType]);     
                        context.InjectHasComponentMethod(field, ECSInjectionCache.Components[componentType], typeof(Entity).FullName);
                        context.InjectAddComponentVoidMethod(field, ECSInjectionCache.Components[componentType], typeof(Entity).FullName);
                        context.InjectAddComponentMethod(field, ECSInjectionCache.Components[componentType], typeof(Entity).FullName);
                        context.InjectGetComponentMethod(field, ECSInjectionCache.Components[componentType], typeof(Entity).FullName);
                    }

                    managerCtor.InjectDictionaryAdd(manager.GetBaseType().GetField("Contexts"), typeof(BaseContext), context);

                }

                foreach (var type in nEngineEditor.GetAllTypes())
                {
                    foreach (var method in type.GetAllMethods())
                    {
                        method.ReplaceContextCalls(typeof(BaseContext), componentContextMapping, contexts);
                        method.ReplaceHasComponentCall(typeof(BaseContext), componentContextMapping, contexts, typeof(Entity));
                        method.ReplaceAddComponentVoidCall(typeof(BaseContext), componentContextMapping, contexts, typeof(Entity));
                    }
                }

                nEngineEditor.Save();
            }
        }

    }
}
