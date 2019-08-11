using System.ComponentModel.Design.Serialization;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
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