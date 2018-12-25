using System;
using System.Globalization;
using System.Text;
using Math.Utilities;
using Math.Vectors;

namespace Math.Matrices
{
    public struct Matrix
    {
        public Vector2Int Dimension;

        private readonly double[] _values;
        private readonly int _hashCode;

        public Matrix(int x, int y, double[] values)
        {
            if (values.Length <= 0)
            {
                throw new ArgumentException("Matrix must not be empty");
            }

            Dimension = new Vector2Int(x, y);
            _values = (double[])values.Clone();

            _hashCode = _values.GetHashCode();
        }

        public double GetValue(int x, int y)
        {
            if (!MathUtilities.IsInRange(x, 0, Dimension.X - 1) || !MathUtilities.IsInRange(y, 0, Dimension.Y - 1))
            {
                throw new IndexOutOfRangeException("Index is out of the matrix dimension");
            }

            return _values[x + Dimension.Y + y];
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return matrix1.Equals(matrix2);
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !matrix1.Equals(matrix2);
        }

        public bool Equals(Matrix other)
        {
            if (Dimension != other.Dimension)
            {
                return false;
            }

            for (int i = 0; i < _values.Length; ++i)
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
            return obj is Matrix && Equals((Matrix)obj);
        }

        public override string ToString()
        {
            var cultureInfo = new CultureInfo("en-EN");
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{");
            for (int y = 0; y < Dimension.Y; ++y)
            {
                for (int x = 0; x < Dimension.X; ++x)
                {
                    stringBuilder.Append(_values[y*Dimension.X + x].ToString("0.###", cultureInfo));

                    if (x < Dimension.X - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }

                if (y < Dimension.Y - 1)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
            }
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }
    }
}
