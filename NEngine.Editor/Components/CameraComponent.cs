using System.ComponentModel.Design.Serialization;
using ECS;
using Math.Vectors;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct CameraComponent : IComponent
    {
        public bool HasValue
        {
            get; set;
        }

        public CameraComponent(Vector3 position, Vector3 target)
        {
            Position = position;
            Target = target;
            HasValue = true;
        }

        public Vector3 Position;
        public Vector3 Target;
    }
}