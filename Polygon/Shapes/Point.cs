using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PolygonMaker.Shapes
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public System.Windows.Point ToWindows()
        {
            return new System.Windows.Point(X, Y);
        }

        public Point() { }

        public Point(System.Windows.Point p) { X = p.X; Y = p.Y; }

        public double DistTo(Point p)
        {
            return CalculateDistance(ToWindows(), p.ToWindows());
        }

        public Point Translate(TranslateTransform transform)
        {
            return new Point() { X = X + transform.X, Y = Y + transform.Y };
        }

        public double DistTo(System.Windows.Point p)
        {
            return CalculateDistance(ToWindows(), p);
        }

        public static double CalculateDistance(System.Windows.Point point1, System.Windows.Point point2)
        {
            // Using the Euclidean distance formula
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;

            return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }

        public Point Clone()
        {
            return new Point() {X=X, Y = Y}; 
        }
    }
}
