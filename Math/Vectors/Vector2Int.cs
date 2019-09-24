using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Math.Vectors
{
    public struct Vector2Int
    {
        private readonly VectorInt _innerVector;


        public int X;

        public int Y;

        public Vector2Int(int x, int y)
        {
            _innerVector = new VectorInt(x, y);
            X = x;
            Y = y;
        }

        public Vector2Int(VectorInt vector)
        {
            if (vector.Length != 2)
            {
                throw new ArgumentException("Length of inner vector is not equal 2");
            }

            _innerVector = vector;
            X = vector.GetValue(0);
            Y = vector.GetValue(1);
        }

        public VectorInt GetVector()
        {
            return _innerVector;
        }

        public override int GetHashCode()
        {
            return _innerVector.GetHashCode();
        }

        public static bool operator ==(Vector2Int vector1, Vector2Int vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector2Int vector1, Vector2Int vector2)
        {
            return !vector1.Equals(vector2);
        }

        public static Vector2Int operator +(Vector2Int vector1, Vector2Int vector2)
        {
            return new Vector2Int(VectorInt.Add(vector1.GetVector(), vector2.GetVector()));
        }

        public static Vector2Int operator -(Vector2Int vector1, Vector2Int vector2)
        {
            return new Vector2Int(VectorInt.Sub(vector1.GetVector(), vector2.GetVector()));
        }

        public static Vector2 operator /(Vector2Int vector1, float number)
        {
            return new Vector2(VectorInt.Div(vector1.GetVector(), number));
        }

        public Vector2 GetNormalized()
        {
            return new Vector2(_innerVector.GetNormalized());
        }

        public float GetMagnitude()
        {
            return _innerVector.GetMagnitude();
        }

        public Vector2Int GetReverse()
        {
            return new Vector2Int(_innerVector.GetReverse());
        }

        public bool Equals(Vector2Int other)
        {
            return _innerVector.Equals(other._innerVector);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 vector2 && Equals(vector2);
        }

        public override string ToString()
        {
            return _innerVector.ToString();
        }
    }
}