using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Systems
{
    public class RenderInitializeSystem : IInitializeSystem
    {
        public void Execute()
        {
            var context = Services.ECS.GetContext<MainContext>();
            var deviceEntity = context.CreateEntity();
            
            var deviceComponent = new DeviceComponent(new Vector2(640, 480), new byte[640 * 480 * 4], Services.EditorContext.RenderBitmap);
            deviceEntity.AddComponent(ref deviceComponent);
        }
    }
}