using System.Drawing;
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
            
            var deviceComponent = context.RegisterComponent(new DeviceComponent(new Vector2Int(640, 480), Services.EditorContext.RenderBitmap, Color.Black));
            deviceEntity.AddComponent(ref deviceComponent);
        }
    }
}