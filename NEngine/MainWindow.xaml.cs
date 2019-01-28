using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
       
        public MainWindow()
        {
            this.InitializeComponent();

            _editor = new Editor.Editor();
            _editor.Initialize();

            var bmp = BitmapFactory.New(640, 480);

            FrontBuffer.Source = bmp;

            Services.EditorContext.RenderBitmap = bmp;

            InitializeComponent();

            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            Services.ECS.Execute();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }
    }
}
