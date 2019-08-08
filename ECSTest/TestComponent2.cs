using ECS.Experimental;

namespace ECSTest
{
    [Component(typeof(MainContext))]
    public struct TestComponent2
    {
        public bool Y1;

        [NotifyPropertyChanged]
        public bool Y2
        {
            get;
            set;
        }

        [NotifyPropertyChanged]
        public bool Y3
        {
            get;
        }

        public bool Y4
        {
            get;
            set;
        }
    }
}