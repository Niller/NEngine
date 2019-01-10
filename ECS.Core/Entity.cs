using System.Runtime.CompilerServices;

namespace ECS
{
    public struct Entity
    {
        public ulong Id
        {
            get;
        }

        private readonly BaseContext _currentContext;

        internal Entity(BaseContext currentContext, ulong id)
        {
            Id = id;
            _currentContext = currentContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetComponent<T>() where T : struct
        {
            return _currentContext.GetComponent<T>(this);
        }
        
    }
}