﻿using Math.Matrices;
using Math.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathTests
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void EqualVectorsTest()
        {
            var vector21 = new Vector2(1, 2);
            var vector22 = new Vector2(1, 2);

            Assert.AreEqual(vector21, vector22);

            vector21 = new Vector2(1, 3);
            vector22 = new Vector2(1, 2);

            Assert.AreNotEqual(vector21, vector22);

            var vector31 = new Vector3(1, 2, 3);
            var vector32 = new Vector3(1, 2, 3);

            Assert.AreEqual(vector31, vector32);

            vector31 = new Vector3(1, 3, 4);
            vector32 = new Vector3(1, 2, 5);

            Assert.AreNotEqual(vector31, vector32);
        }

        [TestMethod]
        public void VectorToStringTest()
        {
            var vector3 = new Vector3(0.21f, 12343.3333f, 1.4444444f);
            Assert.AreEqual("(0.21, 12343.33, 1.44)", vector3.ToString());
        }

        [TestMethod]
        public void TransformCoordinateTest()
        {
            var vector3 = new Vector3(1, 2, 3);
            var m = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f,
                9f, 10f, 11f, 12f,
                13f, 14f, 15f, 16f
            };
            var matrix1 = new Matrix4X4(m);

            var result = vector3.TransformCoordinate(matrix1);

            Assert.AreEqual(new Vector3(18, 46, 74), result);
        }

        [TestMethod]
        public void DotTest()
        {
            var vector31 = new Vector3(1, 2, 3);
            var vector32 = new Vector3(3, 2, 1);

            Assert.AreEqual(10, vector31.Dot(vector32));
        }

        [TestMethod]
        public void CrossTest()
        {
            var vector31 = new Vector3(1, 2, 3);
            var vector32 = new Vector3(3, 2, 1);

            Assert.AreEqual(new Vector3(-4, 8, -4), vector31.Cross(vector32));
        }

        [TestMethod]
        public void MagnitudeTest()
        {
            var vector = new Vector(-3, 4);
            Assert.AreEqual(5, vector.GetMagnitude());

            var vector3 = new Vector3(3, 4, 12);
            Assert.AreEqual(13, vector3.GetMagnitude());
        }

        [TestMethod]
        public void NormalizedTest()
        {
            var vector = new Vector(3, 4);
            Assert.AreEqual(new Vector(3f/5f, 4f/5f), vector.GetNormalized());
        }

        [TestMethod]
        public void ReversedTest()
        {
            var vector = new Vector(3, 4);
            Assert.AreEqual(new Vector(-3, -4), vector.GetReverse());
        }

        [TestMethod]
        public void AddTest()
        {
            var vector = new Vector(3, 4, 3);
            var vector1 = new Vector(1, 2, 3);

            Assert.AreEqual(new Vector(4, 6, 6), vector + vector1);
            Assert.AreEqual(new Vector(4, 6, 6), vector1 + vector);
        }

        [TestMethod]
        public void SubTest()
        {
            var vector = new Vector(3, 4, 3);
            var vector1 = new Vector(1, 2, 3);

            Assert.AreEqual(new Vector(2, 2, 0), vector - vector1);
            Assert.AreEqual(new Vector(-2, -2, 0), vector1 - vector);
        }
    }
}