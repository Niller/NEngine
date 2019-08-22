using ECS.Experimental;

namespace ECSTest
{
    [Component(typeof(MainContext))]
    public struct TestComponent5
    {
        [NotifyPropertyChanged]
        public string StringId
        {
            get;
            set;
        }
    }
}