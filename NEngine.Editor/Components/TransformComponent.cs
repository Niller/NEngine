using ECS;
using Math.Vectors;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct TransformComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }

        public TransformComponent(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            HasValue = true;
        }

        public TransformComponent(Vector3 scale)
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = scale;
            HasValue = true;
        }

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}