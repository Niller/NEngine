using System;
using System.Drawing;
using System.Windows.Data;
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
        public DeviceComponent(Vector2Int resolution, WriteableBitmap bmp, Color backColor)
        {
            Resolution = resolution;
            BackBuffer = new byte[resolution.X * resolution.Y * 4];
            ClearBackBuffer = new byte[resolution.X * resolution.Y * 4];
            for (var i = 0; i < ClearBackBuffer.Length; i += 4)
            {
                ClearBackBuffer[i] = backColor.B;
                ClearBackBuffer[i + 1] = backColor.G;
                ClearBackBuffer[i + 2] = backColor.R;
                ClearBackBuffer[i + 3] = backColor.A;
            }

            ClearDepthBuffer = new float[resolution.X * resolution.Y];
            for (var i = 0; i < ClearDepthBuffer.Length; ++i)
            {
                ClearDepthBuffer[i] = float.MinValue;
            }

            BufferSyncObjects = new object[resolution.X * resolution.Y];
            for (var i = 0; i < BufferSyncObjects.Length; ++i)
            {
                BufferSyncObjects[i] = new object();
            }

            DepthBuffer = new float[resolution.X * resolution.Y];
            Bmp = bmp;
            BmpWidth = Bmp.PixelWidth;
            BmpHeight = Bmp.PixelHeight;
        }

        public Vector2Int Resolution
        {
            get;
        }

        public byte[] BackBuffer;
        public byte[] ClearBackBuffer;
        public float[] DepthBuffer;
        public float[] ClearDepthBuffer;
        public WriteableBitmap Bmp;
        public int BmpWidth;
        public int BmpHeight;
        public object[] BufferSyncObjects;

    }
}