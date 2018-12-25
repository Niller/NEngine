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

        public static bool operator ==(Vector2Int vector1, Vector2Int vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector2Int vector1, Vector2Int vector2)
        {
            return !vector1.Equals(vector2);
        }

        public bool Equals(Vector2Int other)
        {
            if (_hashCode != other.GetHashCode())
            {
                return false;
            }

            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int && Equals((Vector2Int)obj);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}