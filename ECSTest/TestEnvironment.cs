using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS.Experimental;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECSTest
{
    public class TestEnvironment
    {
        private Context _context;
        public TestEnvironment()
        {
            GetEntityByIndexTest1();

            Console.ReadKey();
        }

        private void GetEntityByIndexTest1()
        {
            _context = new Context();

            CreateEntityTestComponent2(1);
            CreateEntityTestComponent2(2);
            CreateEntityTestComponent2(3);
            CreateEntityTestComponent2(2);
            CreateEntityTestComponent2(1);
            CreateEntityTestComponent2(1);

            var result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(1))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "0 4 5 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(2))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "1 3 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(3))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "2 ");
        }

        private void GetEntityByIndexTest2()
        {
            _context = new Context();

            CreateEntityTestComponent2And5(1, "1");
            CreateEntityTestComponent2And5(2, "2");
            CreateEntityTestComponent2And5(3, "3");
            CreateEntityTestComponent2And5(2, "2");
            CreateEntityTestComponent2And5(1, "1");
            CreateEntityTestComponent2And5(1, "1");

            var result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(1))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "0 4 5 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(2))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "1 3 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent5>("3"))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "2 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent5>("1"))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "0 4 5 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent5>("2"))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "1 3 ");

            result = string.Empty;
            foreach (var entityId in _context.GetEntitiesByIndex<TestComponent2>(3))
            {
                result += entityId + " ";
            }
            Assert.AreEqual(result, "2 ");
        }

        private int CreateEntityTestComponent2(int id)
        {
            var entity = _context.CreateEntity();
            ref var testComponent2 = ref entity.AddComponent<TestComponent2>();
            testComponent2.Id = id;

            return entity.Id;
        }

        private int CreateEntityTestComponent2And5(int id, string stringId)
        {
            var entity = _context.CreateEntity();
            ref var testComponent2 = ref entity.AddComponent<TestComponent2>();
            testComponent2.Id = id;
            ref var testComponent5 = ref entity.AddComponent<TestComponent5>();
            testComponent5.StringId = stringId;

            return entity.Id;
        }
    }
}
