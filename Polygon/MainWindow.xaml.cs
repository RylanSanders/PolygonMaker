using PolygonMaker.Render;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolygonMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MainGrid.Children.OfType<IKeyListener>().ToList().ForEach(c=>c.HandleKeyEvent(e));
        }
    }
}