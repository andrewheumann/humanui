using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace HumanUI
{
    public class GraphMapperElement : Viewbox, INotifyPropertyChanged
    {

        Canvas LayoutRoot;
        Ellipse FreeHandleLeft, FreeHandleRight, FixedHandleLeft, FixedHandleRight;
        Line LeftLine, RightLine;
        Path BezierPath;
        PathFigure MainBezierFigure;
        BezierSegment MainBezierSegment;
        int ctrlSize = 310;
        int HandleSize = 15;
        private Point _c0, _c1, _c2, _c3;
        public Point C0 {
            get => _c0;
            set
            {
                _c0 = value;
                try
                {
                    MainBezierFigure.StartPoint = Scale(value, ctrlSize);
                    LeftLine.Y1 = value.Y * ctrlSize;
                    Canvas.SetTop(FixedHandleLeft, value.Y * ctrlSize);
                }
                catch { }
            }
        }

        public Point C1
        {
            get => _c1;
            set
            {
                _c1 = value;
                try
                {
                    MainBezierSegment.Point1 = Scale(value, ctrlSize);
                    LeftLine.X2 = value.X * ctrlSize;
                    LeftLine.Y2 = value.Y * ctrlSize;
                    Canvas.SetLeft(FreeHandleLeft, value.X * ctrlSize);
                    Canvas.SetTop(FreeHandleLeft, value.Y * ctrlSize);
                }
                catch { }
                

            }
        }


        public Point C2
        {
            get => _c2;
            set
            {
                _c2 = value;
                try
                {
                    MainBezierSegment.Point2 = Scale(value, ctrlSize);
                    RightLine.X2 = value.X * ctrlSize;
                    RightLine.Y2 = value.Y * ctrlSize;
                    Canvas.SetLeft(FreeHandleRight, value.X * ctrlSize);
                    Canvas.SetTop(FreeHandleRight, value.Y * ctrlSize);
                }
                catch { }


            }
        }


        public Point C3
        {
            get => _c3;
            set
            {
                _c3 = value;
                try
                {
                    MainBezierSegment.Point3 = Scale(value, ctrlSize);
                    RightLine.Y1 = value.Y * ctrlSize;
                    RightLine.X1 = ctrlSize;
                    Canvas.SetTop(FixedHandleRight, value.Y * ctrlSize);
                    Canvas.SetLeft(FixedHandleRight, ctrlSize);
                }
                catch { }
            }
        }

        public GraphMapperElement() : base()
        {
            Point c0 = new Point(0, 0);
            Point c1 = new Point( 0.25, 0.25);
            Point c2 = new Point( 0.75,  0.75);
            Point c3 = new Point(1, 1);
            Initialize(c0, c1, c2, c3);

        }

        private void Initialize(Point c0, Point c1, Point c2, Point c3)
        {
            ctrlSize = 310;
            HandleSize = 15;
            Height = 230;
            Width = 230;
            Margin = new Thickness(10);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

           


            Grid childGrid = new Grid();
            Child = childGrid;

            Rectangle outer = new Rectangle() { Width = ctrlSize + HandleSize, Height = ctrlSize + HandleSize, Stroke = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128)), StrokeThickness = 7.5 };
            childGrid.Children.Add(outer);
            Rectangle inner = new Rectangle() { Width = ctrlSize, Height = ctrlSize, Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeThickness = 1, Fill = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)) };
            childGrid.Children.Add(inner);

            LayoutRoot = new Canvas() { Width = ctrlSize, Height = ctrlSize, RenderTransformOrigin = new Point(0.5, 0.5) };
            childGrid.Children.Add(LayoutRoot);


            BezierPath = new Path() { Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeThickness = 4 };
            List<PathFigure> figures = new List<PathFigure>();
            List<PathSegment> segments = new List<PathSegment>();
            MainBezierSegment = new BezierSegment();
            segments.Add(MainBezierSegment);
            MainBezierFigure = new PathFigure(new Point(0,0), segments, false);
            figures.Add(MainBezierFigure);
            BezierPath.Data = new PathGeometry(figures);

            LayoutRoot.Children.Add(BezierPath);

            FreeHandleLeft = new Ellipse();
            FreeHandleRight = new Ellipse();
            FixedHandleRight = new Ellipse();
            FixedHandleLeft = new Ellipse();

            List<Ellipse> ellipses = new List<Ellipse>()
            {
                FreeHandleLeft, FreeHandleRight, FixedHandleLeft, FixedHandleRight
            };

            LeftLine = new Line() { Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), StrokeThickness = 1 };
            RightLine = new Line() { Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), StrokeThickness = 1 };

            LayoutRoot.Children.Add(LeftLine);
            LayoutRoot.Children.Add(RightLine);

            foreach (Ellipse e in ellipses)
            {

                e.Width = HandleSize;
                e.Height = HandleSize;
                e.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                e.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                e.StrokeThickness = 3;
                e.AddHandler(Ellipse.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Handle_MouseLeftButtonDown));
                e.AddHandler(Ellipse.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Handle_MouseLeftButtonUp));
                e.RenderTransform = new TranslateTransform(-HandleSize / 2.0, -HandleSize / 2.0);
                LayoutRoot.Children.Add(e);


            }
            FreeHandleLeft.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(FreeHandle_MouseMove));
            FreeHandleRight.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(FreeHandle_MouseMove));
            FixedHandleLeft.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(FixedHandle_MouseMove));
            FixedHandleRight.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(FixedHandle_MouseMove));

            C0 = c0;
            C1 = c1;
            C2 = c2;
            C3 = c3;
            LayoutRoot.RenderTransform = new ScaleTransform(1, -1);
        }
        public GraphMapperElement(Point c0, Point c1, Point c2, Point c3) : base()
        {
            Initialize(c0,c1,c2,c3);
        }



        static Point Scale(Point p, double scaleFactor)
        {
            return new Point(p.X * scaleFactor, p.Y * scaleFactor);
        }

        UIElement source = null;
        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Handle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
        }

        private void FreeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {

                double x = e.GetPosition(LayoutRoot).X;
                double y = e.GetPosition(LayoutRoot).Y;
                x = constrain(x, LayoutRoot.Width);
                y = constrain(y, LayoutRoot.Height);
                x_shape += x - x_canvas;
                x_shape = constrain(x_shape, LayoutRoot.Width);
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                y_shape = constrain(y_shape, LayoutRoot.Height);
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
                if (source == FreeHandleLeft)
                {
                    
                    C1 = new Point(x_shape/ctrlSize, y_shape/ctrlSize);
                    

                }
                else if (source == FreeHandleRight)
                {
                  
                    C2 = new Point(x_shape/ctrlSize, y_shape/ctrlSize);
                  
                }
                OnPropertyChanged("Handle Move");
            }
        }

        private double constrain(double coord, double max)
        {
            if (coord < 0) return 0;
            if (coord > max) return max;
            return coord;
        }

        private void FixedHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {

                double y = e.GetPosition(LayoutRoot).Y;
                y = constrain(y, LayoutRoot.Height);
                y_shape += y - y_canvas;
                y_shape = constrain(y_shape, LayoutRoot.Height);
               
                y_canvas = y;
                if (source == FixedHandleLeft)
                {
                    C0 = new Point(0, y_shape/ctrlSize);
    
                }
                else if (source == FixedHandleRight)
                {
                    C3 = new Point(1, y_shape/ctrlSize);
                }
                OnPropertyChanged("Handle Move");
            }
        }

        internal void SetByCurve(Rhino.Geometry.NurbsCurve crv)
        {
            SetByCurve(new Rhino.Geometry.BezierCurve(crv.Points.Select(p => p.Location)));
        }

        internal void SetByCurve(Rhino.Geometry.BezierCurve crv)
        {
            var C0p = crv.GetControlVertex2d(0);
            var C1p = crv.GetControlVertex2d(1);
            var C2p = crv.GetControlVertex2d(2);
            var C3p = crv.GetControlVertex2d(3);
            C0 = new Point(C0p.X, C0p.Y);
            C1 = new Point(C1p.X, C1p.Y);
            C2 = new Point(C2p.X, C2p.Y);
            C3 = new Point(C3p.X, C3p.Y);
        }

        public Rhino.Geometry.BezierCurve GetCurve()
        {
            List<Rhino.Geometry.Point3d> ctrlPts = new List<Rhino.Geometry.Point3d>()
            {
                new Rhino.Geometry.Point3d(C0.X,C0.Y,0),
                new Rhino.Geometry.Point3d(C1.X,C1.Y,0),
                new Rhino.Geometry.Point3d(C2.X,C2.Y,0),
                new Rhino.Geometry.Point3d(C3.X,C3.Y,0),
            };

            Rhino.Geometry.BezierCurve bzC = new Rhino.Geometry.BezierCurve(ctrlPts);
            return bzC;
        }

        private void Handle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = e.GetPosition(LayoutRoot).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = e.GetPosition(LayoutRoot).Y;

        }

    }

}
