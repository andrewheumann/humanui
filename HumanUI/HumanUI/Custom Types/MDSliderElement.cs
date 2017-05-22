using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace HumanUI
{
    public class MDSliderElement : Viewbox, INotifyPropertyChanged
    {

        private string _LocString = "";

        public Rhino.Geometry.Point3d SliderPoint
        {
            get => new Rhino.Geometry.Point3d(XSlider.Value / XSlider.Maximum, YSlider.Value / YSlider.Maximum, 0);
            set
            {
             //   OnPropertyChanged("SliderPoint");
                XSlider.Value = value.X * (400 - 33);
                YSlider.Value = value.Y * (400 - 33);

            }
        }

        internal string locationString
        {
            get
            {
                Rhino.Geometry.Point3d sliderPt = SliderPoint;
                _LocString = String.Format("{0:0.00}, {1:0.00}", sliderPt.X, sliderPt.Y);
                return _LocString;
            }

            set
            {
            //    OnPropertyChanged("locationString");
                tooltip.Content = value;
                _LocString = value;
            }
        }


        Canvas LayoutRoot;
        Slider XSlider, YSlider;
        Ellipse DragCircle;
        Label tooltip;

        public MDSliderElement()
            : base()
        {
            this.Height = 230;
            this.Width = 230;
            this.Margin = new Thickness(10);
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Grid childGrid = new Grid();


            childGrid.Height = 400;
            childGrid.Width = 400;

            TransformGroup flipV = new TransformGroup();
            flipV.Children.Add(new ScaleTransform(1, -1));
            childGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            childGrid.RenderTransform = flipV;

            RowDefinition firstRow = new RowDefinition() { Height = new System.Windows.GridLength(33) };
            RowDefinition secondRow = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition firstColumn = new ColumnDefinition() { Width = new System.Windows.GridLength(33) };
            ColumnDefinition secondColumn = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
            childGrid.RowDefinitions.Add(firstRow);
            childGrid.RowDefinitions.Add(secondRow);
            childGrid.ColumnDefinitions.Add(firstColumn);
            childGrid.ColumnDefinitions.Add(secondColumn);

            Rectangle rectangle = new Rectangle() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)) };
            rectangle.SetValue(Grid.RowProperty, 1);
            rectangle.SetValue(Grid.ColumnProperty, 1);

            LayoutRoot = new Canvas();
            LayoutRoot.SetValue(Grid.RowProperty, 1);
            LayoutRoot.SetValue(Grid.ColumnProperty, 1);

            TransformGroup SliderTransform = new TransformGroup();
            SliderTransform.Children.Add(new ScaleTransform() { ScaleX = 1.03, ScaleY = 1.03 });
            XSlider = new Slider() { RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = SliderTransform };
            XSlider.SetValue(Grid.ColumnProperty, 1);
            Binding xWidthBinding = new Binding() { Source = LayoutRoot, Mode = BindingMode.OneWay, Path = new PropertyPath("ActualWidth") };
            XSlider.SetBinding(Slider.MaximumProperty, xWidthBinding);

            XSlider.GotMouseCapture += Slider_MouseDown;
            XSlider.LostMouseCapture += Slider_MouseUp;
            XSlider.ValueChanged += SliderValueChanged;



            YSlider = new Slider() { RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = SliderTransform, Orientation = Orientation.Vertical, IsDirectionReversed = true };
            YSlider.SetValue(Grid.RowProperty, 1);
            Binding yHeightBinding = new Binding() { Source = LayoutRoot, Mode = BindingMode.OneWay, Path = new PropertyPath("ActualHeight") };
            YSlider.SetBinding(Slider.MaximumProperty, yHeightBinding);

            YSlider.GotMouseCapture += Slider_MouseDown;
            YSlider.LostMouseCapture += Slider_MouseUp;
            YSlider.ValueChanged += SliderValueChanged;

            childGrid.Children.Add(XSlider);
            childGrid.Children.Add(YSlider);
            childGrid.Children.Add(rectangle);
            childGrid.Children.Add(LayoutRoot);


            Binding XPositionBinding = new Binding() { Source = XSlider, Mode = BindingMode.TwoWay, Path = new PropertyPath("Value") };
            Binding YPositionBinding = new Binding() { Source = YSlider, Mode = BindingMode.TwoWay, Path = new PropertyPath("Value") };

            Line verticalLine = new Line() { Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), StrokeDashArray = new DoubleCollection(new List<double>() { 2, 4 }) };
            verticalLine.SetBinding(Line.X1Property, XPositionBinding);
            verticalLine.SetBinding(Line.X2Property, XPositionBinding);
            verticalLine.Y1 = 0;
            verticalLine.SetBinding(Line.Y2Property, yHeightBinding);
            LayoutRoot.Children.Add(verticalLine);

            Line horizontalLine = new Line() { Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), StrokeDashArray = new DoubleCollection(new List<double>() { 2, 4 }) };
            horizontalLine.SetBinding(Line.Y1Property, YPositionBinding);
            horizontalLine.SetBinding(Line.Y2Property, YPositionBinding);
            horizontalLine.X1 = 0;
            horizontalLine.SetBinding(Line.X2Property, xWidthBinding);
            LayoutRoot.Children.Add(horizontalLine);
            TransformGroup circleShift = new TransformGroup();
            circleShift.Children.Add(new TranslateTransform(-7.5, -7.5));

            DragCircle = new Ellipse() { Width = 15, Height = 15, Fill = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)), Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = circleShift };
            DragCircle.SetBinding(Canvas.LeftProperty, XPositionBinding);
            DragCircle.SetBinding(Canvas.TopProperty, YPositionBinding);

            DragCircle.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            DragCircle.MouseMove += Ellipse_MouseMove;
            DragCircle.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

            tooltip = new Label();
            tooltip.RenderTransformOrigin = new Point(0.5, 0.5);
            tooltip.RenderTransform = flipV;
            tooltip.FontSize = 16;
            tooltip.Visibility = System.Windows.Visibility.Hidden;
            tooltip.SetBinding(Canvas.LeftProperty, XPositionBinding);
            tooltip.SetBinding(Canvas.TopProperty, YPositionBinding);



            Binding tooltipBinding = new Binding() { Source = this.locationString, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

            tooltip.SetBinding(Label.ContentProperty, tooltipBinding);
            LayoutRoot.Children.Add(tooltip);

            LayoutRoot.Children.Add(DragCircle);

            this.Child = childGrid;
            Size maxSize = new Size(Double.MaxValue, Double.MaxValue);
            LayoutRoot.Measure(maxSize);
            this.Measure(maxSize);
            childGrid.Measure(maxSize);
            Rhino.Geometry.Point3d sliderPt = SliderPoint;
        }

        bool captured = false;
        double xShape, yShape, xCanvas, yCanvas;
        UIElement source = null;

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            xShape = Canvas.GetLeft(source);
            xCanvas = e.GetPosition(LayoutRoot).X;
            yShape = Canvas.GetTop(source);
            yCanvas = e.GetPosition(LayoutRoot).Y;
        }

        private void Slider_MouseDown(object sender, EventArgs e)
        {
            tooltip.Visibility = System.Windows.Visibility.Visible;

        }
        private void SliderValueChanged(object sender, EventArgs e)
        {
            locationString = String.Format("{0:0.00},{1:0.00}", SliderPoint.X, SliderPoint.Y);
            OnPropertyChanged("Location");
        }
        private void Slider_MouseUp(object sender, EventArgs e)
        {
            tooltip.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tooltip.Visibility = System.Windows.Visibility.Hidden;
            Mouse.Capture(null);
            captured = false;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                tooltip.Visibility = System.Windows.Visibility.Visible;
                double x = e.GetPosition(LayoutRoot).X;
                double y = e.GetPosition(LayoutRoot).Y;
                xShape += x - xCanvas;
                Canvas.SetLeft(source, xShape);
                xCanvas = x;
                yShape += y - yCanvas;
                Canvas.SetTop(source, yShape);
                yCanvas = y;
              //  OnPropertyChanged("Location");
                Rhino.Geometry.Point3d sliderPt = SliderPoint;
                locationString = String.Format("{0:0.00},{1:0.00}", SliderPoint.X, SliderPoint.Y);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
