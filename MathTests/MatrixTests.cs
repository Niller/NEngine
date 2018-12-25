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

            Assert.AreEqual("{1, 2, 3, 4" + Environment.NewLine + "5, 6, 7, 8}", matrix1.ToString());
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

        [TestMethod]
        public void GetValueTest()
        {
            var m = new[]
            {
                1d, 2d, 3d, 4d,
                5d, 6d, 7d, 8d,
                9d, 10d, 11d, 12d,
                13d, 14d, 15d, 16d
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
                1d, 2d, 3d, 
                4d, 5d, 6d,
                7d, 8d, 9d,

            };

            var matrix1 = new Matrix(3, 3, m);
            var matrix2 = new Matrix(3, 3, m);

            var result = matrix1 * matrix2;

            var mustResult = new Matrix(3, 3, new []
            {
                30d, 36d, 42d,
                66d, 81d, 96d,
                102d, 126d, 150d
            });

            Assert.AreEqual(mustResult, result);

            m = new[]
            {
                2d, 
            };

            matrix1 = new Matrix(1, 1, m);
            matrix2 = new Matrix(1, 1, m);

            result = matrix1 * matrix2;

            mustResult = new Matrix(1, 1, new[]
            {
                4d
            });

            Assert.AreEqual(mustResult, result);
        }
    }
}
