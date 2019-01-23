using ECS;
using NEngine.Editor.Systems;

namespace NEngine.Editor
{
    public class Editor 
    {
        public void Initialize()
        {
            var feature = new Feature("Rendering");
            feature.AddSystem<RenderSystem>();

            Services.ECS.AddFeature(feature);

            feature = new Feature("Editor");
            feature.AddSystem<TestSceneInitializeSystem>();
            feature.AddSystem<PermanentRotateCubeSystem>();

            Services.ECS.AddFeature(feature);
        }
    }
}
