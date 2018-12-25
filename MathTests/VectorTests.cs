using Math.Matrices;
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
            var vector2Int1 = new Vector2Int(1, 2);
            var vector2Int2 = new Vector2Int(1, 2);

            Assert.AreEqual(vector2Int1, vector2Int2);

            vector2Int1 = new Vector2Int(1, 3);
            vector2Int2 = new Vector2Int(1, 2);

            Assert.AreNotEqual(vector2Int1, vector2Int2);

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
            var vector3 = new Vector3(0.21d, 12343.3333d, 1.4444444d);
            Assert.AreEqual("(0.21, 12343.333, 1.444)", vector3.ToString());
        }
    }
}