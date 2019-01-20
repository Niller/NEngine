using System;
using System.ComponentModel;

namespace ECS
{
    public struct Entity
    {
        public int Id
        {
            get;
        }

        public readonly BaseContext CurrentContext;

        //TODO Check performance
        public bool HasValue;

        internal Entity(BaseContext currentContext, int id)
        {
            HasValue = true;
            Id = id;
            CurrentContext = currentContext;
        }

        public bool HasComponent<T>() where T : struct
        {
            throw new Exception("You cannot use directly HasComponent method. It must be replaced by code injection!");
        }

        public ref T GetComponent<T>() where T : struct
        {
            throw new Exception("You cannot use directly GetComponent method. It must be replaced by code injection!");
        }

        public ref T AddComponent<T>(ref T component) where T : struct
        {
            throw new Exception("You cannot use directly AddComponent method. It must be replaced by code injection!");
        }

        public void AddComponent<T>(T component) where T : struct
        {
            throw new Exception("You cannot use directly AddComponent method. It must be replaced by code injection!");
        }
    }
}