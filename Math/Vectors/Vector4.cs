using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Math.Vectors
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public struct Vector4
    {
        public double X;
        public double Y;
        public double Z;
        public double W;
        private readonly int _hashCode;

        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;

            int hashCode1 = X.GetHashCode();
            int hashCode2 = Y.GetHashCode();
            int hashCode3 = Z.GetHashCode();
            int hashCode4 = W.GetHashCode();
            _hashCode = hashCode1 ^ hashCode2 ^ hashCode3 ^ hashCode4;
        }

        public override int GetHashCode()
        {
            return _hashCode;
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
            if (_hashCode != other.GetHashCode())
            {
                return false;
            }

            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 && Equals((Vector4)obj);
        }

        public override string ToString()
        {
            return string.Format(new CultureInfo("en-EN"), "({0:0.###}, {1:0.###}, {2:0.###}, {3:0.###})", X, Y, Z, W);
        }
    }
}