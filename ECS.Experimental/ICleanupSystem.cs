namespace ECS
{
    public interface ICleanupSystem : ISystem
    {
        void Execute();
    }
}