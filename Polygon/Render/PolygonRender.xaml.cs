
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolygonMaker.Render
{
    /// <summary>
    /// Interaction logic for PolygonRender.xaml
    /// </summary>
    public partial class PolygonRender : UserControl, IKeyListener
    {
        public Shapes.Polygon Model { get; set; } 
        public bool isDragging = false;
        public Vector PolygonDraggingOffset;
        public bool isSelected = false;
        public int PointRadius = 8;

        //TODO figure out a good way to deal with this so I don't need a seperate point thing
        public bool isDraggingPoint = false;
        public Shapes.Point? SelectedPoint;
        public Shapes.Point OriginalDraggedPoint;

        public TranslateTransform Transform { get; set; }

        public Grid Parent { get; set; }
        public PolygonRender(Shapes.Polygon model, Grid parent)
        {
            InitializeComponent();
            Model = model;
            Parent = parent;
            MouseDown += PolygonRender_MouseDown;
            MouseUp += PolygonRender_MouseUp;
            MouseDoubleClick += PolygonRender_MouseDoubleClick;
            PreviewMouseMove += PolygonRender_MouseMove;
            Parent.PreviewMouseMove += PolygonRender_MouseMove;
            Parent.MouseDown += Parent_MouseDown;
            Parent.MouseUp += PolygonRender_MouseUp;
            Transform = new TranslateTransform();
        }

        private void PolygonRender_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = false;
            isDraggingPoint = false;
            InvalidateVisual();
        }

        private void PolygonRender_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                var mousePoint = e.GetPosition(Parent) - PolygonDraggingOffset;
                Transform = new TranslateTransform(mousePoint.X, mousePoint.Y);
                InvalidateVisual();
            }
            else if (isDraggingPoint)
            {
                //Subtract the Transform because we push that with the render so the real point value shouldn't take it into effect
                SelectedPoint.X = e.GetPosition(Parent).X - Transform.X;
                SelectedPoint.Y = e.GetPosition(Parent).Y - Transform.Y;
                InvalidateVisual();
            }
        }

        //This should be the entire window - This way the previewMouseDraggedEvent will always be called within the window
        //TODO figure out if I need this - I started just using the parent events instead =)
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return new Size(1000, 1000);
        }

        //This is only called if the clicked on a visible part of the polygon
        private void PolygonRender_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Points are only visible when selected
            if (isSelected)
            {
                //This needs to take into account the Transform
                var closePoint = Model.Points.FirstOrDefault(p => p.Translate(Transform).DistTo(e.GetPosition(Parent)) < PointRadius);
                //var minPoint = Model.Points.Select(p => p.DistTo(e.GetPosition(Parent)));
                if (closePoint != null)
                {
                    isDraggingPoint = true;
                    SelectedPoint = closePoint;
                    OriginalDraggedPoint = SelectedPoint.Translate(Transform).Clone();
                    return;
                }
            }
            
            isDragging = true;
            PolygonDraggingOffset = e.GetPosition(Parent) - new Point(Transform.X, Transform.Y);
        }

        private void Parent_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Clicked on the canvas outside of this shape
            if (this.InputHitTest(e.GetPosition(Parent))==null)
            {
                isDragging = false;
                isSelected = false;
                InvalidateVisual();
            }
            
        }

        private void PolygonRender_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Add Point Behavior
            if (isSelected)
            {
                var pointInRelationToPolygon = e.GetPosition(Parent);
                pointInRelationToPolygon.X -= Transform.X;
                pointInRelationToPolygon.Y -= Transform.Y;
                var closePoint = GetClosePoint(pointInRelationToPolygon);
                if (closePoint != null)
                {
                   int pos = Model.Points.IndexOf(closePoint);
                    Model.Points.Insert(pos+1, new Shapes.Point(pointInRelationToPolygon));
                }
            }
            isSelected = true;
            SelectedPoint = null;
            
            InvalidateVisual();
        }

        double MIN_POINT_TO_LINE_DIST = 5;
        private Shapes.Point GetClosePoint(Point p)
        {
            for(int i = 1; i < Model.Points.Count; i++)
            {
                Shapes.Point p1 = Model.Points[i-1];
                Shapes.Point p2 = Model.Points[i];
                if (GetDist(p1.X, p1.Y, p2.X, p2.Y, p.X, p.Y)< MIN_POINT_TO_LINE_DIST)
                {
                    return p1;
                }
            }
            //Handle line between the last point and the first index
            var lastPoint = Model.Points.Last();
            var firstPoint = Model.Points.First();
            if (GetDist(lastPoint.X, lastPoint.Y, firstPoint.X, firstPoint.Y, p.X, p.Y) < MIN_POINT_TO_LINE_DIST)
            {
                return lastPoint;
            }
            return null;
        }
        //TODO get this in a Util class - and use the points instead of doubles
        private double GetDist(double ax, double ay, double bx,
                             double by, double x, double y)
        {
            if ((ax - bx) * (x - bx) + (ay - by) * (y - by) <= 0)
                return Math.Sqrt((x - bx) * (x - bx) + (y - by) * (y - by));

            if ((bx - ax) * (x - ax) + (by - ay) * (y - ay) <= 0)
                return Math.Sqrt((x - ax) * (x - ax) + (y - ay) * (y - ay));

            return Math.Abs((by - ay) * x - (bx - ax) * y + bx * ay - by * ax) /
                Math.Sqrt((ay - by) * (ay - by) + (ax - bx) * (ax - bx));
        }

        //TODO could make this more efficient by only calling this when the model is updated
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var myPen = new Pen
            {
                Thickness = 2,
                Brush = Brushes.Black
            };
            myPen.Freeze();

            if(Transform != null)
             {
                drawingContext.PushTransform(Transform);
            }
            var combinedGeometry = new CombinedGeometry();
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                
                ctx.BeginFigure(Model.Points.First().ToWindows(), true /* is filled */, true /* is closed */);
                Model.Points.Select(p => p.ToWindows()).Skip(1).ToList().ForEach(p => ctx.LineTo(p, true, false));
                //ctx.LineTo(new Point(100, 100), true /* is stroked */, false /* is smooth join */);
                //ctx.LineTo(new Point(100, 50), true /* is stroked */, false /* is smooth join */);
            }
            geometry.Freeze();

            drawingContext.DrawGeometry(Brushes.Red, myPen, geometry);

            if (isSelected)
            {
                
                //TOOD make this a combined geometry with the streamGeometryContext for the path
                Model.Points.Select(p => p.ToWindows()).ToList().ForEach(p=>DrawPoint(drawingContext, p));
            }
        }

        private void DrawPoint(DrawingContext dc, Point p)
        {
            var pointPen = new Pen
            {
                Thickness = 2,
                Brush = Brushes.Blue
            };
            Brush fillPoint = Brushes.Blue;
            if(SelectedPoint != null && SelectedPoint.ToWindows() == p)
            {
                fillPoint = Brushes.Green;
                pointPen.Brush = Brushes.Green;
            }
            dc.DrawEllipse(fillPoint, pointPen, p, PointRadius, PointRadius);
        }

        public void HandleKeyEvent(KeyEventArgs args)
        {
            if (!isSelected) return;
            if(args.Key == Key.Delete)
            {
                if (SelectedPoint != null)
                {
                    Model.Points.Remove(SelectedPoint);
                    SelectedPoint = null;
                    InvalidateVisual();
                }
                else
                {
                    Parent.Children.Remove(this);
                }
            }
        }
    }
}
