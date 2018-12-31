using System;
using Math.Vectors;

namespace Math.Matrices
{
    public struct Matrix4X4
    {
        private readonly Matrix _innerMatrix;

        public Matrix4X4(float[] values)
        {
            if (values.Length != 16)
            {
                throw new ArgumentException("Matrix must have 16 values");
            }

            _innerMatrix = new Matrix(4, 4, values);
        }



        public Matrix GetMatrix()
        {
            return _innerMatrix;
        }

        public static Matrix4X4 GetLookAtLeftHandedMatrix(Vector3 cameraPos, Vector3 cameraTarget, Vector3 vectorUp)
        {
            return default(Matrix4X4);
        }
    }
}