using System.Linq.Expressions;

namespace ECS.Experimental
{
    public static class EntityUtilities
    {
        public static void AddComponent<T>(this in Entity entity, ref T component) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();

            //TODO: Avoid casting to interface due to boxing/unboxing
            var iComponent = (IComponent) component;
            iComponent.SetContext(entity.CurrentContext);
            iComponent.SetEntityId(entity.Id);
            iComponent.SetType(typeof(T));
            iComponent.AddEntityByIndex();
            component = (T)iComponent;

            entity.CurrentContext.MarkComponentDirty(entity.Id, typeof(T));
            componentArray.Add(entity.Id, ref component);
        }

        public static ref T AddComponent<T>(this in Entity entity) where T : struct
        {
            var component = new T();
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();

            //TODO: Avoid casting to interface due to boxing/unboxing
            var iComponent = (IComponent)component;
            iComponent.SetContext(entity.CurrentContext);
            iComponent.SetEntityId(entity.Id);
            iComponent.SetType(typeof(T));
            iComponent.AddEntityByIndex();
            component = (T) iComponent;

            componentArray.Add(entity.Id, ref component);
            entity.CurrentContext.MarkComponentDirty(entity.Id, typeof(T));
            return ref componentArray.GetValue(entity.Id);
        }

        public static bool TryGetComponent<T>(this in Entity entity, ref T component) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            return componentArray.TryGetValue(entity.Id, ref component);
        }

        public static ref T GetComponent<T>(this in Entity entity) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            return ref componentArray.GetValue(entity.Id);
        }

        public static void RemoveComponent<T>(this in Entity entity) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();

            //TODO: Avoid casting to interface due to boxing
            var component = (IComponent) componentArray.GetValue(entity.Id);
            component.RemoveEntityByIndex();

            componentArray.Remove(entity.Id);
            entity.CurrentContext.MarkComponentDirty(entity.Id, typeof(T));
        }
    }
}
