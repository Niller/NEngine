using System;
using System.Collections;
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

        public ref Entity GetEntity(int id)
        {
            return ref Entities[id];
        }

        public bool HasEntity(int id)
        {
            return !Entities[id].IsNull;
        }

        public abstract bool HasEntity<T>() where T : struct;

        public abstract ref Entity GetEntity<T>() where T : struct;

        public abstract IEnumerable<int> GetAllEntities<T>() where T : struct;

        internal abstract ref T GetComponent<T>(Entity entity) where T : struct;
    }
}