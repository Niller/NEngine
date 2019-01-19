using System;
using System.Collections.Generic;

namespace ECS
{
    public class TestContext : BaseContext
    {
        public TestComponent[] _TestComponents = new TestComponent[CapacityStep];

        public bool HasEntity_TestComponent<T>() where T : struct
        {
            for (var i = 0; i < CurrentEntityPoolSize; ++i)
            {
                if (_TestComponents[i].@NotNull)
                {
                    return true;
                }
            }

            return false;
        }

        public ref Entity GetEntity_TestComponent<T>() where T : struct
        {
            for (var i = 0; i < CurrentEntityPoolSize; ++i)
            {
                if (_TestComponents[i].@NotNull)
                {
                    return ref Entities[i];
                }
            }

            throw new Exception("Entity not exist");
        }

        public List<int> GetAllEntities_TestComponent<T>() where T : struct
        {
            AllEntitiesRequestPool.Clear();
            for (var i = 0; i < CurrentEntityPoolSize; ++i)
            {
                if (_TestComponents[i].NotNull)
                {
                    AllEntitiesRequestPool.Add(i);
                }
            }

            return AllEntitiesRequestPool;
        }
    }

    public struct TestComponent
    {
        public bool NotNull;
    }

    public abstract class BaseContext
    {
        protected const int CapacityStep = 32;
        
        private readonly Queue<int> _freeIds = new Queue<int>(64);
        private int _currentId = 0;
        private int NextId => _freeIds.Count <= 0 ? _currentId++ : _freeIds.Dequeue();

        protected int CurrentEntityPoolSize = CapacityStep;
        protected List<int> AllEntitiesRequestPool = new List<int>(CapacityStep);

        public Entity[] Entities = new Entity[CapacityStep];

        public ref Entity AddEntity()
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

            return ref Entities[entity.Id];
        }

        public virtual void Resize()
        {
            Array.Resize(ref Entities, Entities.Length + CapacityStep);
            AllEntitiesRequestPool.Capacity += CapacityStep;
            CurrentEntityPoolSize += CapacityStep;
        }

        public ref Entity GetEntity(int id)
        {
            return ref Entities[id];
        }

        public bool HasEntity(int id)
        {
            return !Entities[id].IsNull;
        }

        public void Test()
        {
            var l = 10;
            for (int i = 0; i < 10; ++i)
            {
            }
        }

        public bool HasEntity<T>() where T : struct
        {
            throw new Exception("You cannot use directly HasEntity method. It must be replaced by code injection!");
        }

        public ref Entity GetEntity<T>() where T : struct
        {
            throw new Exception("You cannot use directly GetEntity method. It must be replaced by code injection!");
        }

        public  IEnumerable<int> GetAllEntities<T>() where T : struct
        {
            throw new Exception("You cannot use directly GetAllEntities method. It must be replaced by code injection!");
        }
    }
}