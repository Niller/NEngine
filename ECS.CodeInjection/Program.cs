#define V2


using CodeInjection.Experimental;
using Mono.Cecil;

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
                    "G:\\projects\\NEngine\\NEngine\\bin\\Debug\\ECS.Core.dll",
                    "G:\\projects\\NEngine\\NEngine\\bin\\Debug\\NEngine.Editor.dll",
                };
            }

#if V2
            using (var assembly = new Assembly(args[0]))
            {
                //var type1 = assembly.AddType("Test.TestClass", TypeAttributes.Class);
                var type1 = assembly.GetType("ECS.TestContext");
                var intType = assembly.ImportType<int>();

                type1.AddField("field1", intType, FieldAttributes.Private);
                type1.AddField("field2", type1, FieldAttributes.Private);

                var method1 = type1.AddMethod("method1", MethodAttributes.Private, intType,
                    new ParameterType("arg1", intType), new ParameterType("arg2", intType));

                var method1State = method1.GetState(Method.DefaultStates.MethodStart);

                method1State.AddVariable("v1", intType)
                    .MathOperation(MathOperation.Add, method1State.GetArgument(0), method1State.GetArgument(1))
                    .AddVariableSet("v1").ReturnValue(method1State.GetVariable("v1"));


                assembly.Save();
            }
#else
            InjectionCache.Initialize(args);

            using (var ecsCore = new ModuleDefinitionWrapper(args[0]))
            {
                ECSInjectionCache.BaseContextType = ecsCore.GetType(typeof(BaseContext));
                ECSInjectionCache.EntityType = ecsCore.GetType(typeof(Entity));


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
                    var entityCtor = ECSInjectionCache.EntityType.GetConstructor(typeof(BaseContext), typeof(int));

                    foreach (var componentsForContext in ECSInjectionCache.ComponentsForContexts)
                    {
                        var context = nEngineEditor.GetType(componentsForContext.Key);

                        contexts[context.FullName] = context;

                        var contextCtor = context.GetConstructor();
                        var resizeMethod =
                            context.InjectOverrideMethod(ECSInjectionCache.BaseContextType.GetMethod("Resize"), true);

                        foreach (var componentType in componentsForContext.Value)
                        {
                            var field = context.InjectComponentsListField(typeof(ComponentsList<>), componentType,
                                ComponentsArrayPrefix + ECSInjectionUtilities.GetTypeName(componentType));

                            var componentTypeWrapper = ECSInjectionCache.Components[componentType];
                            contextCtor.InjectComponentsListInitialization(field, componentTypeWrapper, 16, 128,
                                contextCtor.GetEndLineIndex(), InjectLineOrder.Before);

                            resizeMethod.InjectComponentsListResize(field, componentTypeWrapper, 128, Operation.Add,
                                resizeMethod.GetEndLineIndex(),
                                InjectLineOrder.Before);
                            context.InjectHasEntityMethod(field, componentTypeWrapper);
                            context.InjectGetEntityMethod(field, componentTypeWrapper, typeof(Entity).FullName);
                            context.InjectGetAllEntitiesMethod(field, componentTypeWrapper);
                            context.InjectHasComponentMethod(field, componentTypeWrapper);
                            context.InjectAddComponentVoidMethod(field, componentTypeWrapper);
                            context.InjectAddComponentMethod(field, componentTypeWrapper);
                            context.InjectGetComponentMethod(field, componentTypeWrapper);

                            contextCtor.InjectContextGenericMethods(componentTypeWrapper,
                                typeof(BaseContext.ContextGenericMethods));
                            entityCtor.InjectEntityGenericMethods(componentTypeWrapper, context,
                                typeof(Entity.EntityGenericMethods<>));
                        }

                        managerCtor.InjectDictionaryAdd(manager.GetBaseType().GetField("Contexts"), typeof(BaseContext),
                            context);

                    }

#if !DEBUG
                foreach (var type in nEngineEditor.GetAllTypes())
                {
                    foreach (var method in type.GetAllMethods())
                    {
                        method.ReplaceContextCalls(typeof(BaseContext), componentContextMapping, contexts);
                        method.ReplaceHasComponentCall(typeof(BaseContext), componentContextMapping, contexts, typeof(Entity));
                        method.ReplaceAddComponentVoidCall(typeof(BaseContext), componentContextMapping, contexts, typeof(Entity));
                        method.ReplaceGetComponentCall(typeof(BaseContext), componentContextMapping, contexts, typeof(Entity));
                    }
                }
#endif

                    nEngineEditor.Save();
                }

                ecsCore.Save();
            }

#endif
        }

    }
}
