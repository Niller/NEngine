using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS.Experimental;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Utilities
{
    public static class GameObjectUtilities
    {
        public static ref Entity CreateGameObject<T>(string name, ref Entity entity) where T : Context
        {
            ref var gameObject = ref CreateGameObject<T>(name);
            var hierarchyComponent = new HierarchyComponent(entity.Id);
            gameObject.AddComponent(ref hierarchyComponent);
            return ref gameObject;
        }

        public static ref Entity CreateGameObject<T>(string name) where T : Context
        {
            var context = Services.ECS.GetContext<T>();
            var meshEntity = context.CreateEntity();

            var gameObject = new GameObjectComponent(name);
            meshEntity.AddComponent(ref gameObject);

            return ref context.GetEntity(meshEntity.Id);
        }
    }
}
