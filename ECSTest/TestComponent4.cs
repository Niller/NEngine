using ECS.Experimental;

namespace ECSTest
{
    [Component(typeof(MainContext))]
    public struct TestComponent4
    {
        [NotifyPropertyChanged]
        public int NotEntityIndex
        {
            get;
            set;
        }
    }
}