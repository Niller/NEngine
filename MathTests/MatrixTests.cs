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
        public void CreateMatrixTest()
        {
            var matrix1 = new Matrix(1, 3);
            var matrix2 = new Matrix(new Vector2Int(1, 3));

            Assert.AreEqual(matrix1.Dimension, matrix2.Dimension);
            Assert.AreEqual(matrix1.Dimension, new Vector2Int(1, 3));

            matrix1 = new Matrix(1, 2);
            matrix2 = new Matrix(new Vector2Int(1, 3));

            Assert.AreNotEqual(matrix1.Dimension, matrix2.Dimension);
        }
    }
}
