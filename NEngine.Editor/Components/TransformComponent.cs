using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct TransformComponent
    {
        public TransformComponent(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public TransformComponent(Vector3 scale)
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = scale;
        }

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}