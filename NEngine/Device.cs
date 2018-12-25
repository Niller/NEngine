﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Math.Vectors;

namespace NEngine
{
    public class Device
    {
        private Vector2Int _resolution;
        private byte[] _backBuffer;
        private WriteableBitmap _bmp;

        public Device(Vector2Int resolution, WriteableBitmap bmp)
        {
            _resolution = resolution;
            _bmp = bmp;
            _backBuffer = new byte[resolution.X * resolution.Y];
            
        }

        // This method is called to clear the back buffer with a specific color
        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (var index = 0; index < _backBuffer.Length; index += 4)
            {
                // BGRA is used by Windows
                _backBuffer[index] = b;
                _backBuffer[index + 1] = g;
                _backBuffer[index + 2] = r;
                _backBuffer[index + 3] = a;
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 
        public void Present()
        {
            unsafe
            {
                byte* backBuffer = (byte*)_bmp.BackBuffer;
                for (int i = 0; i < _backBuffer.Length; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    backBuffer[i] = _backBuffer[i];
                }
            }

            _bmp.AddDirtyRect(new Int32Rect(0, 0,
                _bmp.PixelWidth, _bmp.PixelHeight));
            _bmp.Unlock();
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * _resolution.X) * 4;

            _backBuffer[index] = color.B;
            _backBuffer[index + 1] = color.G;
            _backBuffer[index + 2] = color.R;
            _backBuffer[index + 3] = color.A;
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        public Vector2 Project(Vector3 coord, Math.Matrices.Matrix transMat)
        {
            // transforming the coordinates
            var point = coord.TransformCoordinate(transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * _bmp.PixelWidth + _bmp.PixelWidth / 2.0f;
            var y = -point.Y * _bmp.PixelHeight + _bmp.PixelHeight / 2.0f;
            return (new Vector2(x, y));
        }

        public void Render()
        {

        }
    }
}
