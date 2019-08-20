using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace ECS.Experimental
{
    public class Context
    {
        private const int BaseEntityCount = 50;

        private int _entitiesCount;
        private int _currentEntitiesCapacity = BaseEntityCount;
        private readonly Entity _nullEntity = new Entity();

        private readonly Queue<int> _freeIds = new Queue<int>(BaseEntityCount);

        private Entity[] _entities = new Entity[BaseEntityCount];

        internal Dictionary<Type, IComponentsList> ComponentsArrays = new Dictionary<Type, IComponentsList>();
        internal Dictionary<Type, List<int>> Changes = new Dictionary<Type, List<int>>();

        // ReSharper disable once CollectionNeverUpdated.Local
        //TODO Avoid using HashSet
        private readonly Dictionary<IndexKey, HashSet<int>> _entitiesByIndex = new Dictionary<IndexKey, HashSet<int>>();

        public ComponentsArray<T> GetComponentsArray<T>() where T : struct 
        {
            var type = typeof(T);
            if (ComponentsArrays.TryGetValue(type, out var componentsList))
            {
                return (ComponentsArray<T>)componentsList;
            }

            var newArray = new ComponentsArray<T>(10, BaseEntityCount);
            ComponentsArrays[type] = newArray;
            return newArray;
        }

        public void AddEntityWithIndex(Type componentType, Type indexType, int valueHashCode, int entityId)
        {
            if (_entitiesByIndex.TryGetValue(new IndexKey(componentType, indexType, valueHashCode), out var entities))
            {
                entities.Add(entityId);
            }
        }

        public void RemoveEntityWithIndex(Type componentType, Type indexType, int valueHashCode, int entityId)
        {
            if (_entitiesByIndex.TryGetValue(new IndexKey(componentType, indexType, valueHashCode), out var entities))
            {
                entities.Remove(entityId);
            }
        }

        public void MarkComponentDirty(int entityId, Type type)
        {
            if (!Changes.TryGetValue(type, out var list))
            {
                Changes[type] = list = new List<int>(BaseEntityCount);
            }

            list.Add(entityId);
        }

        public void ClearChanges()
        {
            foreach (var change in Changes)
            {
                change.Value.Clear();
            }
        }

        public Entity CreateEntity()
        {
            var newId = _freeIds.Count > 0 ? _freeIds.Dequeue() : _entitiesCount++;

            if (newId >= _currentEntitiesCapacity)
            {
                _currentEntitiesCapacity *= 2;
                Array.Resize(ref _entities, _currentEntitiesCapacity);

                foreach (var array in ComponentsArrays.Values)
                {
                    array.Resize(_currentEntitiesCapacity);
                }
            }

            var newEntity = new Entity(newId, this);
            _entities[newId] = newEntity;

            return newEntity;
        }

        public void RemoveEntity(int id)
        {
            if (id >= _entitiesCount)
            {
                return;
            }

            _entities[id] = _nullEntity;
            _freeIds.Enqueue(id);
        }

        public bool TryGetEntity(int id, ref Entity entity)
        {
            if (id >= _entitiesCount)
            {
                return false;
            }

            var result = _entities[id];

            if (!result.NotNull)
            {
                return false;
            }

            entity = result;
            return true;
        }

        public ref Entity GetEntity(int id)
        {
            return ref _entities[id];
        }

        public bool TryGetEntity<T>(ref Entity entity) where T : struct
        {
            var componentArray = GetComponentsArray<T>();

            var entityId = 0;
            if (!componentArray.TryGetFirstEntity(ref entityId))
            {
                return false;
            }

            entity = _entities[entityId];
            return true;
        }

        public ref Entity GetEntity<T>() where T : struct
        {
            return ref _entities[GetComponentsArray<T>().GetFirstEntity()];
        }

        public int[] GetAllEntities<T>() where T : struct
        {
            var componentArray = GetComponentsArray<T>();
            return componentArray.GetEntityIds();
        }

        public IEnumerable<int> GetEntitiesByIndex<TComponent>(object indexValue)
        {
            //TODO Get rid of GetType()
            if (_entitiesByIndex.TryGetValue(new IndexKey(typeof(TComponent), indexValue.GetType(), indexValue.GetHashCode()), out var entities))
            {
                return entities;
            }

            return new int[0];
        }
    }
}