using ECS;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct MainCameraComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }
    }
}