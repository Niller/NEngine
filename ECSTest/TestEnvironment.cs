using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS.Experimental;

namespace ECSTest
{
    public class TestEnvironment
    {
        public TestEnvironment()
        {
            var context = new Context();
            var entity = context.CreateEntity();
            ref var testComponent2 = ref entity.AddComponent<TestComponent2>();
            testComponent2.Y2 = true;
            entity.GetComponent(ref testComponent2);
            Console.WriteLine(testComponent2.Y2);
            Console.ReadKey();
        }
    }
}
