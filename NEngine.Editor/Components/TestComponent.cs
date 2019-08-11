using ECS;
using ECS.Experimental;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct TestComponent
    {
        public TestComponent(int x)
        {
            X = x;
        }

        public int X;
    }
}
