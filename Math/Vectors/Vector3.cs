using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Math.Matrices;

namespace Math.Vectors
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;
        private readonly int _hashCode;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            int hashCode1 = X.GetHashCode();
            int hashCode2 = Y.GetHashCode();
            int hashCode3 = Z.GetHashCode();
            _hashCode = hashCode1 ^ hashCode2 ^ hashCode3;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(Vector3 vector1, Vector3 vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector3 vector1, Vector3 vector2)
        {
            return !vector1.Equals(vector2);
        }

        public Vector3 TransformCoordinate(Matrix transformMatrix)
        {
            var vectorMatrix = new Matrix(4, 1, new []
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

        public bool Equals(Vector3 other)
        {
            if (_hashCode != other.GetHashCode())
            {
                return false;
            }

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 && Equals((Vector3)obj);
        }

        public override string ToString()
        {
            return string.Format(new CultureInfo("en-EN"), "({0:0.##}, {1:0.##}, {2:0.##})", X, Y, Z);
        }
    }
}