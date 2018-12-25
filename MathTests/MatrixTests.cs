using System;
using Math.Matrices;
using Math.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathTests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void ToStringMatrixTest()
        {
            var m = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 7d, 8d
            };

            var matrix1 = new Matrix(4, 2, m);

            Assert.AreEqual("{1, 2, 3, 4" + System.Environment.NewLine + "5, 6, 7, 8}", matrix1.ToString());
        }

        [TestMethod]
        public void EqualMatrixTest()
        {
            var m = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 7d, 8d
            };

            var m1 = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 7d, 8d
            };

            var matrix1 = new Matrix(4, 2, m);
            var matrix2 = new Matrix(4, 2, m1);

            Assert.AreEqual(matrix2, matrix1);

            m = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 7d, 8d
            };

            m1 = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 74d, 8d
            };

            matrix1 = new Matrix(4, 2, m);
            matrix2 = new Matrix(4, 2, m1);

            Assert.AreNotEqual(matrix2, matrix1);
        }
    }
}
