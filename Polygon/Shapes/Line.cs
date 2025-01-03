using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Shapes
{
    //Ignoring this for Polygons instead
    public class Line
    {
        public ObservableCollection<Point> Points;

        public Line() { Points = new ObservableCollection<Point>(); }
    }
}
