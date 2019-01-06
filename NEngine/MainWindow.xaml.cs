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
        private readonly Device _device;

        public MainWindow()
        {
            InitializeComponent();
            _device = new Device(new Vector2(640, 480), null);
            
            CompositionTarget.Rendering += CompositionTargetOnRendering;

            TestScene1();
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            //_device.Render();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }

        private void TestScene1()
        {
            var mesh = new Mesh("Cube", 8)
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
        }
    }
}
