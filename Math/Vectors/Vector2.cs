using System;
using System.Globalization;

namespace Math.Vectors
{
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

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return _hashCode == ((Vector2)obj).GetHashCode();
            }

            return false;
        }


        public override string ToString()
        {
            return string.Format(new CultureInfo("en-EN"), "({0:0.###}, {1:0.###})", X, Y);
        }
    }
}