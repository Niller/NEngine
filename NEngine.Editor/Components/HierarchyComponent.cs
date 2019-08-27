using ECS.Experimental;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct HierarchyComponent
    {
        public HierarchyComponent(int parentEntityId)
        {
            Parent = parentEntityId;
        }

        [NotifyPropertyChanged, EntityIndex]
        public int Parent
        {
            get;
            set;
        }
    }
}