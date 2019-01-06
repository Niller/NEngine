using System;
using Math.Vectors;

namespace Math.Matrices
{
    public struct Matrix4X4
    {
        private readonly Matrix _innerMatrix;

        public Matrix4X4(params float[] values)
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
            var zAxis = (cameraTarget - cameraPos).GetNormalized();
            var xAxis = vectorUp.Cross(zAxis).GetNormalized();
            var yAxis = zAxis.Cross(xAxis);
            var m = new Matrix4X4(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                -xAxis.Dot(cameraPos), -yAxis.Dot(cameraPos), -zAxis.Dot(cameraPos), 1);

            return m;
        }

        public override string ToString()
        {
            return _innerMatrix.ToString();
        }
    }
}