namespace NEngine.Editor
{
    //TODO Implement IoC container
    public static class Services
    {
        public static NEngineECSManager ECS { get; }
        public static EditorContext EditorContext { get; }

        static Services()
        {
            ECS = new NEngineECSManager();
            EditorContext = new EditorContext();
        }
    }
}
