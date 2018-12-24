using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Math.Vectors;

namespace NEngine
{
    public class Rendering
    {
        private Vector2Int _resolution;
        private byte[] _backBuffer;

        public Rendering(Vector2Int resolution)
        {
            _resolution = resolution;
            _backBuffer = new byte[resolution.X * resolution.Y];
            
        }

        public void Render()
        {

        }
    }
}
