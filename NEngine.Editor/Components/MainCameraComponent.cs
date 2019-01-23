using ECS;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct MainCameraComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }
    }
}