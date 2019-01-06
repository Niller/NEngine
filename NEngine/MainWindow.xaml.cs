using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Math.Vectors;

namespace NEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Device _device;
        Mesh _mesh;
        readonly Camera _camera = new Camera();

        public MainWindow()
        {
            this.InitializeComponent();

            var bmp = BitmapFactory.New(640, 480);

            FrontBuffer.Source = bmp;

            InitializeComponent();
            _device = new Device(new Vector2(640, 480), bmp);

            CompositionTarget.Rendering += CompositionTargetOnRendering;

            TestScene1();
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            _device.Clear(0, 0, 0, 255);

            // rotating slightly the cube during each frame rendered
            _mesh.Rotation = new Vector3(_mesh.Rotation.X + 0.01f, _mesh.Rotation.Y + 0.01f, _mesh.Rotation.Z);

            // Doing the various matrix operations
            _device.Render(_camera, _mesh);
            // Flushing the back buffer into the front buffer
            _device.Present();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }

        private void TestScene1()
        {
            _mesh = new Mesh("Cube", 8)
            {
                Vertices =
                {
                    [0] = new Vector3(-1, 1, 1),
                    [1] = new Vector3(1, 1, 1),
                    [2] = new Vector3(-1, -1, 1),
                    [3] = new Vector3(-1, -1, -1),
                    [4] = new Vector3(-1, 1, -1),
                    [5] = new Vector3(1, 1, -1),
                    [6] = new Vector3(1, -1, 1),
                    [7] = new Vector3(1, -1, -1)
                }
            };

            _mesh.Position = Vector3.Zero;
            _mesh.Rotation = Vector3.Zero;

            _camera.Position = new Vector3(0, 0, 10.0f);
            _camera.Target = Vector3.Zero;
        }
    }
}
