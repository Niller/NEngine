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
        public void Execute()
        {
            var context = Services.ECS.GetContext<MainContext>();

            Entity mainCameraEntity = new Entity();
            if (!context.TryGetEntity<MainCameraComponent>(ref mainCameraEntity))
            {
                return;
            }

            var cameraComponent = new CameraComponent();
            if (!mainCameraEntity.TryGetComponent<CameraComponent>(ref cameraComponent))
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
                var worldMatrix = Matrix4X4.GetRotationYawPitchRollMatrix(transform.Rotation.Y,
                                      transform.Rotation.X, transform.Rotation.Z) *
                                  Matrix4X4.GetTranslationMatrix(transform.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var triangle in mesh.Triangles)
                {
                    var vertexA = mesh.Vertices[triangle.A];
                    var vertexB = mesh.Vertices[triangle.B];
                    var vertexC = mesh.Vertices[triangle.C];

                    var pixelA = Project(ref deviceComponent, vertexA, transformMatrix);
                    var pixelB = Project(ref deviceComponent, vertexB, transformMatrix);
                    var pixelC = Project(ref deviceComponent, vertexC, transformMatrix);

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
        public Vector2 Project(ref DeviceComponent deviceComponent, Vector3 coord, Matrix4X4 transMat)
        {
            // transforming the coordinates
            var point = coord.TransformCoordinate(transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * deviceComponent.Bmp.PixelWidth + deviceComponent.Bmp.PixelWidth / 2.0f;
            var y = -point.Y * deviceComponent.Bmp.PixelHeight + deviceComponent.Bmp.PixelHeight / 2.0f;
            return (new Vector2(x, y));
        }

        public void DrawBLine(ref DeviceComponent deviceComponent, Vector2 point0, Vector2 point1)
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
                DrawPoint(ref deviceComponent, new Vector2(x0, y0));

                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        public void DrawPoint(ref DeviceComponent deviceComponent, Vector2 point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < deviceComponent.Bmp.PixelWidth && point.Y < deviceComponent.Bmp.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel(ref deviceComponent, (int)point.X, (int)point.Y, Colors.Yellow);
            }
        }

        public void PutPixel(ref DeviceComponent deviceComponent, int x, int y, Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * (int)deviceComponent.Resolution.X) * 4;

            deviceComponent.BackBuffer[index] = color.B;
            deviceComponent.BackBuffer[index + 1] = color.G;
            deviceComponent.BackBuffer[index + 2] = color.R;
            deviceComponent.BackBuffer[index + 3] = color.A;
        }

        public void Clear(ref DeviceComponent deviceComponent, byte r, byte g, byte b, byte a)
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
        public void Present(ref DeviceComponent deviceComponent)
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
    }
}