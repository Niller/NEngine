using System;
using System.Globalization;
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

        public Vector(float[] array)
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

        public float GetValue(int index)
        {
            return _array[index];
        }
        /*
        public Vector3 TransformCoordinate(Matrix transformMatrix)
        {
            var vectorMatrix = new Matrix(4, 1, new[]
            {
                X, Y, Z, 1f
            });
            var resultMatrix = transformMatrix * vectorMatrix;
            return new Vector3(resultMatrix.GetValue(0, 0), resultMatrix.GetValue(1, 0), resultMatrix.GetValue(2, 0));
        }
        */
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
            return obj is Vector4 && Equals((Vector4)obj);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            stringBuilder.Append(_array[0].ToString("0.##", new CultureInfo("en-EN")));
            for (int i = 1; i < Length; ++i)
            {
                stringBuilder.Append(_array[i].ToString("0.##", new CultureInfo("en-EN")));
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}