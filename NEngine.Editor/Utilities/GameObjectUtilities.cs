using ECS.Experimental;
using NEngine.Editor.Components;

namespace NEngine.Editor.Utilities
{
    public static class GameObjectUtilities
    {
        public static ref Entity CreateGameObject<T>(string name, ref Entity entity) where T : Context
        {
            ref var gameObject = ref CreateGameObject<T>(name);
            var hierarchyComponent = entity.CurrentContext.RegisterComponent(new HierarchyComponent(entity.Id));
            gameObject.AddComponent(ref hierarchyComponent);
            return ref gameObject;
        }

        public static ref Entity CreateGameObject<T>(string name) where T : Context
        {
            var context = Services.ECS.GetContext<T>();
            var meshEntity = context.CreateEntity();

            var gameObject = context.RegisterComponent(new GameObjectComponent(name));
            meshEntity.AddComponent(ref gameObject);

            return ref context.GetEntity(meshEntity.Id);
        }
    }
}
