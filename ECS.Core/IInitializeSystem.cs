namespace ECS
{
    public interface ICleanupSystem : ISystem
    {
        void Execute();
    }

    public interface IInitializeSystem : ISystem
    {
        void Execute();
    }
}