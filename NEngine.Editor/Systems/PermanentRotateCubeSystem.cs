using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;

namespace NEngine.Editor.Systems
{
    public class PermanentRotateCubeSystem : IExecuteSystem
    {
        public void Execute()
        {
            var context = Services.ECS.GetContext("Main");
            //context.
        }
    }
}
