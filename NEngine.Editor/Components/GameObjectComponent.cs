using System.Collections.Generic;
using ECS.Experimental;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct GameObjectComponent
    {
        public GameObjectComponent(string name)
        {
            Name = name;
        }

        //[NotifyPropertyChanged, EntityIndex]
        public string Name
        {
            get;
            set;
        }
    }
}