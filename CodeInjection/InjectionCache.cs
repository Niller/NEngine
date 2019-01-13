﻿using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace CodeInjection
{
    public static class InjectionCache
    {
        private const string BannedTypeName = "<Module>";

        public static Dictionary<string, TypeDefinition> Types = new Dictionary<string, TypeDefinition>();

        public static void Initialize(params string[] assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { ReadWrite = true }))
                {
                    foreach (var type in moduleDefinition.Types)
                    {
                        if (type.FullName != BannedTypeName)
                        {
                            AddType(type.FullName, type);
                        }
                    }
                }
            }
        }

        public static void AddType(string fullName, TypeDefinition typeReference)
        {
            if (Types.ContainsKey(fullName))
            {
                throw new Exception($"Type with fullname {fullName} has been already added");
            }
            Types.Add(fullName, typeReference);
        }

        public static TypeDefinition GetType(string fullname)
        {
            TypeDefinition type;
            if (!Types.TryGetValue(fullname, out type))
            {
                // ReSharper disable once PossibleNullReferenceException
                throw new Exception($"Cannot inject array because of type with name {type.FullName} not registered");
            }

            return type;
        }
    }
}