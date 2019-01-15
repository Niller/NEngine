using ECS;
using NEngine.Rendering;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct MeshRendererComponent
    {
        public Mesh Mesh;
    }
}