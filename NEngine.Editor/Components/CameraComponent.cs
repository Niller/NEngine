using System.ComponentModel.Design.Serialization;
using ECS;
using Math.Vectors;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct CameraComponent
    {
        public CameraComponent(Vector3 position, Vector3 target)
        {
            Position = position;
            Target = target;
        }

        public Vector3 Position;
        public Vector3 Target;
    }
}