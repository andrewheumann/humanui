using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;


namespace HumanUI
{
    internal class GradientHandle
    {
        public double T { get; set; }
        public Color Color { get; set; }

        public bool Hide { get; internal set; }

        public GradientStop GradientStop => new GradientStop(Color, T);
        public Ellipse handleEllipse { get; private set; }

        private HUI_GradientEditor GradientEditor;
        private Canvas canvas;
        public GradientHandle(double T, Color Color, HUI_GradientEditor canvas)
        {
            this.T = T;
            this.Color = Color;
            this.GradientEditor = canvas;
            this.canvas = canvas.Children.OfType<Canvas>().FirstOrDefault();
            var handleSize = 14;
            Ellipse e = new Ellipse();
            e.Width = handleSize;
            e.Height = handleSize;
            e.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            e.StrokeThickness = 3;
            e.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            e.RenderTransform = new TranslateTransform(-handleSize / 2.0, -handleSize / 2.0);
            e.MouseLeftButtonUp += Handle_MouseUp;
            e.MouseLeftButtonDown += Handle_MouseLeftButtonDown;
            e.MouseMove += Handle_MouseMove;
            e.MouseRightButtonUp += E_MouseRightButtonUp;

            handleEllipse = e;
            UpdatePosition();
            this.canvas.Children.Add(handleEllipse);
            canvas.SizeChanged += Bounds_SizeChanged;
        }

        private void E_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditColor();
            e.Handled = true;

        }


        private void Bounds_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            if (Hide)
            {
                handleEllipse.Visibility = Visibility.Hidden;
            }
            else
            {
                handleEllipse.Visibility = Visibility.Visible;
            }
            Canvas.SetLeft(handleEllipse, T * GradientEditor.ActualWidth);
            Canvas.SetTop(handleEllipse, GradientEditor.GradientHeight / 2);
            GradientEditor.UpdateGradient();
        }


        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        private void Handle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(string.Format("This handle at position {0} has color {1}", T, Color));
            Mouse.Capture(handleEllipse);
            captured = true;
            x_shape = Canvas.GetLeft(handleEllipse);
            x_canvas = e.GetPosition(GradientEditor).X;
            y_shape = Canvas.GetTop(handleEllipse);
            y_canvas = e.GetPosition(GradientEditor).Y;
        }

        private double constrain(double coord, double max)
        {
            if (coord < 0) return 0;
            if (coord > max) return max;
            return coord;
        }

        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {

                double x = e.GetPosition(GradientEditor).X;
                double y = e.GetPosition(GradientEditor).Y;
                x = constrain(x, GradientEditor.ActualWidth);
                x_shape += x - x_canvas;
                x_shape = constrain(x_shape, GradientEditor.ActualWidth);

                x_canvas = x;

                y_shape += y - y_canvas;

                T = x_shape / GradientEditor.ActualWidth;

                bool draggedOff = e.GetPosition(GradientEditor).Y < 0 || e.GetPosition(GradientEditor).Y > GradientEditor.ActualHeight;
                if (draggedOff)
                {
                    handleEllipse.Opacity = 0.5;
                }
                else
                {
                    handleEllipse.Opacity = 1.0;
                }
                UpdatePosition();

            }
        }

        private void Handle_MouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
            bool draggedOff = e.GetPosition(GradientEditor).Y < 0 || e.GetPosition(GradientEditor).Y > GradientEditor.ActualHeight;
            Hide = draggedOff;
            if (Hide) UpdatePosition();
            Console.WriteLine(draggedOff);
            captured = false;
        }

        ColorCanvas cp;
        Popup popup = null;
        void EditColor()
        {
            if (cp == null || popup == null)
            {
                popup = new Popup();
                cp = new ColorCanvas();
                cp.Focusable = true;
                popup.Focusable = true;
                cp.SelectedColor = Color;
                popup.Child = cp;
                GradientEditor.Children.Add(popup);
                cp.SelectedColorChanged += Cp_SelectedColorChanged;

                popup.Placement = PlacementMode.Mouse;
                popup.StaysOpen = false;
                popup.MouseUp += Popup_MouseUp;
            }
            FocusManager.SetIsFocusScope(popup, true);

            popup.Focus();
            popup.IsOpen = true;



        }

        private void Cp_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null)
            {
                Color = (Color)e.NewValue;
                GradientEditor.UpdateGradient();
            }
        }

        private void Popup_MouseUp(object sender, MouseButtonEventArgs e)
        {

            e.Handled = true;

        }

        public void ClosePopup()
        {
            if (popup != null)
            {
                popup.IsOpen = false;
            }

        }





    }
}
