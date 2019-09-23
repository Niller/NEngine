using System.IO;
using Fbx;
using Math.Vectors;
using NEngine.Rendering;


namespace NEngine.Editor.Utilities
{
    public static class FbxUtilities
    {
        public static FbxAsset Import(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file could not be found", path);
            }

            var document = FbxIO.ReadBinary("E:\\projects\\NEngineResources\\Glock.fbx");
            var verticesNode = document.GetRelative("Objects/Geometry/Vertices");

            var fbxAsset = new FbxAsset();
            fbxAsset.Mesh = new Mesh();

            if (verticesNode.Properties[0] is double[] vertices)
            {
                fbxAsset.Mesh.Vertices = new Vector3[vertices.Length / 3];
                for (int i = 0, j = 0, len = vertices.Length; i < len; i += 3, j++)
                {
                    var xPos = vertices[i];
                    var yPos = vertices[i + 1];
                    var zPos = vertices[i + 2];
                    fbxAsset.Mesh.Vertices[j] = new Vector3(xPos, yPos, zPos);
                }
            }

            var polygonVertexIndexNode = document.GetRelative("Objects/Geometry/PolygonVertexIndex");

            if (polygonVertexIndexNode.Properties[0] is int[] polygonVertexIndex && polygonVertexIndex.Length >= 2)
            {
                bool quadMode = polygonVertexIndex[2] >= 0;

                if (quadMode)
                {
                    fbxAsset.Mesh.Triangles = new Triangle[(polygonVertexIndex.Length / 4) * 2] ;
                    for (int i = 0, j = 0, len = polygonVertexIndex.Length; i < len; i += 4, j += 2)
                    {
                        var v1 = polygonVertexIndex[i];
                        var v2 = polygonVertexIndex[i + 1];
                        var v3 = polygonVertexIndex[i + 2];
                        var v4 = -polygonVertexIndex[i + 3] - 1;
                        fbxAsset.Mesh.Triangles[j] = new Triangle(v1, v2, v4);
                        fbxAsset.Mesh.Triangles[j + 1] = new Triangle(v4, v2, v3);
                    }
                }
                else
                {
                    fbxAsset.Mesh.Triangles = new Triangle[polygonVertexIndex.Length / 3];
                    for (int i = 0, j = 0, len = polygonVertexIndex.Length; i < len; i += 3, j++)
                    {
                        var v1 = polygonVertexIndex[i];
                        var v2 = polygonVertexIndex[i + 1];
                        var v3 = -polygonVertexIndex[i + 2] - 1;
                        fbxAsset.Mesh.Triangles[j] = new Triangle(v1, v2, v3);
                    }
                }
            }

            return fbxAsset;
        }
    }
}