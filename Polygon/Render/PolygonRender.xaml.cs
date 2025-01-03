
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolygonMaker.Render
{
    /// <summary>
    /// Interaction logic for PolygonRender.xaml
    /// </summary>
    public partial class PolygonRender : UserControl
    {
        public Shapes.Polygon Model { get; set; } 
        public bool isDragging = false;
        public Vector PolygonDraggingOffset;
        public bool isSelected = false;
        public int PointRadius = 8;

        //TODO figure out a good way to deal with this so I don't need a seperate point thing
        public bool isDraggingPoint = false;
        public Shapes.Point DraggedPoint;
        public Shapes.Point OriginalDraggedPoint;

        public TranslateTransform Transform { get; set; }

        public UIElement Parent { get; set; }
        public PolygonRender(Shapes.Polygon model, UIElement parent)
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
                DraggedPoint.X = e.GetPosition(Parent).X - Transform.X;
                DraggedPoint.Y = e.GetPosition(Parent).Y - Transform.Y;
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
                    DraggedPoint = closePoint;
                    OriginalDraggedPoint = DraggedPoint.Translate(Transform).Clone();
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
            isSelected = true;
            InvalidateVisual();
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
                var pointPen = new Pen
                {
                    Thickness = 2,
                    Brush = Brushes.Blue
                };
                //TOOD make this a combined geometry with the streamGeometryContext for the path
                Model.Points.Select(p => p.ToWindows()).ToList().ForEach(p=>drawingContext.DrawEllipse(Brushes.Blue, pointPen, p, PointRadius, PointRadius));
            }
        }



    }
}
