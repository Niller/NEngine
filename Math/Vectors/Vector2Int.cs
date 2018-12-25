namespace Math.Vectors
{
    public struct Vector2Int
    {
        public readonly int X;
        public readonly int Y;
        private readonly int _hashCode;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;

            int hashCode1 = X.GetHashCode();
            int hashCode2 = Y.GetHashCode();
            _hashCode = hashCode1 ^ hashCode2;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2Int)
            {
                return _hashCode == ((Vector2Int) obj).GetHashCode();
            }

            return false;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}