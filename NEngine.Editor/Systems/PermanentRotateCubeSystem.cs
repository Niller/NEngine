using System.Runtime.InteropServices;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Systems
{
    public class PermanentRotateCubeSystem : IExecuteSystem
    {
        public void Execute()
        {
            var context = Services.ECS.GetContext<MainContext>();
            foreach (var entityId in context.GetAllEntities<StartCubeComponent>())
            {
                ref var entity = ref context.GetEntity(entityId);
                ref var transform = ref entity.GetComponent<TransformComponent>();
                transform.Rotation = new Vector3(transform.Rotation.X + 0.001f, transform.Rotation.Y + 0.001f, transform.Rotation.Z);
            }
        }
    }
}
