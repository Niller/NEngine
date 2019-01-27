using System;
using System.Collections.Generic;

namespace ECS
{
    public class TestManager : ECSManager
    {
        public TestManager() : base()
        {
            Contexts.Add(typeof(TestContext), new TestContext());
        }

    }

    public class TestSystem
    {
        void Execute()
        {
            TestManager manager = new TestManager();
            var context = manager.GetContext<TestContext>();

            var entity = context.GetEntity(0);

            var b = ((TestContext) entity.CurrentContext).HasComponent_TestComponent(ref entity);
        }
    }

    public class TestContext : BaseContext
    {
        public ComponentsList<TestComponent> _TestComponents = new ComponentsList<TestComponent>(16, 128);

        public void AddComponent_TestComponent(ref Entity entity, TestComponent testComponent)
        {
            _TestComponents.Add(ref testComponent, entity.Id);
        }

        public ref TestComponent AddComponent_TestComponent(ref Entity entity, ref TestComponent testComponent)
        {
            _TestComponents.Add(ref testComponent, entity.Id);
            return ref testComponent;
        }

        public bool HasComponent_TestComponent(ref Entity entity)
        {
            return _TestComponents[entity.Id].HasValue;
        }

        public ref TestComponent GetComponent_TestComponent(ref Entity entity)
        {
            return ref _TestComponents[entity.Id];
        }

        public bool HasEntity_TestComponent()
        {
            for (int i = 0, ilen = _TestComponents.Length; i < ilen; ++i)
            {
                if (_TestComponents[i].HasValue)
                {
                    return true;
                }
            }

            return false;
        }

        public ref Entity GetEntity_TestComponent()
        {
            for (int i = 0, ilen = _TestComponents.Length; i < ilen; ++i)
            {
                if (_TestComponents[i].HasValue)
                {
                    return ref Entities[i];
                }
            }

            return ref DefaultEntity;
        }

        public List<int> GetAllEntities_TestComponent()
        {
            AllEntitiesRequestPool.Clear();
            for (int i = 0, ilen = _TestComponents.Length; i < ilen; ++i)
            {
                if (_TestComponents[i].HasValue)
                {
                    AllEntitiesRequestPool.Add(i);
                }
            }

            return AllEntitiesRequestPool;
        }

        public override void Resize()
        {
            base.Resize();

            _TestComponents.ResizeIds(_TestComponents.Length + 100);
        }
    }

    public struct TestComponent : IComponent
    {
        public TestComponent(int x)
        {
            X = x;
            HasValue = true;
        }

        public int X;

        public bool HasValue { get; set; }
    }

    public abstract class BaseContext
    {
        protected const int CapacityStep = 32;
        
        private readonly Queue<int> _freeIds = new Queue<int>(64);
        private int _currentId = 0;
        private int NextId => _freeIds.Count <= 0 ? _currentId++ : _freeIds.Dequeue();

        protected int CurrentEntityPoolSize = CapacityStep;
        protected List<int> AllEntitiesRequestPool = new List<int>(CapacityStep);
        protected Entity DefaultEntity;

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
            return Entities[id].HasValue;
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

        public  List<int> GetAllEntities<T>() where T : struct
        {
            throw new Exception("You cannot use directly GetAllEntities method. It must be replaced by code injection!");
        }
    }
}