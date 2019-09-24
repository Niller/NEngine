using System.Windows.Media.Imaging;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using NEngine.Editor.Contexts;

namespace NEngine.Editor.Components
{
    [Component(typeof(MainContext))]
    public struct DeviceComponent
    {

        public DeviceComponent(Vector2 resolution, byte[] backBuffer, WriteableBitmap bmp)
        {
            Resolution = resolution;
            BackBuffer = backBuffer;
            Bmp = bmp;
        }

        public Vector2 Resolution
        {
            get;
        }

        public byte[] BackBuffer
        {
            get;
        }

        public WriteableBitmap Bmp
        {
            get;
        }
        
    }
}