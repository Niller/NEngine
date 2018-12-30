namespace Math.Vectors
{
    public struct Vector2
    {
        private readonly Vector _innerVector;

        public float X => _innerVector.GetValue(0);
        public float Y => _innerVector.GetValue(1);

        public Vector2(float x, float y)
        {
            _innerVector = new Vector(x, y);
        }

        public Vector GetVector()
        {
            return _innerVector;
        }

        public override int GetHashCode()
        {
            return _innerVector.GetHashCode();
        }

        public static bool operator ==(Vector2 vector1, Vector2 vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector2 vector1, Vector2 vector2)
        {
            return !vector1.Equals(vector2);
        }

        public bool Equals(Vector2 other)
        {
            return _innerVector.Equals(other._innerVector);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 && Equals((Vector2)obj);
        }

        public override string ToString()
        {
            return _innerVector.ToString();
        }
    }
}