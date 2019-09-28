using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Math.Vectors;
using NEngine.Editor;
using NEngine.Editor.Components;
using NEngine.Editor.Contexts;

namespace NEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Editor.Editor _editor;
        private readonly DispatcherTimer _dispatcherTimer;

        public MainWindow()
        {
            this.InitializeComponent();

            _editor = new Editor.Editor();
            _editor.Initialize();

            var bmp = BitmapFactory.New(640, 480);

            FrontBuffer.Source = bmp;

            Services.EditorContext.RenderBitmap = bmp;
            Services.EditorContext.HierarchyTreeView = Hierarchy;

            InitializeComponent();

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += Callback;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(16);
            _dispatcherTimer.Start();
            
            //CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        private void Callback(object sender, EventArgs e)
        {
            Services.ECS.Execute();
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            Services.ECS.Execute();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
            _dispatcherTimer.Tick -= Callback;
            _dispatcherTimer.Stop();
        }
    }
}
