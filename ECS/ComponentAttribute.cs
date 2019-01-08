using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;

namespace ECS
{

    public static class ECS
    {
        private static Dictionary<string, Context> _contexts = new Dictionary<string, Context>();

        public static Context GetContext(string contextName)
        {
            Context context;
            if (!_contexts.TryGetValue(contextName, out context))
            {
                throw new Exception($"Context with name {contextName} doesn't exist");
            }
            return context;
        }
    }

    public class Context
    {
        private Dictionary<Type, List<Entity>> _componentTypes = new Dictionary<Type, List<Entity>>();
        private readonly Dictionary<Entity, Dictionary<Type, List<object>>> _entities = new Dictionary<Entity, Dictionary<Type, List<object>>>();

        public Entity AddEntity()
        {
            var entity = new Entity(this);
            _entities.Add(entity, new Dictionary<Type, List<object>>());
            return entity;
        }

        internal object GetComponent(Entity entity, Type type)
        {
            Dictionary<Type, List<object>> componentsByType;
            if (_entities.TryGetValue(entity, out componentsByType))
            {
                List<object> components;
                if (componentsByType.TryGetValue(type, out components))
                {
                    return components.FirstOrDefault();
                }
            }
            return null;
        }

    }

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

    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public string Context;

        public ComponentAttribute(string context)
        {
            Context = context;
        }
    }
}
