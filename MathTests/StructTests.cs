using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathTests
{
    [TestClass]
    public class StructTests
    {
        private struct TestStruct
        {
            public int X;

            public TestStruct(int x)
            {
                X = x;
            }
        }

        private TestStruct[] _testStructs;

        [TestMethod]
        public void TestRefStructBehaviour()
        {
            _testStructs = new TestStruct[]
            {
                new TestStruct(1), 
                new TestStruct(2),
                new TestStruct(3),
            };

            GetEntity(0).X++;

            Assert.AreEqual(2, _testStructs[0].X);
        }

        private ref TestStruct GetEntity(int index)
        {
            return ref _testStructs[index];
        }

    }
}
