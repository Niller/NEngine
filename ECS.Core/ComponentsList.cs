using System;
using System.Collections.Generic;

namespace ECS
{
    public sealed class ComponentsList<T> where T : struct, IComponent
    {
        private readonly Queue<int> _freeIds;

        private T[] _items;
        private int[] _idMapping;

        private int _length;
        public int Length => _length;

        public int IdCapacity
        {
            get;
            private set;
        }

        public ref T this[int index] => ref _items[index];

        public ComponentsList(int itemsCapacity, int idCapacity)
        {
            IdCapacity = idCapacity;

            _items = new T[itemsCapacity];
            _freeIds = new Queue<int>(itemsCapacity);

            _idMapping = new int[idCapacity];
        }

        public void ResizeIds(int newCapacity)
        {
            IdCapacity = newCapacity;
            Array.Resize(ref _idMapping, IdCapacity);
        }

        public void Add(ref T item, int id)
        {
            var index = _length;
            if (_freeIds.Count > 0)
            {
                index = _freeIds.Dequeue();
            }

            _items[index] = item;
            _idMapping[id] = index;
            item.HasValue = true;

            _length++;

            if (_length >= _items.Length)
            {
                ResizeItems();
            }
        }

        public void Remove(int id)
        {
            _items[_idMapping[id]].HasValue = false;
        }

        private void ResizeItems()
        {
            Array.Resize(ref _items, _items.Length*2);
        }
    }
}