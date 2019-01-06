using System;

namespace Math.Vectors
{
    public struct Vector4
    {
        private readonly Vector _innerVector;

        public float X => _innerVector.GetValue(0);
        public float Y => _innerVector.GetValue(1);
        public float Z => _innerVector.GetValue(2);
        public float W => _innerVector.GetValue(2);

        public Vector4(float x, float y, float z, float w)
        {
            _innerVector = new Vector(x, y, z, w);
        }

        public Vector4(Vector vector)
        {
            if (vector.Length != 4)
            {
                throw new ArgumentException("Length of inner vector is not equal 4");
            }

            _innerVector = vector;
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

        public static Vector4 operator +(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(Vector.Add(vector1.GetVector(), vector2.GetVector()));
        }

        public static Vector4 operator -(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(Vector.Sub(vector1.GetVector(), vector2.GetVector()));
        }

        public Vector4 GetNormalized()
        {
            return new Vector4(_innerVector.GetNormalized());
        }

        public float GetMagnitude()
        {
            return _innerVector.GetMagnitude();
        }

        public Vector4 GetReverse()
        {
            return new Vector4(_innerVector.GetReverse());
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