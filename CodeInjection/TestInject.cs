using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection
{
    public class Assembly : IDisposable
    {
        private ModuleDefinition _moduleDefinition;

        public Assembly(string assemblyPath)
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

        public void InjectArray(Type type, string name)
        {

        }

        public void Dispose()
        {
            _moduleDefinition?.Dispose();
        }

        public void Save()
        {
            _moduleDefinition.Write();
        }
    }

    public static class TestInject
    {
        public static void Inject(string assemblyPath)
        {
            using (var module = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { ReadWrite = true }))
            {
                // Modify the assembly

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

                module.Write(); // Write to the same file that was used to open the file
            }
        }
    }
}
