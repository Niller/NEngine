﻿using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Math.Vectors
{
    public struct VectorInt
    {
        public int Length
        {
            get;
        }

        private readonly int[] _values;
        private readonly int _hashCode;

        public VectorInt(params int[] values)
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

        public VectorInt(params float[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("Array is null or empty");
            }

            Length = values.Length;
            _values = new int[Length];

            _values[0] = (int)values[0];
            _hashCode = _values[0].GetHashCode();
            for (int i = 1; i < Length; ++i)
            {
                _values[i] = (int)values[i];
                _hashCode ^= _values[i].GetHashCode();
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetValue(int index)
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
        public VectorInt GetReverse()
        {
            var newVectorValues = new int[_values.Length];
            for (var i = 0; i < _values.Length; ++i)
            {
                newVectorValues[i] = -GetValue(i);
            }

            return new VectorInt(newVectorValues);
        }

        public static VectorInt Add(VectorInt vector1, VectorInt vector2)
        {
            var length = vector1._values.Length;

            if (length != vector2.Length)
            {
                throw new ArgumentException("Vectors must have equal length");
            }

            var newVectorValues = new int[length];
            for (var i = 0; i < length; ++i)
            {
                newVectorValues[i] = vector1.GetValue(i) + vector2.GetValue(i);
            }

            return new VectorInt(newVectorValues);
        }

        public static Vector Div(VectorInt vector1, float number)
        {
            var length = vector1._values.Length;

            var newVectorValues = new float[length];
            for (var i = 0; i < length; ++i)
            {
                newVectorValues[i] = vector1.GetValue(i) / number;
            }

            return new Vector(newVectorValues);
        }

        public static VectorInt Sub(VectorInt vector1, VectorInt vector2)
        {
            return Add(vector1, vector2.GetReverse());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static VectorInt operator +(VectorInt vector1, VectorInt vector2)
        {
            return Add(vector1, vector2);
        }

        public static VectorInt operator -(VectorInt vector1, VectorInt vector2)
        {
            return Sub(vector1, vector2);
        }

        public static Vector operator /(VectorInt vector1, float number)
        {
            return Div(vector1, number);
        }

        public static bool operator ==(VectorInt vector1, VectorInt vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(VectorInt vector1, VectorInt vector2)
        {
            return !vector1.Equals(vector2);
        }

        public static explicit operator Vector(VectorInt v) => new Vector(v._values);

        [Pure]
        public bool Equals(VectorInt other)
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
            return obj is Vector vector && Equals(vector);
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