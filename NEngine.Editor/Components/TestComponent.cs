using ECS;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct TestComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }

        public TestComponent(int x)
        {
            X = x;
            HasValue = true;
        }

        public int X;
    }
}
