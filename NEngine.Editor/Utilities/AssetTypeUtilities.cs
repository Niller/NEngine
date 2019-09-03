namespace NEngine.Editor.Utilities
{
    public static class AssetTypeUtilities
    {
        public static AssetType GetAssetType(string path)
        {
            var lowerPath = path.ToLower();
            if (lowerPath.EndsWith(".fbx"))
            {
                return AssetType.Fbx;
            }
            return AssetType.Undefined;
        }
    }
}