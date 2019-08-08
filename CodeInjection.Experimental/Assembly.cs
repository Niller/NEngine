using System;
using System.Collections.Generic;
using System.Linq;
using Logger;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Assembly : IDisposable
    {
        private readonly ModuleDefinition _moduleDefinition;

        public Assembly(string assemblyPath)
        {
            _moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { ReadWrite = true });
        }

        public Assembly(ModuleDefinition moduleDefinition)
        {
            _moduleDefinition = moduleDefinition;
        }

        public Type GetType(string fullname)
        {
            return _moduleDefinition.GetType(fullname).ToWrapper();
        }

        public IEnumerable<Type> GetAllTypesByAttribute(Type attributeType)
        {
            foreach (var moduleDefinitionType in _moduleDefinition.Types)
            {
                var typeWrapper = moduleDefinitionType.ToWrapper();
                if (typeWrapper.HasAttribute(attributeType))
                {
                    yield return moduleDefinitionType.ToWrapper();
                }
            }
        }

        public Type Import(System.Type type)
        {
            return _moduleDefinition.ImportReference(type).ToWrapper();
        }

        public Type Import<T>()
        {
            return Import(typeof(T));
        }

        public Field Import(Field field)
        {
            return _moduleDefinition.ImportReference(field.GetDefinition()).ToWrapper();
        }

        public Method Import(Method method)
        {
            return _moduleDefinition.ImportReference(method.GetDefinition()).ToWrapper();
        }

        public Type AddType(string fullname, TypeAttributes typeAttributes, Type baseType = null)
        {
            var typeDefinition = new TypeDefinition(NameUtilities.GetNamespace(fullname),
                NameUtilities.GetName(fullname), typeAttributes, baseType == null ? Import<object>().GetDefinition() : baseType.GetDefinition());

            _moduleDefinition.Types.Add(typeDefinition);

            return typeDefinition.ToWrapper();
        }

        public void Save()
        {
            _moduleDefinition.Write();
        }

        public void Save(string filePath)
        {
            _moduleDefinition.Write(filePath);
        }

        public void Dispose()
        {
            _moduleDefinition?.Dispose();
        }
    }
}