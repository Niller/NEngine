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
        }
    }
}
