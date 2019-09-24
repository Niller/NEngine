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
        private struct ScreenPoint
        {
            public Vector2Int Point;
            public float Z;

            public int X => Point.X;
            public int Y => Point.Y;

            public ScreenPoint(Vector2Int point, float z)
            {
                Point = point;
                Z = z;
            }
        }

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

                    var color = (byte)((0.25f + (index++ % mesh.Triangles.Length) * 0.75f / mesh.Triangles.Length)*255);
                    _currentColor = new Color {R = color, G = color, B = color, A = 255};
                    //_currentColor = Colors.Gray;

                    if (!(pixelA.Y == pixelB.Y && pixelA.Y == pixelC.Y))
                    {
                        DrawTriangle(ref deviceComponent, new ScreenPoint(pixelA, vertexA.Z), new ScreenPoint(pixelB, vertexB.Z), new ScreenPoint(pixelC, vertexC.Z));
                    }
                    else
                    {
                        //DrawBLine(ref deviceComponent, new ScreenPoint(pixelA, vertexA.Z), new ScreenPoint(pixelB, vertexB.Z));
                        //DrawBLine(ref deviceComponent, new ScreenPoint(pixelB, vertexB.Z), new ScreenPoint(pixelC, vertexC.Z));
                        //DrawBLine(ref deviceComponent, new ScreenPoint(pixelC, vertexC.Z), new ScreenPoint(pixelA, vertexA.Z));
                    }

                    //continue;
                    _currentColor = Colors.Red;
                    //DrawBLine(ref deviceComponent, new ScreenPoint(pixelA, vertexA.Z), new ScreenPoint(pixelB, vertexB.Z));
                    //DrawBLine(ref deviceComponent, new ScreenPoint(pixelB, vertexB.Z), new ScreenPoint(pixelC, vertexC.Z));
                    //DrawBLine(ref deviceComponent, new ScreenPoint(pixelC, vertexC.Z), new ScreenPoint(pixelA, vertexA.Z));
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
        private void DrawBLine(ref DeviceComponent deviceComponent, ScreenPoint p0, ScreenPoint p1)
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
                //TODO Add depth handling
                DrawPoint(ref deviceComponent, new ScreenPoint(new Vector2Int(x, y), float.MinValue));

                if (x == p1.X && y == p1.Y) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x += sx; }
                if (e2 < dx) { err += dx; y += sy; }
            }
        }

        //Simple draw line (use for align axis lines)
        private void DrawLine(ref DeviceComponent deviceComponent, ScreenPoint p0, ScreenPoint p1)
        {
            int deltaX = 0;
            int deltaY = 0;
            int len;

            if (p0.X == p1.X)
            {
                var diff = p1.Y - p0.Y;
                var sign = System.Math.Sign(diff);
                deltaY = sign;
                len = diff * sign;
            }
            else if (p0.Y == p1.Y)
            {
                var diff = p1.X - p0.X;
                var sign = System.Math.Sign(diff);
                deltaX = sign;
                len = diff * sign;
            }
            else
            {
                throw new ArgumentException($"Points {p0} and {p1} doesn't have common axis value");
            }

            var p0X = p0.X;
            var p0Y = p0.Y;
            for (var i = 0; i < len; ++i)
            {

                var z = Math.Utilities.MathUtilities.Lerp(p0.Z, p1.Z, (float) i / len);
                DrawPoint(ref deviceComponent, new ScreenPoint(new Vector2Int(deltaX * i + p0X, deltaY * i + p0Y), z));
            }
        }

        private void DrawPoint(ref DeviceComponent deviceComponent, ScreenPoint p)
        {
            // Clipping what's visible on screen
            if (p.Point.X >= 0 && p.Point.Y >= 0 && p.Point.X < deviceComponent.Bmp.PixelWidth && p.Point.Y < deviceComponent.Bmp.PixelHeight)
            {
                var index = (p.Point.X + p.Point.Y * deviceComponent.Resolution.X);

                if (deviceComponent.DepthBuffer[index] < p.Z)
                {
                    return; // Discard
                }

                deviceComponent.DepthBuffer[index] = p.Z;

                PutPixel(ref deviceComponent, p, _currentColor);
            }
        }

        private void PutPixel(ref DeviceComponent deviceComponent, ScreenPoint p, Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var rawIndex = (p.Point.X + p.Point.Y * deviceComponent.Resolution.X);
            var index = rawIndex * 4;

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
            Array.Copy(deviceComponent.ClearDepthBuffer, deviceComponent.DepthBuffer, deviceComponent.DepthBuffer.Length);
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

        private void FillBottomFlatTriangle(ref DeviceComponent deviceComponent, ScreenPoint v1, ScreenPoint v2, ScreenPoint v3)
        {

            var invSlope1 = (float)(v2.X - v1.X) / (v2.Y - v1.Y);
            var invSlope2 = (float)(v3.X - v1.X) / (v3.Y - v1.Y);

            float curX1 = v1.X;
            float curX2 = v1.X;

            for (var scanLineY = v1.Y; scanLineY <= v2.Y; ++scanLineY)
            {
                var z = Math.Utilities.MathUtilities.Lerp(v1.Z, v2.Z, (float)scanLineY / v2.Y);

                DrawLine(ref deviceComponent, new ScreenPoint(new Vector2Int((int)curX1, scanLineY), z), new ScreenPoint(new Vector2Int((int)curX2, scanLineY), z));
                curX1 += invSlope1;
                curX2 += invSlope2;
            }
        }

        private void FillTopFlatTriangle(ref DeviceComponent deviceComponent, ScreenPoint v1, ScreenPoint v2, ScreenPoint v3)
        {
            var invSlope1 = (float)(v3.X - v1.X) / (v3.Y - v1.Y);
            var invSlope2 = (float)(v3.X - v2.X) / (v3.Y - v2.Y);

            float curX1 = v3.X;
            float curX2 = v3.X;

            for (var scanLineY = v3.Y; scanLineY > v1.Y; --scanLineY)
            {
                var z = Math.Utilities.MathUtilities.Lerp(v3.Z, v1.Z, (float)scanLineY / v1.Y);

                DrawLine(ref deviceComponent, new ScreenPoint(new Vector2Int((int)curX1, scanLineY), z), new ScreenPoint(new Vector2Int((int)curX2, scanLineY), z));
                curX1 -= invSlope1;
                curX2 -= invSlope2;
            }
        }

        
        private void DrawTriangle(ref DeviceComponent deviceComponent, ScreenPoint p1, ScreenPoint p2, ScreenPoint p3)
        {
            // at first sort the three vertices by y-coordinate ascending so p1 is the topmost vertice 
            if (p1.Point.Y > p2.Point.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Point.Y > p3.Point.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Point.Y > p2.Point.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            // here we know that p1.y <= p2.y <= p3.y 
            // check for trivial case of bottom-flat triangle 
            if (p2.Point.Y == p3.Point.Y)
            {
                FillBottomFlatTriangle(ref deviceComponent, p1, p2, p3);
            }
            // check for trivial case of top-flat triangle 
            else if (p1.Point.Y == p2.Point.Y)
            {
                FillTopFlatTriangle(ref deviceComponent, p1, p2, p3);
            }
            else
            {
                // general case - split the triangle in a topflat and bottom-flat one 
                var splitPoint = new Vector2Int(
                    (int)(p1.Point.X + ((float)(p2.Point.Y - p1.Point.Y) / (p3.Y - p1.Y)) * (p3.X - p1.X)), p2.Y);
                var v4 = new ScreenPoint(splitPoint, Math.Utilities.MathUtilities.Lerp(p3.Z, p1.Z, 
                    (splitPoint - p3.Point).GetMagnitude()/
                    (p1.Point - p3.Point).GetMagnitude()));

                FillBottomFlatTriangle(ref deviceComponent, p1, p2, v4);
                FillTopFlatTriangle(ref deviceComponent, p2, v4, p3);
            }
        }
        
    }
}