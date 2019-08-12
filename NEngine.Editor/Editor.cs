using ECS;
using NEngine.Editor.Systems;

namespace NEngine.Editor
{
    public class Editor 
    {
        public void Initialize()
        {
            var feature = Services.ECS.AddFeature("Rendering");
            feature.AddSystem<RenderInitializeSystem>();
            feature.AddSystem<RenderSystem>();

            feature = Services.ECS.AddFeature("Editor");
            feature.AddSystem<TestSceneInitializeSystem>();
            feature.AddSystem<PermanentRotateCubeSystem>();
        }
    }
}
