using System;
using System.Collections.Generic;
using ECS;
using ECS.Experimental;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;
using NEngine.Editor.Utilities;

namespace NEngine.Editor.Systems
{
    public class LoadAssetsSystem : ReactiveSystem<MainContext, AssetComponent>
    {
        public override void Execute(List<int> entities)
        {
            var context = Services.ECS.GetContext<MainContext>();
            for (int i = 0, len = entities.Count; i < len; ++i)
            {
                ref var entity = ref context.GetEntity(entities[i]);
                ref var assetComponent = ref entity.GetComponent<AssetComponent>();
                switch (assetComponent.Type)
                {
                    case AssetType.Fbx:
                        HandleFbx(ref entity, assetComponent.Path);
                        break;
                    default:
                        continue;
                }
            }
        }

        private void HandleFbx(ref Entity entity, string path)
        {
            var fbxAsset = FbxUtilities.Import(path);
            var meshComponent = entity.CurrentContext.RegisterComponent(new MeshRendererComponent(fbxAsset.Mesh));
            entity.AddComponent(ref meshComponent);
        }
    }
}