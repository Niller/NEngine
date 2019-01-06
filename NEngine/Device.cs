using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Math.Matrices;
using Math.Vectors;
using Matrix = System.Windows.Media.Matrix;

namespace NEngine
{
    public class Device
    {
        private readonly Vector2 _resolution;
        private readonly byte[] _backBuffer;
        private readonly WriteableBitmap _bmp;

        public Device(Vector2 resolution, WriteableBitmap bmp)
        {
            _resolution = resolution;
            _bmp = bmp;
            _backBuffer = new byte[(int)resolution.X * (int)resolution.Y*4];
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
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * (int)_resolution.X) * 4;

            _backBuffer[index] = color.B;
            _backBuffer[index + 1] = color.G;
            _backBuffer[index + 2] = color.R;
            _backBuffer[index + 3] = color.A;
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        public Vector2 Project(Vector3 coord, Matrix4X4 transMat)
        {
            // transforming the coordinates
            var point = coord.TransformCoordinate(transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * (_bmp.PixelWidth/8f) + _bmp.PixelWidth / 2.0f;
            var y = -point.Y * (_bmp.PixelHeight/8f) + _bmp.PixelHeight / 2.0f;
            return (new Vector2(x, y));
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            var viewMatrix = Matrix4X4.GetLookAtLeftHandedMatrix(camera.Position, camera.Target, Vector3.Up);
            var projectionMatrix = Matrix4X4.GetPerspectiveFovRightHandedMatrix(0.78f,
                (float)_bmp.PixelWidth / _bmp.PixelHeight,
                0.01f, 1.0f);

            foreach (Mesh mesh in meshes)
            {
                
                // Beware to apply rotation before translation 
                var worldMatrix = Matrix4X4.GetRotationYawPitchRollMatrix(mesh.Rotation.Y,
                                      mesh.Rotation.X, mesh.Rotation.Z) *
                                  Matrix4X4.GetTranslationMatrix(mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;
                
                foreach (var vertex in mesh.Vertices)
                {
                    // First, we project the 3D coordinates into the 2D space
                    var point = Project(vertex, transformMatrix);
                    // Then we can draw on screen
                    DrawPoint(point);
                }
                
            }
        }

        public void DrawPoint(Vector2 point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < _bmp.PixelWidth && point.Y < _bmp.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel((int)point.X, (int)point.Y, Colors.Yellow);
            }
        }
    }
}
