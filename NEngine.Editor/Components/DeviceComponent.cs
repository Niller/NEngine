using System.Windows.Media.Imaging;
using ECS;
using Math.Vectors;

namespace NEngine.Editor.Components
{
    [Component("Main")]
    public struct DeviceComponent : IComponent
    {
        public bool HasValue
        {
            get;
            set;
        }

        public DeviceComponent(Vector2 resolution, byte[] backBuffer, WriteableBitmap bmp)
        {
            Resolution = resolution;
            BackBuffer = backBuffer;
            Bmp = bmp;
            HasValue = true;
        }

        public readonly Vector2 Resolution;
        public readonly byte[] BackBuffer;
        public readonly WriteableBitmap Bmp;
        
    }
}