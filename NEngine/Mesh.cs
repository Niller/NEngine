using System.Windows;
using System.Windows.Media.Media3D;
using Math.Vectors;

namespace NEngine
{
    public struct Triangle
    {
        public int A;
        public int B;
        public int C;
    }

    public class Mesh
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; private set; }
        public Triangle[] Triangles { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Mesh(string name, int verticesCount, int trianglesCount)
        {
            Vertices = new Vector3[verticesCount];
            Triangles = new Triangle[trianglesCount];
            Name = name;
        }
    }
}