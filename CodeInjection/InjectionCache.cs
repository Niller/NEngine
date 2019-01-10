using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace CodeInjection
{
    public static class InjectionCache
    {
        public static Dictionary<string, TypeReference> Types;

        public static void Initialize(params string[] assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { ReadWrite = true }))
                {
                    foreach (var type in moduleDefinition.GetAllTypes())
                    {
                        if (Types.ContainsKey(type.FullName))
                        {
                            throw new Exception("Type with such fullname has been already added");
                        }       
                        Types.Add(type.FullName, type);
                    }
                }
            }
            
        }
    }
}