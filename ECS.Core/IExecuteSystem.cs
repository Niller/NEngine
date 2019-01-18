namespace ECS
{
    public interface IInitializeSystem : ISystem
    {
        void Execute();
    }

    public interface IExecuteSystem : ISystem
    {
        void Execute();
    }
}