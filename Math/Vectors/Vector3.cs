using System;
using System.Globalization;

namespace Math.Vectors
{
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

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                return _hashCode == ((Vector3)obj).GetHashCode();
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format(new CultureInfo("en-EN"), "({0:0.###}, {1:0.###}, {2:0.###})", X, Y, Z);
        }
    }
}