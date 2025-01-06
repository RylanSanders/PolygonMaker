using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Shapes
{
    //TODO figure out if I actually want this for now I'm going to use a DO
    public class Polygon
    {
        public ObservableCollection<Point> Points;
        public Polygon() { Points = new ObservableCollection<Point>(); }
    }
}
