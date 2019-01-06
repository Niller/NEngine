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

        private readonly float[] _values;
        private readonly int _hashCode;

        public Vector(params float[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("Array is null or empty");
            }

            _values = values;
            Length = values.Length;

            _hashCode = _values[0].GetHashCode();
            for (int i = 1; i < Length; ++i)
            {
                _hashCode ^= _values[i].GetHashCode();
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetValue(int index)
        {
            return _values[index];
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector GetNormalized()
        {
            var magnitude = GetMagnitude();

            var newVectorValues = new float[_values.Length];
            for (var i = 0; i < _values.Length; ++i)
            {
                newVectorValues[i] = GetValue(i) / magnitude;
            }

            return new Vector(newVectorValues);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMagnitude()
        {
            var magnitude = 0f;
            for (var i = 0; i < _values.Length; ++i)
            {
                var value = GetValue(i);
                magnitude += value * value;
            }

            return (float)System.Math.Sqrt(magnitude);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector GetReverse()
        {
            var newVectorValues = new float[_values.Length];
            for (var i = 0; i < _values.Length; ++i)
            {
                newVectorValues[i] = -GetValue(i);
            }

            return new Vector(newVectorValues);
        }

        public static Vector Add(Vector vector1, Vector vector2)
        {
            var length = vector1._values.Length;

            if (length != vector2.Length)
            {
                throw new ArgumentException("Vectors must have equal length");
            }

            var newVectorValues = new float[length];
            for (var i = 0; i < length; ++i)
            {
                newVectorValues[i] = vector1.GetValue(i) + vector2.GetValue(i);
            }

            return new Vector(newVectorValues);
        }

        public static Vector Sub(Vector vector1, Vector vector2)
        {
            return Add(vector1, vector2.GetReverse());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return Add(vector1, vector2);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return Sub(vector1, vector2);
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
                if (_values[i] != other._values[i])
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
            stringBuilder.Append(_values[0].ToString("0.##", new CultureInfo("en-EN")));
            for (int i = 1; i < Length; ++i)
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(_values[i].ToString("0.##", new CultureInfo("en-EN")));
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}