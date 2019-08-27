using System.Collections.Generic;
using ECS;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Systems
{
    public class HierarchyEditorSystem : ReactiveSystem<MainContext, HierarchyComponent>
    {
        public override void Execute(List<int> entities)
        {
            
        }
    }

    public class HierarchyGameObjectEditorSystem : ReactiveSystem<MainContext, GameObjectComponent>
    {
        public override void Execute(List<int> entities)
        {
            //Services.EditorContext.HierarchyTreeView.
        }
    }
}