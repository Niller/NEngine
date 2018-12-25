using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Math.Vectors
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public struct Vector3
    {
        public double X;
        public double Y;
        public double Z;
        private readonly int _hashCode;

        public Vector3(double x, double y, double z)
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
            return string.Format(new CultureInfo("en-EN"), "({0:0.###}, {1:0.###}, {2:0.###})", X, Y, Z);
        }
    }
}