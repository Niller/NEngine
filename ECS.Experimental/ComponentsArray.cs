using System;
using System.Collections.Generic;

namespace ECS.Experimental
{
    public class ComponentsArray<T> : IComponentsList where T : struct
    {
        private T[] _items;
        private int[] _entities;
        private int[] _idMapping;

        public int Length
        {
            get;
            private set;
        }

        public int IdCapacity
        {
            get;
            private set;
        }

        public ComponentsArray(int itemsCapacity, int idCapacity)
        {
            IdCapacity = idCapacity;

            _items = new T[itemsCapacity];
            _entities = new int[itemsCapacity];
            _idMapping = new int[idCapacity];

            Length = 0;
        }

        public bool TryGetValue(int id, ref T value)
        {
            var index = _idMapping[id];

            if (index < 0)
            {
                return false;
            }

            value = _items[index];
            return true;
        }

        public bool TryGetFirstEntity(ref int value)
        {
            if (Length <= 0)
            {
                return false;
            }

            value = _entities[0];
            return true;
        }

        public ref int GetFirstEntity()
        {
            return ref _entities[0];
        }

        public ref T GetValue(int id)
        {
            var index = _idMapping[id];
            return ref _items[index];
        }

        public void Resize(int newCapacity)
        {
            IdCapacity = newCapacity;
            Array.Resize(ref _idMapping, IdCapacity);
        }

        public void Add(int id, ref T item)
        {
            var index = Length;

            _items[index] = item;
            _entities[index] = id;
            _idMapping[id] = index;

            Length++;

            if (Length >= _items.Length)
            {
                ResizeItems();
            }
        }

        public void Remove(int id)
        {
            var index = _idMapping[id];

            if (index < 0)
            {
                return;
            }

            _items[index] = _items[Length - 1];
            _entities[index] = _entities[Length - 1];

            --Length;
            _idMapping[id] = -1;
        }

        public int[] GetEntityIds()
        {
            return _entities;
        }

        private void ResizeItems()
        {
            var newLength = _items.Length * 2;
            Array.Resize(ref _items, newLength);
            Array.Resize(ref _entities, newLength);
        }
    }
}