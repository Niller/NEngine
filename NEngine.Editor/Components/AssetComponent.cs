using ECS.Experimental;
using NEngine.Editor.Contexts;
using NEngine.Editor.Utilities;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct AssetComponent
    {
        public AssetComponent(string path)
        {
            Path = path;
            Type = AssetTypeUtilities.GetAssetType(path);
        }

        public string Path
        {
            get;
        }

        public AssetType Type
        {
            get;
        }
    }
}