using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ECS
{
    public struct Entity
    {
        internal class EntityGenericMethods<T>
        {
            internal delegate ref T GetComponentDelegate();
            internal delegate bool HasComponentDelegate();
            internal delegate void AddComponentVoidDelegate(T component);
            internal delegate ref T AddComponentDelegate(ref T component);

            public readonly GetComponentDelegate GetComponentMethod;
            public readonly HasComponentDelegate HasComponentMethod;
            public readonly AddComponentVoidDelegate AddComponentVoidMethod;
            public readonly AddComponentDelegate AddComponentMethod;

            public EntityGenericMethods(GetComponentDelegate getComponentMethod, HasComponentDelegate hasComponentMethod, 
                AddComponentVoidDelegate addComponentVoidMethod, AddComponentDelegate addComponentMethod)
            {
                GetComponentMethod = getComponentMethod;
                HasComponentMethod = hasComponentMethod;
                AddComponentVoidMethod = addComponentVoidMethod;
                AddComponentMethod = addComponentMethod;
            }
        }

        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly Dictionary<Type, object> _genericMethods;

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

            _genericMethods = new Dictionary<Type, object>();
        }

        public bool HasComponent<T>() where T : struct
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                return ((EntityGenericMethods<T>)genericMethods).HasComponentMethod();
            }

            throw new Exception("Code injection went wrong!");
        }

        public ref T GetComponent<T>() where T : struct
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                return ref ((EntityGenericMethods<T>)genericMethods).GetComponentMethod();
            }

            throw new Exception("Code injection went wrong!");
        }

        public ref T AddComponent<T>(ref T component) where T : struct
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                return ref ((EntityGenericMethods<T>)genericMethods).AddComponentMethod(ref component);
            }

            throw new Exception("Code injection went wrong!");
        }

        public void AddComponent<T>(T component) where T : struct
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                ((EntityGenericMethods<T>)genericMethods).AddComponentVoidMethod(component);
            }

            throw new Exception("Code injection went wrong!");
        }
    }
}