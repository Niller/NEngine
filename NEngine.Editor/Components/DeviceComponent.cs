using System;
using System.Windows.Media.Imaging;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct DeviceComponent
    {
        public DeviceComponent(Vector2Int resolution, WriteableBitmap bmp)
        {
            Resolution = resolution;
            BackBuffer = new byte[resolution.X * resolution.Y * 4];

            ClearDepthBuffer = new float[resolution.X * resolution.Y];
            for (var i = 0; i < ClearDepthBuffer.Length; ++i)
            {
                ClearDepthBuffer[i] = float.MaxValue;
            }

            DepthBuffer = new float[resolution.X * resolution.Y];
            Bmp = bmp;
        }

        public Vector2Int Resolution
        {
            get;
        }

        public byte[] BackBuffer
        {
            get;
        }

        public float[] DepthBuffer
        {
            get;
        }

        public float[] ClearDepthBuffer
        {
            get;
        }

        public WriteableBitmap Bmp
        {
            get;
        }
        
    }
}