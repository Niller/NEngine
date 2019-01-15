using System.Windows.Media.Imaging;
using ECS;
using Math.Vectors;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct DeviceComponent
    {
        public readonly Vector2 Resolution;
        public readonly byte[] BackBuffer;
        public readonly WriteableBitmap Bmp;
    }
}