using System;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Assembly
    {
        private readonly ModuleDefinition _moduleDefinition;

        public Assembly(string assemblyPath)
        {
            _moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { ReadWrite = true });
        }

        public Type GetType(string fullname)
        {
            return new Type(_moduleDefinition.GetType(fullname));
        }

        public Type AddType(string fullname, TypeAttributes typeAttributes)
        {
            var typeDefinition = new TypeDefinition(NameUtilities.GetNamespace(fullname),
                NameUtilities.GetName(fullname), typeAttributes);

            return new Type(typeDefinition);
        }
    }
}