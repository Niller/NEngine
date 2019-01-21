using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEngine.Editor
{
    //TODO Implement IoC container
    public static class Services
    {
        public static NEngineECSManager ECS { get; }

        static Services()
        {
            ECS = new NEngineECSManager();
        }
    }
}
