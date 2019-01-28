using ECS;
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
                var entity = context.GetEntity(entityId);
                var transform = entity.GetComponent<TransformComponent>();

                transform.Rotation = new Vector3(transform.Rotation.X + 0.01f, transform.Rotation.Y + 0.01f, transform.Rotation.Z);
            }
        }
    }
}
