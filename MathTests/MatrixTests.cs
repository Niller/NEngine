using System;
using Math.Matrices;
using SharpDX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matrix = Math.Matrices.Matrix;

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
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f
            };

            var matrix1 = new Matrix(4, 2, m);

            Assert.AreEqual("{1, 2, 3, 4" + Environment.NewLine + "5, 6, 7, 8}", matrix1.ToString());
        }

        [TestMethod]
        public void EqualMatrixTest()
        {
            var m = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f
            };

            var m1 = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f
            };

            var matrix1 = new Matrix(4, 2, m);
            var matrix2 = new Matrix(4, 2, m1);

            Assert.AreEqual(matrix2, matrix1);

            m = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f
            };

            m1 = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 74f, 8f
            };

            matrix1 = new Matrix(4, 2, m);
            matrix2 = new Matrix(4, 2, m1);

            Assert.AreNotEqual(matrix2, matrix1);
        }

        [TestMethod]
        public void GetValueTest()
        {
            var m = new[]
            {
                1f, 2f, 3f, 4f,
                5f, 6f, 7f, 8f,
                9f, 10f, 11f, 12f,
                13f, 14f, 15f, 16f
            };

            var matrix1 = new Matrix(4, 4, m);

            Assert.AreEqual(1, matrix1.GetValue(0, 0));
            Assert.AreEqual(5, matrix1.GetValue(1, 0));
            Assert.AreEqual(15, matrix1.GetValue(3, 2));
            Assert.AreEqual(16, matrix1.GetValue(3, 3));

            try
            {
                matrix1.GetValue(3, 4);
            }
            catch (IndexOutOfRangeException e)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void MulMatrixTest()
        {
            var m = new[]
            {
                1f, 2f, 3f, 
                4f, 5f, 6f,
                7f, 8f, 9f,

            };

            var matrix1 = new Matrix(3, 3, m);
            var matrix2 = new Matrix(3, 3, m);

            var result = matrix1 * matrix2;

            var mustResult = new Matrix(3, 3, new []
            {
                30f, 36f, 42f,
                66f, 81f, 96f,
                102f, 126f, 150f
            });

            Assert.AreEqual(mustResult, result);

            m = new[]
            {
                2f, 
            };

            matrix1 = new Matrix(1, 1, m);
            matrix2 = new Matrix(1, 1, m);

            result = matrix1 * matrix2;

            mustResult = new Matrix(1, 1, new[]
            {
                4f
            });

            Assert.AreEqual(mustResult, result);
        }

        

        [TestMethod]
        public void LookAtLHTest()
        {
            //SharpDX.Matrix.LookAtLH()
        }
    }
}
