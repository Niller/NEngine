using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathTests
{
    [TestClass]
    public class StructTests
    {
        private ref struct TestStruct
        {
            public int X;
            public int Y;

            public TestStruct(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        //private TestStruct _testStructs;

        [TestMethod]
        public void TestRefStructBehaviour()
        {
            //TestStruct _testStructs
        }
    }
}
