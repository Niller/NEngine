using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Math.Vectors
{
    public struct Vector
    {
        public int Length
        {
            get;
        }

        private readonly float[] _array;
        private readonly int _hashCode;

        public Vector(params float[] array)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("Array is null or empty");
            }

            _array = array;
            Length = array.Length;

            _hashCode = _array[0].GetHashCode();
            for (int i = 1; i < Length; ++i)
            {
                _hashCode ^= _array[i].GetHashCode();
            }
        }

        [Pure]
        public float GetValue(int index)
        {
            return _array[index];
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !vector1.Equals(vector2);
        }

        [Pure]
        public bool Equals(Vector other)
        {
            if (_hashCode != other.GetHashCode() || Length != other.Length)
            {
                return false;
            }

            for (int i = 0; i < other.Length; ++i)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_array[i] != other._array[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector && Equals((Vector)obj);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            stringBuilder.Append(_array[0].ToString("0.##", new CultureInfo("en-EN")));
            for (int i = 1; i < Length; ++i)
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(_array[i].ToString("0.##", new CultureInfo("en-EN")));
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}