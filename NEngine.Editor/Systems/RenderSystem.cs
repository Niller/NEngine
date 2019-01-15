using ECS;
using Math.Matrices;
using Math.Vectors;
using NEngine.Editor.Components;

namespace NEngine.Editor.Systems
{
    public class RenderSystem : IExecuteSystem
    {
        public void Execute()
        {
            var context = Services.ECS.GetContext("Main");

            if (!context.HasEntity<MainCameraComponent>())
            {
                return;
            }
            
            var mainCameraEntity = context.GetEntity<MainCameraComponent>();

            if (!mainCameraEntity.HasComponent<CameraComponent>())
            {
                return;
            }

            var cameraComponent = mainCameraEntity.GetComponent<CameraComponent>();

            if (!context.HasEntity<DeviceComponent>())
            {
                return;
            }

            var deviceEntity = context.GetEntity<DeviceComponent>();
            var deviceComponent = deviceEntity.GetComponent<DeviceComponent>();

            var viewMatrix = Matrix4X4.GetLookAtLeftHandedMatrix(cameraComponent.Position, cameraComponent.Target, Vector3.Up);
            var projectionMatrix = Matrix4X4.GetPerspectiveFovRightHandedMatrix(0.78f,
                (float)deviceComponent.Bmp.PixelWidth / deviceComponent.Bmp.PixelHeight,
                0.01f, 1.0f);

            foreach (var entityId in context.GetAllEntities<MeshRendererComponent>())
            {
                var entity = context.GetEntity(entityId);

                if (!entity.HasComponent<TransformComponent>())
                {
                    continue;
                }

                var transform = entity.GetComponent<TransformComponent>();

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

                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    DrawBLine(pixelA, pixelB);
                    DrawBLine(pixelB, pixelC);
                    DrawBLine(pixelC, pixelA);
                }

            }
        }
    }
}