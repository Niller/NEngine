using Math.Vectors;

namespace NEngine.Rendering
{
    public struct Mesh
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; set; }
        public Triangle[] Triangles { get; set; }

        public Mesh(string name, int verticesCount, int trianglesCount)
        {
            Vertices = new Vector3[verticesCount];
            Triangles = new Triangle[trianglesCount];
            Name = name;
        }
    }
}