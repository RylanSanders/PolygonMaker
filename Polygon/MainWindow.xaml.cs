using Microsoft.Win32;
using PolygonMaker.Controls;
using PolygonMaker.Notification;
using PolygonMaker.Render;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolygonMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScaleTransform scaleTransform;
        TranslateTransform translateTransform;
        private const double WHEEL_ZOOM_SCALE = 0.001;
        public MainWindow()
        {
            InitializeComponent();

            scaleTransform = new ScaleTransform();
            translateTransform = new TranslateTransform();
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            MainGrid.RenderTransform = transformGroup;

            Shapes.Polygon p = new Shapes.Polygon();
            p.Points.Add(new Shapes.Point() {X=1, Y=1 });
            p.Points.Add(new Shapes.Point() { X = 50, Y = 50 });
            p.Points.Add(new Shapes.Point() { X = 200, Y = 30 });
            MainGrid.Children.Add(new PolygonRender(p, MainGrid));

            Shapes.Polygon p2 = new Shapes.Polygon();
            p2.Points.Add(new Shapes.Point() { X = 1, Y = 1 });
            p2.Points.Add(new Shapes.Point() { X = 50, Y = 50 });
            p2.Points.Add(new Shapes.Point() { X = 200, Y = 30 });
            MainGrid.Children.Add(new PolygonRender(p2, MainGrid));

            KeyDown += MainWindow_KeyDown;
            PreviewMouseWheel += MainWindow_MouseWheel;

            HorizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            VerticalScrollBar.Scroll += VerticalScrollBar_Scroll;

            NotificationHandler.RegisterListener(SelectedPolygonPanel);
        }

        private void VerticalScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            translateTransform.Y = -1*e.NewValue * MainGrid.ActualHeight * scaleTransform.ScaleY;
        }

        private void HorizontalScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            translateTransform.X = -1*e.NewValue * MainGrid.ActualWidth * scaleTransform.ScaleX;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MainGrid.Children.OfType<IKeyListener>().ToList().ForEach(c=>c.HandleKeyEvent(e));
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scaleTransform.ScaleX += e.Delta * WHEEL_ZOOM_SCALE;
            scaleTransform.ScaleY += e.Delta * WHEEL_ZOOM_SCALE;
        }

        public string CurrentFilePath = string.Empty;

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFilePath == string.Empty)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.json)|*.json|All Files (*.*)|*.*",
                    DefaultExt = ".txt",
                    FileName = "polygons.json",
                    Title = "Save As"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    CurrentFilePath  = saveFileDialog.FileName;
                }
            }
            
            if(CurrentFilePath != string.Empty)
            {
                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                StringWriter outputStr = new StringWriter();
                jsonSerializer.Serialize(outputStr, MainGrid.Children.OfType<PolygonRender>().Select(p => p.ToDataObject()));
                File.WriteAllText(CurrentFilePath, outputStr.ToString());
                outputStr.Close();
            }

        }
    }
}