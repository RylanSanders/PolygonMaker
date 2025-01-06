using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PolygonMaker.Serialization
{
    [Serializable]
    public class PolygonDO
    {
        public List<Point> Points = new List<Point>();
        public int ZIndex {  get; set; }
        public Brush FillBrush { get; set; }
        public Brush LineBrush { get; set; }
        public int LineThickness { get; set; }
        public double TranslateTransformX { get; set; }
        public double TranslateTransformY { get; set; }
    }
}
