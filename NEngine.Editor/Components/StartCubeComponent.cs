using ECS;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct StartCubeComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }
    }
}