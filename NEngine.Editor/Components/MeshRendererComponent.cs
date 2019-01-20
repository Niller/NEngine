using ECS;
using NEngine.Rendering;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct MeshRendererComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }

        public MeshRendererComponent(Mesh mesh)
        {
            Mesh = mesh;
            HasValue = true;
        }

        public Mesh Mesh;
    }
}