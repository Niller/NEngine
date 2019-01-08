using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
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
}