using System;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;
using ECS;
using ECS.Experimental;
using Math.Matrices;
using Math.Vectors;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Systems
{
    public class RenderSystem : IExecuteSystem
    {
        private Color _currentColor;

        public void Execute()
        {
            var context = Services.ECS.GetContext<MainContext>();

            Entity mainCameraEntity = new Entity();
            if (!context.TryGetEntity<MainCameraComponent>(ref mainCameraEntity))
            {
                return;
            }

            var cameraComponent = new CameraComponent();
            if (!mainCameraEntity.TryGetComponent(ref cameraComponent))
            {
                return;
            }

            var deviceEntity = new Entity();
            if (!context.TryGetEntity<DeviceComponent>(ref deviceEntity))
            {
                return;
            }

            var deviceComponent = deviceEntity.GetComponent<DeviceComponent>();

            deviceComponent.Bmp.Lock();
            Clear(ref deviceComponent, 0, 0, 0, 255);

            var viewMatrix = Matrix4X4.GetLookAtLeftHandedMatrix(cameraComponent.Position, cameraComponent.Target, Vector3.Up);
            var projectionMatrix = Matrix4X4.GetPerspectiveFovRightHandedMatrix(0.78f,
                (float)deviceComponent.Bmp.PixelWidth / deviceComponent.Bmp.PixelHeight,
                0.01f, 1.0f);

            var emptyTransformComponent = new TransformComponent();
            foreach (var entityId in context.GetAllEntities<MeshRendererComponent>())
            {
                var entity = context.GetEntity(entityId);

                var transform = emptyTransformComponent;
                if (!entity.TryGetComponent(ref transform))
                {
                    continue;
                }

                var mesh = entity.GetComponent<MeshRendererComponent>().Mesh;

                // Beware to apply rotation before translation 
                var worldMatrix = Matrix4X4.GetScalingMatrix(transform.Scale) * Matrix4X4.GetRotationYawPitchRollMatrix(transform.Rotation.Y,
                                      transform.Rotation.X, transform.Rotation.Z) *
                                  Matrix4X4.GetTranslationMatrix(transform.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                int index = 0;
                foreach (var triangle in mesh.Triangles)
                {
                    var vertexA = mesh.Vertices[triangle.A];
                    var vertexB = mesh.Vertices[triangle.B];
                    var vertexC = mesh.Vertices[triangle.C];

                    var pixelA = Project(ref deviceComponent, vertexA, transformMatrix);
                    var pixelB = Project(ref deviceComponent, vertexB, transformMatrix);
                    var pixelC = Project(ref deviceComponent, vertexC, transformMatrix);

                    _currentColor = Colors.Blue + Colors.Gray * ((float)(index++)/ mesh.Triangles.Length);
                    //continue;
                    if (!(pixelA.Y == pixelB.Y && pixelA.Y == pixelC.Y))
                    {
                        DrawTriangle(ref deviceComponent, pixelA, pixelB, pixelC);
                    }
                    else
                    {
                        //DrawBLine(ref deviceComponent, pixelA, pixelB);
                        //DrawBLine(ref deviceComponent, pixelB, pixelC);
                        //DrawBLine(ref deviceComponent, pixelC, pixelA);
                    }

                    _currentColor = Colors.Red;
                    DrawBLine(ref deviceComponent, pixelA, pixelB);
                    DrawBLine(ref deviceComponent, pixelB, pixelC);
                    DrawBLine(ref deviceComponent, pixelC, pixelA);
                }

            }

            Present(ref deviceComponent);

            deviceComponent.Bmp.AddDirtyRect(new Int32Rect(0, 0,
                deviceComponent.Bmp.PixelWidth, deviceComponent.Bmp.PixelHeight));
            deviceComponent.Bmp.Unlock();
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        private Vector2Int Project(ref DeviceComponent deviceComponent, Vector3 coord, Matrix4X4 transMat)
        {
            // transforming the coordinates
            var point = coord.TransformCoordinate(transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * deviceComponent.Bmp.PixelWidth + deviceComponent.Bmp.PixelWidth / 2f;
            var y = -point.Y * deviceComponent.Bmp.PixelHeight + deviceComponent.Bmp.PixelHeight / 2f;
            return new Vector2Int((int)x, (int)y);
        }

        //Draw Line by Bresenham algorithm
        private void DrawBLine(ref DeviceComponent deviceComponent, Vector2Int p0, Vector2Int p1)
        {

            var dx = System.Math.Abs(p1.X - p0.X);
            var dy = System.Math.Abs(p1.Y - p0.Y);
            var sx = (p0.X < p1.X) ? 1 : -1;
            var sy = (p0.Y < p1.Y) ? 1 : -1;
            var err = dx - dy;

            var x = p0.X;
            var y = p0.Y;

            while (true)
            {
                DrawPoint(ref deviceComponent, new Vector2Int(x, y));

                if (x == p1.X && y == p1.Y) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x += sx; }
                if (e2 < dx) { err += dx; y += sy; }
            }
        }

        //Simple draw line (use for align axis lines)
        private void DrawLine(ref DeviceComponent deviceComponent, Vector2Int p0, Vector2Int p1)
        {
            Vector2Int delta;
            int len;

            if (p0.X == p1.X)
            {
                var diff = p1.Y - p0.Y;
                var sign = System.Math.Sign(diff);
                delta = new Vector2Int(0, sign);
                len = diff * sign;
            }
            else if (p0.Y == p1.Y)
            {
                var diff = p1.X - p0.X;
                var sign = System.Math.Sign(diff);
                delta = new Vector2Int(sign, 0);
                len = diff * sign;
            }
            else
            {
                throw new ArgumentException($"Points {p0} and {p1} doesn't have common axis value");
            }

            for (var i = 0; i < len; ++i)
            {
                DrawPoint(ref deviceComponent, p0 + new Vector2Int(delta.X * i, delta.Y * i));
            }
        }

        private void DrawPoint(ref DeviceComponent deviceComponent, Vector2Int point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < deviceComponent.Bmp.PixelWidth && point.Y < deviceComponent.Bmp.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel(ref deviceComponent, point, _currentColor);
            }
        }

        private void PutPixel(ref DeviceComponent deviceComponent, Vector2Int p, Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (p.X + p.Y * (int)deviceComponent.Resolution.X) * 4;

            deviceComponent.BackBuffer[index] = color.B;
            deviceComponent.BackBuffer[index + 1] = color.G;
            deviceComponent.BackBuffer[index + 2] = color.R;
            deviceComponent.BackBuffer[index + 3] = color.A;
        }

        private void Clear(ref DeviceComponent deviceComponent, byte r, byte g, byte b, byte a)
        {
            for (var index = 0; index < deviceComponent.BackBuffer.Length; index += 4)
            {
                // BGRA is used by Windows
                deviceComponent.BackBuffer[index] = b;
                deviceComponent.BackBuffer[index + 1] = g;
                deviceComponent.BackBuffer[index + 2] = r;
                deviceComponent.BackBuffer[index + 3] = a;
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 
        private void Present(ref DeviceComponent deviceComponent)
        {
            unsafe
            {
                byte* backBuffer = (byte*)deviceComponent.Bmp.BackBuffer;
                for (int i = 0, len = deviceComponent.BackBuffer.Length; i < len; ++i)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    backBuffer[i] = deviceComponent.BackBuffer[i];
                }
            }
        }

        private void FillBottomFlatTriangle(ref DeviceComponent deviceComponent, Vector2Int v1, Vector2Int v2, Vector2Int v3)
        {
            var invSlope1 = (float)(v2.X - v1.X) / (v2.Y - v1.Y);
            var invSlope2 = (float)(v3.X - v1.X) / (v3.Y - v1.Y);

            float curX1 = v1.X;
            float curX2 = v1.X;

            for (var scanLineY = v1.Y; scanLineY <= v2.Y; ++scanLineY)
            {
                DrawBLine(ref deviceComponent, new Vector2Int((int)curX1, scanLineY), new Vector2Int((int)curX2, scanLineY));
                curX1 += invSlope1;
                curX2 += invSlope2;
            }
        }

        private void FillTopFlatTriangle(ref DeviceComponent deviceComponent, Vector2Int v1, Vector2Int v2, Vector2Int v3)
        {
            var invSlope1 = (float)(v3.X - v1.X) / (v3.Y - v1.Y);
            var invSlope2 = (float)(v3.X - v2.X) / (v3.Y - v2.Y);

            float curX1 = v3.X;
            float curX2 = v3.X;

            for (var scanLineY = v3.Y; scanLineY > v1.Y; --scanLineY)
            {
                DrawBLine(ref deviceComponent, new Vector2Int((int)curX1, scanLineY), new Vector2Int((int)curX2, scanLineY));
                curX1 -= invSlope1;
                curX2 -= invSlope2;
            }
        }

        
        private void DrawTriangle(ref DeviceComponent deviceComponent, Vector2Int p1, Vector2Int p2, Vector2Int p3)
        {
            // at first sort the three vertices by y-coordinate ascending so p1 is the topmost vertice 
            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            // here we know that p1.y <= p2.y <= p3.y 
            // check for trivial case of bottom-flat triangle 
            if (p2.Y == p3.Y)
            {
                FillBottomFlatTriangle(ref deviceComponent, p1, p2, p3);
            }
            // check for trivial case of top-flat triangle 
            else if (p1.Y == p2.Y)
            {
                FillTopFlatTriangle(ref deviceComponent, p1, p2, p3);
            }
            else
            {
                // general case - split the triangle in a topflat and bottom-flat one 
                var v4 = new Vector2Int(
                    (int)((float)p1.X + ((float)(p2.Y - p1.Y) / (float)(p3.Y - p1.Y)) * (float)(p3.X - p1.X)), p2.Y);
                FillBottomFlatTriangle(ref deviceComponent, p1, p2, v4);
                FillTopFlatTriangle(ref deviceComponent, p2, v4, p3);
            }
        }
        
    }
}