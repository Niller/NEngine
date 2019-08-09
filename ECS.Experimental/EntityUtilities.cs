namespace ECS.Experimental
{
    public static class EntityUtilities
    {
        public static void AddComponent<T>(this in Entity entity, ref T component) where T : struct, IComponent
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();

            //TODO: Avoid casting to interface due to boxing/unboxing
            var iComponent = (IComponent) component;
            iComponent.SetContext(entity.CurrentContext);
            iComponent.SetEntityId(entity.Id);

            componentArray.Add(entity.Id, ref component);
        }

        public static bool GetComponent<T>(this in Entity entity, ref T component) where T : struct, IComponent
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            return componentArray.GetValue(entity.Id, ref component);
        }

        public static void RemoveComponent<T>(this in Entity entity) where T : struct, IComponent
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            componentArray.Remove(entity.Id);
            entity.CurrentContext.MarkComponentDirty(entity.Id, typeof(T));
        }
    }
}
