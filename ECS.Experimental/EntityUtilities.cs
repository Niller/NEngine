namespace ECS.Experimental
{
    public static class EntityUtilities
    {
        public static void AddComponent<T>(this in Entity entity, ref T component) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            componentArray.Add(entity.Id, ref component);
        }

        public static bool GetComponent<T>(this in Entity entity, ref T component) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            return componentArray.GetValue(entity.Id, ref component);
        }

        public static void RemoveComponent<T>(this in Entity entity) where T : struct
        {
            var componentArray = entity.CurrentContext.GetComponentsArray<T>();
            componentArray.Remove(entity.Id);
        }
    }
}
