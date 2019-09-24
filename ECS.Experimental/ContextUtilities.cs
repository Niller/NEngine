namespace ECS.Experimental
{
    public static class ContextUtilities
    {
        public static T CreateComponent<T>(this Context context) where T : struct
        {
            var component = new T();

            //TODO: Avoid casting to interface due to boxing/unboxing
            var iComponent = (IComponent)component;
            iComponent.SetContext(context);
            iComponent.SetType(typeof(T));
            iComponent.AddEntityByIndex();
            component = (T)iComponent;

            return component;
        }

        public static T RegisterComponent<T>(this Context context, T component) where T : struct
        {
            //TODO: Avoid casting to interface due to boxing/unboxing
            var iComponent = (IComponent)component;
            iComponent.SetContext(context);
            iComponent.SetType(typeof(T));
            iComponent.AddEntityByIndex();
            component = (T)iComponent;

            return component;
        }
    }
}