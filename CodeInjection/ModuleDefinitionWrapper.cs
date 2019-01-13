using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace CodeInjection
{
    public class ModuleDefinitionWrapper : IDisposable
    {
        private readonly ModuleDefinition _moduleDefinition;

        public ModuleDefinitionWrapper(string assemblyPath)
        {
            _moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters {ReadWrite = true});
        }

        public void Dispose()
        {
            _moduleDefinition?.Dispose();
        }

        public TypeDefinitionWrapper GetType(Type type)
        {
            return GetType(type.FullName);
        }

        public TypeDefinitionWrapper GetType(string fullname)
        {
            return new TypeDefinitionWrapper(_moduleDefinition.GetType(fullname));
        }

        public TypeDefinitionWrapper AddClass(string @namespace, string name, Type baseClass = null)
        {
            var newType = baseClass == null ? new TypeDefinition(@namespace, name, TypeAttributes.Class) 
                : new TypeDefinition(@namespace, name, TypeAttributes.Class, _moduleDefinition.ImportReference(InjectionCache.GetType(baseClass.FullName)));
            newType.Attributes = TypeAttributes.Public;

            _moduleDefinition.Types.Add(newType);
            InjectionCache.Types.Add(newType.FullName, newType);

            return new TypeDefinitionWrapper(newType);
        }

        public List<TypeDefinitionWrapper> GetTypesByAttribute(Type targetAttribute)
        {
            var result = new List<TypeDefinitionWrapper>();
            foreach (var type in _moduleDefinition.Types)
            {
                bool hasAttribute = false;
                if (type.HasCustomAttributes)
                {
                    foreach (var customAttribute in type.CustomAttributes)
                    {
                        if (customAttribute.AttributeType.FullName == targetAttribute.FullName)
                        {
                            hasAttribute = true;
                            break;
                        }
                    }
                }

                if (hasAttribute)
                {
                    result.Add(new TypeDefinitionWrapper(type));
                }
            }

            return result;
        }

        public void Import(string type)
        {
            _moduleDefinition.ImportReference(InjectionCache.GetType(type));
        }

        public void Save()
        {
            _moduleDefinition.Write();
        }
    }
}