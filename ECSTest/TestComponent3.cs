using ECS.Experimental;

namespace ECSTest
{
    [Component(typeof(MainContext))]
    public struct TestComponent3
    {
        [EntityIndex, NotifyPropertyChanged]
        public int Id1
        {
            get;
            set;
        }
    }
}