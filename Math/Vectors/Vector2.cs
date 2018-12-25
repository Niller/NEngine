using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Math.Vectors
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public struct Vector2
    {
        public double X;
        public double Y;
        private readonly int _hashCode;

        public Vector2(double x, double y)
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
            if (_hashCode != other.GetHashCode())
            {
                return false;
            }

            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 && Equals((Vector2)obj);
        }


        public override string ToString()
        {
            return string.Format(new CultureInfo("en-EN"), "({0:0.###}, {1:0.###})", X, Y);
        }
    }
}