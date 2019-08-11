namespace ECS
{
    public interface IInitializeSystem : ISystem
    {
        void Execute();
    }
}