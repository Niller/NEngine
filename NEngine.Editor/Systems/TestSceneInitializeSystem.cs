using System.ComponentModel;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;
using NEngine.Editor.Utilities;
using NEngine.Rendering;

namespace NEngine.Editor.Systems
{
    public class TestSceneInitializeSystem : IInitializeSystem
    {
        public void Execute()
        {
            ref var root = ref GameObjectUtilities.CreateGameObject<MainContext>("Meshes");
            
            var meshEntity = GameObjectUtilities.CreateGameObject<MainContext>("Cube", ref root);

            var transform = meshEntity.CurrentContext.RegisterComponent(new TransformComponent(new Vector3(0, 0, 0f), Vector3.Zero, new Vector3(.1f, .1f, .1f)));
            meshEntity.AddComponent(ref transform);

            /*
            var mesh = new Mesh("Cube", 8, 12)
            {
                Vertices =
                {
                    [0] = new Vector3(-1, 1, 1),
                    [1] = new Vector3(1, 1, 1),
                    [2] = new Vector3(-1, -1, 1),
                    [3] = new Vector3(1, -1, 1),
                    [4] = new Vector3(-1, 1, -1),
                    [5] = new Vector3(1, 1, -1),
                    [6] = new Vector3(1, -1, -1),
                    [7] = new Vector3(-1, -1, -1)
                },
                Triangles =
                {
                    [0] = new Triangle {A = 0, B = 1, C = 2},
                    [1] = new Triangle {A = 1, B = 2, C = 3},
                    [2] = new Triangle {A = 1, B = 3, C = 6},
                    [3] = new Triangle {A = 1, B = 5, C = 6},
                    [4] = new Triangle {A = 0, B = 1, C = 4},
                    [5] = new Triangle {A = 1, B = 4, C = 5},
                    [6] = new Triangle {A = 2, B = 3, C = 7},
                    [7] = new Triangle {A = 3, B = 6, C = 7},
                    [8] = new Triangle {A = 0, B = 2, C = 7},
                    [9] = new Triangle {A = 0, B = 4, C = 7},
                    [10] = new Triangle {A = 4, B = 5, C = 6},
                    [11] = new Triangle {A = 4, B = 6, C = 7}
                }
            };
            var meshRenderer = new MeshRendererComponent(mesh);
            meshEntity.AddComponent(ref meshRenderer);
            */

            var assetComponent = meshEntity.CurrentContext.RegisterComponent(new AssetComponent("E:\\projects\\NEngineResources\\Glock.fbx"));
            meshEntity.AddComponent(ref assetComponent);

            ref var cameraEntity = ref GameObjectUtilities.CreateGameObject<MainContext>("Camera");

            var cameraComponent = cameraEntity.CurrentContext.RegisterComponent(new CameraComponent(new Vector3(0, 0, 10.0f), Vector3.Zero));
            cameraEntity.AddComponent(ref cameraComponent);
            cameraEntity.AddComponent<MainCameraComponent>();
        }
    }
}