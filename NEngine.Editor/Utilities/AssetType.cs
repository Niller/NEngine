using System.IO;


namespace NEngine.Editor.Utilities
{
    public static class FbxUtilities
    {
        public static void Import(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file could not be found", path);
            }
        }
    }
        
    public enum AssetType
    {
        Undefined,
        Fbx
    }
}