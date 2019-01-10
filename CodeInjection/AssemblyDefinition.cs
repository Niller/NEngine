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


    public class ClassDefinition
    {
        private TypeDefinition _typeDefinition;

        public ClassDefinition(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
        }

        public void InjectArray(Type type, string name)
        {
            //_typeDefinition.Module.GetType()
            //_typeDefinition.Fields.Add(new FieldDefinition(name, FieldAttributes.Private, new ArrayType(type)));
        }
    }

    public class AssemblyDefinition : IDisposable
    {
        private ModuleDefinition _moduleDefinition;

        public AssemblyDefinition(string assemblyPath)
        {
            _moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters {ReadWrite = true});
            // Modify the assembly
            /*
            // получаем метод Console.WriteLine, используя стандартные методы Reflection
            var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            // создаем ссылку на метод, полученный Reflection, для использования в Mono.Cecil
            var writeLineRef = module.ImportReference(writeLineMethod);
            foreach (var typeDef in module.Types)
            {
                foreach (var method in typeDef.Methods)
                {
                    // Для каждого метода в полученной сборке
                    // Загружаем на стек строку "Inject!"
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "Inject!"));
                    // Вызываем метод Console.WriteLine, параметры он берет со стека - в данном случае строку "Injected".
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, writeLineRef));
                }
            }
            */
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

        public void Save()
        {
            _moduleDefinition.Write();
        }
    }
}