using CodeInjection;

namespace ECS.CodeInjection
{
    class Program
    {
        //args[0] - ECS.Core.dll;
        //args[1] - ECS.dll;
        //args[2] - ECS components source dll
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                args = new[]
                {
                    "G:\\projects\\NEngine\\ECS.Core\\bin\\Debug\\ECS.Core.dll",
                    "G:\\projects\\NEngine\\NEngine\\bin\\Debug\\NEngine.ECS.dll",
                    "G:\\projects\\NEngine\\NEngine\\bin\\Debug\\NEngine.Editor.dll",
                };
            }

            InjectionCache.Initialize(args);
            using (var nEngineEcs = new AssemblyDefinition(args[1]))
            {
                nEngineEcs.AddClass("NEngine.ECS.Contexts", "MainContext", typeof(BaseContext));
                nEngineEcs.Save();
            }
        }
    }
}
