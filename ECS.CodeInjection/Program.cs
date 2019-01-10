using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeInjection;

namespace ECS.CodeInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                TestInject.Inject("G:\\projects\\NEngine\\NEngine\\bin\\Debug\\ECS.dll");
                return;
            }

            InjectionCache.Initialize(args);

            TestInject.Inject(args[0]);
        }
    }
}
