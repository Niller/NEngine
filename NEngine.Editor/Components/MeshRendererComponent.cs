using ECS;
using NEngine.Rendering;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct MeshRendererComponent
    {
        public MeshRendererComponent(Mesh mesh)
        {
            Mesh = mesh;
        }

        public Mesh Mesh;
    }
}