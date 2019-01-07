using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Math.Matrices;
using Math.Vectors;
using Matrix = System.Windows.Media.Matrix;
using Vector = System.Windows.Vector;

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

                foreach (var triangle in mesh.Triangles)
                {
                    var vertexA = mesh.Vertices[triangle.A];
                    var vertexB = mesh.Vertices[triangle.B];
                    var vertexC = mesh.Vertices[triangle.C];

                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    DrawBline(pixelA, pixelB);
                    DrawBline(pixelB, pixelC);
                    DrawBline(pixelC, pixelA);
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

        public void DrawLine(Vector2 point0, Vector2 point1)
        {
            var dist = (point1 - point0).GetMagnitude();

            // If the distance between the 2 points is less than 2 pixels
            // We're exiting
            if (dist < 2)
                return;

            // Find the middle point between first & second point
            Vector2 middlePoint = point0 + (point1 - point0) / 2;
            // We draw this point on screen
            DrawPoint(middlePoint);
            // Recursive algorithm launched between first & middle point
            // and between middle & second point
            DrawLine(point0, middlePoint);
            DrawLine(middlePoint, point1);
        }

        public void DrawBline(Vector2 point0, Vector2 point1)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var dx = System.Math.Abs(x1 - x0);
            var dy = System.Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                DrawPoint(new Vector2(x0, y0));

                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }
    }
}
