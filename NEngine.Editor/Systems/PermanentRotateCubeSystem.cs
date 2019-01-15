using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Math.Vectors;
using NEngine.Editor.Components;

namespace NEngine.Editor.Systems
{
    public class PermanentRotateCubeSystem : IExecuteSystem
    {
        public void Execute()
        {
            var context = Services.ECS.GetContext("Main");
            foreach (var entityId in context.GetAllEntities<StartCubeComponent>())
            {
                var entity = context.GetEntity(entityId);
                var transform = entity.GetComponent<TransformComponent>();

                transform.Rotation = new Vector3(transform.Rotation.X + 0.01f, transform.Rotation.Y + 0.01f, transform.Rotation.Z);
            }
        }
    }
}
