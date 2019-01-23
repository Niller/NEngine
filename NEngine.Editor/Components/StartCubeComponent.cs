using ECS;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct StartCubeComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }
    }
}