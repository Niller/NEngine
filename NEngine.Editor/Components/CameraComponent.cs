using ECS;
using Math.Vectors;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct CameraComponent
    {
        public Vector3 Position;
        public Vector3 Target;
    }
}