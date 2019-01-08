namespace ECS
{
    public class Entity
    {
        private readonly Context _currentContext;

        internal Entity(Context currentContext)
        {
            _currentContext = currentContext;
        }

        
        public T GetComponent<T>()
        {
            return (T)_currentContext.GetComponent(this, typeof(T));
        }
        
    }
}