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

        public static Matrix4X4 GetPerspectiveFovRightHandedMatrix(float fieldOfView, float aspectRatio, float zNearPlane, float zFarPlane)
        {
            var h = 1f / (float)System.Math.Tan(fieldOfView/2f);
            var w = h / aspectRatio;

            return new Matrix4X4(
                w, 0, 0 ,0,
                0, h, 0, 0,
                0, 0, zFarPlane/(zNearPlane - zFarPlane), -1,
                0, 0, zNearPlane*zFarPlane/(zNearPlane - zFarPlane), 0);
        }

        /// <summary>
        /// Rotate by 3 axis in radians
        /// </summary>
        /// <param name="yaw">Rotate by Z</param>
        /// <param name="pitch">Rotate by Y</param>
        /// <param name="roll">Rotate by X</param>
        /// <returns></returns>
        public static Matrix4X4 GetRotationYawPitchRollMatrix(float yaw, float pitch, float roll)
        {
            var yawMatrix = new Matrix(3, 3,
                (float)System.Math.Cos(yaw), 0, -(float)System.Math.Sin(yaw),
                0, 1, 0,
                (float)System.Math.Sin(yaw), 0, (float)System.Math.Cos(yaw));

            var pitchMatrix = new Matrix(3, 3,
                1, 0, 0,
                0, (float)System.Math.Cos(pitch), (float)System.Math.Sin(pitch),
                0, -(float)System.Math.Sin(pitch), (float)System.Math.Cos(pitch));

            var rollMatrix = new Matrix(3, 3,
                (float)System.Math.Cos(roll), (float)System.Math.Sin(roll), 0,
                -(float)System.Math.Sin(roll), (float)System.Math.Cos(roll), 0,
                0, 0, 1);

            var m = rollMatrix * pitchMatrix * yawMatrix;

            return new Matrix4X4(
                m.GetValue(0), m.GetValue(1), m.GetValue(2), 0,
                m.GetValue(3), m.GetValue(4), m.GetValue(5), 0,
                m.GetValue(6), m.GetValue(7), m.GetValue(8), 0,
                0, 0, 0, 1);
        }

        public override string ToString()
        {
            return _innerMatrix.ToString();
        }
    }
}