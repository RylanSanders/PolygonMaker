using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Shapes
{
    public class Polygon
    {
        public ObservableCollection<Point> Points;
        public Polygon() { Points = new ObservableCollection<Point>(); }
    }
}
