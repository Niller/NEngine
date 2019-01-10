using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private WriteableBitmap _bmp;

        public MainWindow()
        {
            this.InitializeComponent();

            _bmp = BitmapFactory.New(640, 480);

            FrontBuffer.Source = _bmp;

            InitializeComponent();
            _device = new Device(new Vector2(640, 480), _bmp);

            CompositionTarget.Rendering += CompositionTargetOnRendering;
           

            TestScene1();
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            Render();
        }

        private void Render()
        {
            _bmp.Lock();
            _device.Clear(0, 0, 0, 255);

            // rotating slightly the cube during each frame rendered
            _mesh.Rotation = new Vector3(_mesh.Rotation.X + 0.01f, _mesh.Rotation.Y + 0.01f, _mesh.Rotation.Z);

            // Doing the various matrix operations
            _device.Render(_camera, _mesh);
            // Flushing the back buffer into the front buffer
            _device.Present();

            _bmp.AddDirtyRect(new Int32Rect(0, 0,
                _bmp.PixelWidth, _bmp.PixelHeight));
            _bmp.Unlock();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }

        private void TestScene1()
        {
            _mesh = new Mesh("Cube", 8, 12)
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
                },
                Position = Vector3.Zero,
                Rotation = Vector3.Zero
            };





            _camera.Position = new Vector3(0, 0, 10.0f);
            _camera.Target = Vector3.Zero;
        }
    }
}
