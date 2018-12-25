using Math.Vectors;

namespace Math.Matrices
{
    public struct Matrix
    {
        public Vector2Int Dimension;

        public double[,] Values;

        public Matrix(int x, int y) : this(new Vector2Int(x, y))
        {
            
        }

        public Matrix(Vector2Int dimension)
        {
            Dimension = dimension;
            Values = new double[dimension.X, dimension.Y];
        }
    }
}
