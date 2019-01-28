using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECS.Hardcode.Tests
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        public int ContextIndex;

        public ComponentAttribute(int contextIndex)
        {
            ContextIndex = contextIndex;
        }
    }

    [Component((int)Contexts.TestContext1)]
    public struct TestComponent1
    {

    }

    [Component((int)Contexts.TestContext2)]
    public struct TestComponent2
    {

    }

    public abstract class BaseECSManager
    {
        protected Context[] Contexts;

        public Context GetContext(int contextIndex)
        {
            if (Contexts.Length <= contextIndex)
            {
                throw new ArgumentOutOfRangeException($"Context with index {contextIndex} doesn't exist");
            }

            return Contexts[contextIndex];
        }

    }

    public class ECSManager : BaseECSManager
    {
        //Generated
        public ECSManager()
        {
            var data = new Data();

            Contexts = new Context[2];
            Contexts[0] = new Context(data, 0);
            Contexts[1] = new Context(data, 1);
        }

    }

    public enum Contexts
    {
        TestContext1,
        TestContext2
    }

    public class Context
    {
        internal class ContextGenericMethods
        {
            internal delegate ref Entity GetEntityDelegate();

            private readonly GetEntityDelegate _getEntityMethod;

            public ContextGenericMethods(GetEntityDelegate getEntityMethod)
            {
                _getEntityMethod = getEntityMethod;
            }

            public ref Entity GetEntity()
            {
                return ref _getEntityMethod();
            }
        }

        private Data _data;
        private int _contextIndex; 

        private Dictionary<Type, ContextGenericMethods> _genericMethods = new Dictionary<Type, ContextGenericMethods>();

        public Context(Data data, int contextIndex)
        {
            _data = data;
            _contextIndex = contextIndex;

            //Generated
            _genericMethods[typeof(TestComponent1)] = new ContextGenericMethods(GetEntity_TestComponent);
        }

        public ref Entity AddEntity()
        {
            return ref _data.AddEntity(_contextIndex);
        }

        public ref Entity GetEntity<T>()
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                return ref genericMethods.GetEntity();
            }

            throw new Exception();
        }

        //Generated
        public ref Entity GetEntity_TestComponent()
        {
            return ref _data.GetEntity_TestComponent();
        }
    }

    //Generated
    public class Data
    {
        public Entity[][] _entities;

        public Entity[] _entities_context1 = new Entity[128];
        public Entity[] _entities_context2 = new Entity[128];

        public List<TestComponent1> _components_TestComponent1 = new List<TestComponent1>(128);
        public List<TestComponent2> _components_TestComponent2 = new List<TestComponent2>(128);

        public Data()
        {
            _entities = new Entity[2][];

            _entities[0] = _entities_context1;
            _entities[1] = _entities_context2;
        }

        public ref Entity AddEntity(int contextIndex)
        {
            if (_entities.Length > contextIndex)
            {
                _entities[contextIndex][33] = new Entity();
                return ref _entities[contextIndex][33];
            } 

            throw new Exception();
        }

        //Generated
        public ref Entity GetEntity_TestComponent()
        {
            throw new Exception();
        }
    }

    public struct Entity
    {
        internal class EntityGenericMethods<T>
        {
            internal delegate ref T GetComponentDelegate();

            private readonly GetComponentDelegate _getEntityMethod;

            public EntityGenericMethods(GetComponentDelegate getEntityMethod)
            {
                _getEntityMethod = getEntityMethod;
            }

            public ref T GetComponent()
            {
                return ref _getEntityMethod();
            }
        }

        private Dictionary<Type, object> _genericMethods;

        public Entity(int id)
        {
            _genericMethods = new Dictionary<Type, object>();
        }

        public ref T GetComponent<T>()
        {
            if (_genericMethods.TryGetValue(typeof(T), out var genericMethods))
            {
                return ref ((EntityGenericMethods<T>)genericMethods).GetComponent();
            }

            throw new Exception();
        }
    }

    public class TestSystem
    {
        public void Execute()
        {

        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
