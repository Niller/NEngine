using System;
using System.Globalization;
using System.Text;
using Math.Utilities;
using Math.Vectors;

namespace Math.Matrices
{
    public struct Matrix
    {
        public int Rows;
        public int Columns;

        private readonly float[] _values;
        private readonly int _hashCode;

        public Matrix(int x, int y, params float[] values)
        {
            if (values.Length <= 0)
            {
                throw new ArgumentException("Matrix must not be empty");
            }

            Columns = x;
            Rows = y;

            _values = (float[])values.Clone();

            _hashCode = _values.GetHashCode();
        }

        public float GetValue(int x, int y)
        {
            if (!MathUtilities.IsInRange(x, 0, Columns - 1) || !MathUtilities.IsInRange(y, 0, Rows - 1))
            {
                throw new IndexOutOfRangeException($"Index {new Vector2(x, y).ToString()} is out of the matrix dimension");
            }

            return _values[x * Rows + y];
        }

        public float GetValue(int i)
        {
            return _values[i];
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

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return Mul(matrix1, matrix2);
        }

        private static Matrix Mul(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Columns)
            {
                throw new ArgumentException("Number of columns in Matrix1 not equals the number of rows in Matrix2");
            }

            var array = new float[matrix1.Columns * matrix2.Rows];

            for (var i = 0; i < matrix1.Columns; ++i)
            {
                for (var j = 0; j < matrix2.Rows; ++j)
                {
                    var result = 0f;
                    for (var k = 0; k < matrix1.Rows; k++)
                    {
                        result += matrix1._values[i * matrix1.Rows + k] * matrix2._values[k * matrix2.Rows + j];
                    }
                    array[i * matrix2.Rows + j] = result;
                }
            }

            return new Matrix(matrix1.Columns, matrix2.Rows, array);
        } 

        public bool Equals(Matrix other)
        {
            if (Rows != other.Rows || Columns != other.Columns)
            {
                return false;
            }

            for (var i = 0; i < _values.Length; ++i)
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
            for (var y = 0; y < Rows; ++y)
            {
                for (var x = 0; x < Columns; ++x)
                {
                    stringBuilder.Append(_values[y*Columns + x].ToString("0.###", cultureInfo));

                    if (x < Columns - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }

                if (y < Rows - 1)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
            }
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }
    }
}
