using System;
using System.Collections.Generic;

namespace ECS
{
    public abstract class BaseContext
    {
        private ulong _currentId = 0;
        private ulong NextId => _currentId++;

        private Dictionary<ulong, Entity> _entities = new Dictionary<ulong, Entity>();

        public Entity AddEntity()
        {
            //TODO [Optimization] avoid using property
            var entity = new Entity(this, NextId);

            _entities.Add(entity.Id, entity);
            return entity;
        }

        internal abstract T GetComponent<T>(Entity entity) where T : struct;
    }
}