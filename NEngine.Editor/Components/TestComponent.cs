using ECS;

namespace NEngine.Editor.Components
{
    [Component("Main")]
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
