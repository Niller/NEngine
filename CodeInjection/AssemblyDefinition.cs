using System;
using Mono.Cecil;

namespace CodeInjection
{
    public class AssemblyDefinition : IDisposable
    {
        private readonly ModuleDefinition _moduleDefinition;

        public AssemblyDefinition(string assemblyPath)
        {
            _moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters {ReadWrite = true});
        }

        public void Dispose()
        {
            _moduleDefinition?.Dispose();
        }

        public ClassDefinition GetClass(Type type)
        {
            return GetClass(type.FullName);
        }

        public ClassDefinition GetClass(string fullname)
        {
            return new ClassDefinition(_moduleDefinition.GetType(fullname));
        }

        public ClassDefinition AddClass(string @namespace, string name, Type baseClass = null)
        {
            var newType = baseClass == null ? new TypeDefinition(@namespace, name, TypeAttributes.Class) 
                : new TypeDefinition(@namespace, name, TypeAttributes.Class, InjectionCache.GetType(baseClass.FullName));
            newType.Attributes = TypeAttributes.Public;

            _moduleDefinition.Types.Add(newType);
            InjectionCache.Types.Add(newType.FullName, newType);

            return new ClassDefinition(newType);
        }

        public void Save()
        {
            _moduleDefinition.Write();
        }
    }
}