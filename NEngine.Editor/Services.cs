using ECS;
using NEngine.Editor.Contexts;

namespace NEngine.Editor
{
    //TODO Implement IoC container
    public static class Services
    {
        public static ECSManager ECS { get; }
        public static EditorContext EditorContext { get; }

        static Services()
        {
            ECS = new ECSManager();
            ECS.AddContext<MainContext>();
            EditorContext = new EditorContext();
        }
    }
}
