using System;
using System.Collections.Generic;

namespace ECS
{
    public abstract class BaseContext
    {
        protected const int CapacityStep = 100;

        private readonly Queue<int> _freeIds = new Queue<int>(50);
        private int _currentId = 0;
        private int NextId => _freeIds.Count <= 0 ? _currentId++ : _freeIds.Dequeue();

        public Entity[] Entities = new Entity[CapacityStep];

        public Entity AddEntity()
        {
            //TODO [Optimization] avoid using property
            var entityId = NextId;

            var entity = new Entity(this, entityId);

            var length = Entities.Length;
            if (length < entityId - 1)
            {
                Resize();
            }

            Entities[entity.Id] = entity;

            return entity;
        }

        public virtual void Resize()
        {
            Array.Resize(ref Entities, Entities.Length + CapacityStep);
        }

        internal abstract T GetComponent<T>(Entity entity) where T : struct;
    }
}