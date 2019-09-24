using ECS;
using ECS.Experimental;
using NEngine.Editor.Contexts;
using NEngine.Rendering;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct MeshRendererComponent
    {
        public MeshRendererComponent(Mesh mesh)
        {
            Mesh = mesh;
        }

        public Mesh Mesh
        {
            get;
        }
    }
}