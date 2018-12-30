namespace Math.Vectors
{
    public struct Vector4
    {
        private Vector _innerVector;

        public float X => _innerVector.GetValue(0);
        public float Y => _innerVector.GetValue(1);
        public float Z => _innerVector.GetValue(2);
        public float W => _innerVector.GetValue(2);

        public Vector4(float x, float y, float z, float w)
        {
            _innerVector = new Vector(x, y, z, w);
        }

        public Vector GetVector()
        {
            return _innerVector;
        }

        public override int GetHashCode()
        {
            return _innerVector.GetHashCode();
        }

        public static bool operator ==(Vector4 vector1, Vector4 vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector4 vector1, Vector4 vector2)
        {
            return !vector1.Equals(vector2);
        }

        public bool Equals(Vector4 other)
        {
            return _innerVector.Equals(other._innerVector);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 && Equals((Vector4)obj);
        }

        public override string ToString()
        {
            return _innerVector.ToString();
        }
    }
}