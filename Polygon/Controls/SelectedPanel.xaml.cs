using PolygonMaker.Notification;
using PolygonMaker.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PolygonMaker.Controls
{
    /// <summary>
    /// Interaction logic for SelectedPanel.xaml
    /// </summary>
    public partial class SelectedPanel : UserControl, IListener
    {
        public PolygonRender Polygon { get; set; }
        public ObservableCollection<PolygonProperty> Properties { get; set; }
        public SelectedPanel()
        {
            InitializeComponent();
            Properties = new ObservableCollection<PolygonProperty>();
            PolygonPropertiesDataGrid.ItemsSource = Properties;
            Visibility = Visibility.Collapsed;

        }

        private int CastInt(string s)
        {
            int toRet = 0;
            if (!int.TryParse(s, out toRet)) return -1;
            return toRet;
        }

        public void SetPolygon(PolygonRender newP)
        {
            Polygon = newP;
            Visibility = Visibility.Visible;
        }

        public void ClearPolygon()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Notify(EventBase eventBase)
        {
            var polygonSelectedEvent = eventBase as PolygonSelectedEvent;
            if (polygonSelectedEvent != null)
            {
                Polygon = polygonSelectedEvent.SelectedPolygon;
                Properties.Clear();
                Visibility = Visibility.Visible;
                Properties.Add(new PolygonProperty() { Name = "Z Index", Value = Polygon.ZIndex.ToString(), OnValueChanged = (a) => Polygon.ZIndex = CastInt(a) });
                Properties.Add(new PolygonProperty() { Name = "Fill Brush", Value = Polygon.FillBrush.ToString(), OnValueChanged = (a) => Polygon.FillBrush = GetBrushFromString(a) });
                Properties.Add(new PolygonProperty() { Name = "Line Brush", Value = Polygon.LineBrush.ToString(), OnValueChanged = (a) => Polygon.LineBrush = GetBrushFromString(a) });
                Properties.Add(new PolygonProperty() { Name = "Line Thickness", Value = Polygon.LineThickness.ToString(), OnValueChanged = (a) => Polygon.LineThickness = CastInt(a) });
            }
        }

        private Brush GetBrushFromString(string colorString)
        {
            BrushConverter brushConverter = new BrushConverter();
            return (Brush)brushConverter.ConvertFromString(colorString);
        }

        public class PolygonProperty
        {
            public string Name { get; set; }
            private string _value;
            public string Value { get { return _value; } set { _value = value;if(OnValueChanged!=null) OnValueChanged.Invoke(value); } }

            public Action<string> OnValueChanged { get; set; }
        }
    }
}
