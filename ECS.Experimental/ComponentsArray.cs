using System;
using System.Collections.Generic;

namespace ECS.Experimental
{
    public interface IComponentsList
    {
        void Resize(int newCapacity);
    }

    public class ComponentsArray<T> : IComponentsList where T : struct
    {
        private T[] _items;
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
            _idMapping = new int[idCapacity];

            Length = 0;
        }

        public bool GetValue(int id, ref T value)
        {
            var index = _idMapping[id];

            if (index < 0)
            {
                return false;
            }

            value = _items[index];
            return true;
        }

        public ref T GetValueUnsafe(int id)
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
            --Length;
            _idMapping[id] = -1;
        }

        private void ResizeItems()
        {
            Array.Resize(ref _items, _items.Length * 2);
        }
    }
}