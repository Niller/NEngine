using System;
using System.Data;
using Math.Matrices;

namespace Math.Vectors
{
    public struct Vector3
    {
        private readonly Vector _innerVector;

        public float X => _innerVector.GetValue(0);
        public float Y => _innerVector.GetValue(1);
        public float Z => _innerVector.GetValue(2);

        public static Vector3 Up = new Vector3(0, 1, 0);
        public static Vector3 Down = new Vector3(0, -1, 0);
        public static Vector3 Zero = new Vector3(0, 0, 0);

        public Vector3(float x, float y, float z)
        {
            _innerVector = new Vector(x, y, z);
        }

        public Vector3(Vector vector)
        {
            if (vector.Length != 3)
            {
                throw new ArgumentException("Length of inner vector is not equal 3");
            }

            _innerVector = vector;
        }

        public Vector GetVector()
        {
            return _innerVector;
        }

        public Vector3 TransformCoordinate(Matrix transformMatrix)
        {
            var vectorMatrix = new Matrix(4, 1, new[]
            {
                X, Y, Z, 1f
            });
            var resultMatrix = transformMatrix * vectorMatrix;
            return new Vector3(resultMatrix.GetValue(0, 0), resultMatrix.GetValue(1, 0), resultMatrix.GetValue(2, 0));
        }

        public float Dot(Vector3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Vector3 Cross(Vector3 v)
        {
            return new Vector3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public override int GetHashCode()
        {
            return _innerVector.GetHashCode();
        }

        public static bool operator ==(Vector3 vector1, Vector3 vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector3 vector1, Vector3 vector2)
        {
            return !vector1.Equals(vector2);
        }

        public static Vector3 operator +(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(Vector.Add(vector1.GetVector(), vector2.GetVector()));
        }

        public static Vector3 operator -(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(Vector.Sub(vector1.GetVector(), vector2.GetVector()));
        }

        public Vector3 GetNormalized()
        {
            return new Vector3(_innerVector.GetNormalized());
        }

        public float GetMagnitude()
        {
            return _innerVector.GetMagnitude();
        }

        public Vector3 GetReverse()
        {
            return new Vector3(_innerVector.GetReverse());
        }

        public bool Equals(Vector3 other)
        {
            return _innerVector.Equals(other._innerVector);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 && Equals((Vector3)obj);
        }

        public override string ToString()
        {
            return _innerVector.ToString();
        }   
    }
}