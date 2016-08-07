using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HumanUI
{
    public class HUI_GradientEditor : StackPanel, INotifyPropertyChanged
    {
        private Rectangle gradientDisplay;
        private Rectangle backgroundDisplay;
        private ComboBox OptionSelector;
        private List<GradientHandle> handles;
        
        private Canvas MainCanvas;
        internal Double GradientHeight;
        internal Double SelectorHeight;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool HasOptionSelector { get; set; }

        public HUI_Gradient Gradient
        {
            get
            {
                return new HUI_Gradient(handles.Select(h => h.T).ToList(), handles.Select(h => h.Color).ToList());
            }
            set
            {
                handles = LoadFromGradient(value);
                UpdateGradient();
            }
        }

        public HUI_GradientEditor(bool hasOptionSelector, bool showEditor, List<HUI_Gradient> presets) : base()
        {
            Initialize(hasOptionSelector, showEditor, presets);
        }


        void Initialize(bool hasOptionSelector, bool showEditor, List<HUI_Gradient> presets)
        {
            Orientation = Orientation.Vertical;
            MainCanvas = new Canvas();
            Children.Add(MainCanvas);
            Margin = new Thickness(10);
            HasOptionSelector = hasOptionSelector;
            GradientHeight = 40;
            SelectorHeight = 30;
            MainCanvas.Height = GradientHeight;
            gradientDisplay = new Rectangle();
            backgroundDisplay = new Rectangle();
            MainCanvas.Children.Add(backgroundDisplay);
            MainCanvas.Children.Add(gradientDisplay);
            if (!showEditor)
            {
                MainCanvas.Visibility = Visibility.Collapsed;
            }
            handles = new List<GradientHandle>();
            handles.Add(new GradientHandle(0, Color.FromRgb(0, 0, 0), this));
            handles.Add(new GradientHandle(1, Color.FromRgb(255, 255, 255), this));
            Binding widthBinding = new Binding() { Source = this, Path = new PropertyPath("ActualWidth"), Mode = BindingMode.OneWay };
            //  Binding heightBinding = new Binding() { Source = this, Path = new PropertyPath("Height"), Mode = BindingMode.OneWay };
            MainCanvas.SetBinding(Canvas.WidthProperty, widthBinding);
            gradientDisplay.SetBinding(WidthProperty, widthBinding);
            gradientDisplay.Height = GradientHeight;
            backgroundDisplay.SetBinding(WidthProperty, widthBinding);
            backgroundDisplay.Height = GradientHeight;
            backgroundDisplay.Fill = TransparencyGrid();
            gradientDisplay.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            if (presets != null && presets.Count != 0)
            {
                handles = LoadFromGradient(presets[0]);
            }


            if (HasOptionSelector)
            {
                //List<HUI_Gradient> presets = GeneratePresets();

                OptionSelector = new ComboBox();

                FrameworkElementFactory fef = new FrameworkElementFactory(typeof(DockPanel));

                fef.SetValue(Rectangle.WidthProperty, widthBinding);
                fef.SetValue(Rectangle.HeightProperty, (double)20);

                FrameworkElementFactory gradientFEF = new FrameworkElementFactory(typeof(Rectangle));
                gradientFEF.SetValue(Rectangle.FillProperty, new Binding("Brush"));
                FrameworkElementFactory whiteSpaceFEF = new FrameworkElementFactory(typeof(Rectangle));

                whiteSpaceFEF.SetValue(Rectangle.WidthProperty, (double)30);
                whiteSpaceFEF.SetValue(DockPanel.DockProperty, Dock.Right);
                gradientFEF.SetValue(Rectangle.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                fef.AppendChild(whiteSpaceFEF);
                fef.AppendChild(gradientFEF);
                DataTemplate itemTemplate = new DataTemplate() { VisualTree = fef };
                OptionSelector.ItemTemplate = itemTemplate;
                OptionSelector.SelectedIndex = 0;
                OptionSelector.Height = SelectorHeight;
                OptionSelector.SetBinding(WidthProperty, widthBinding);
                OptionSelector.ItemsSource = presets;
                OptionSelector.SelectionChanged += OptionSelector_SelectionChanged;
                Children.Add(OptionSelector);
            }



            KeyDown += HUI_GradientEditor_KeyDown;

            MouseDown += Canvas_MouseDown;
            UpdateGradient();
        }

        public HUI_GradientEditor() : base()
        {
            Initialize(false, true, new List<HUI_Gradient>());
        }

        private void OptionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            HUI_Gradient selectedGradient = e.AddedItems[0] as HUI_Gradient;
            handles = LoadFromGradient(selectedGradient);
            UpdateGradient();
        }



        private List<GradientHandle> LoadFromGradient(HUI_Gradient selectedGradient)
        {
            foreach (GradientHandle existingHandle in handles)
            {
                MainCanvas.Children.Remove(existingHandle.handleEllipse);
            }

            List<GradientHandle> newHandles = new List<GradientHandle>();
            foreach (GradientStop stop in selectedGradient.Stops)
            {
                newHandles.Add(new GradientHandle(stop.Offset, stop.Color, this));
            }
            return newHandles;
        }

        private static List<HUI_Gradient> GeneratePresets()
        {
            HUI_Gradient BW = new HUI_Gradient(new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(0,0,0),0),
                     new GradientStop(Color.FromRgb(255,255,255),1)
                });
            HUI_Gradient RGB = new HUI_Gradient(new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(255,0,0),0),
                     new GradientStop(Color.FromRgb(0,255,0),0.5),
                     new GradientStop(Color.FromRgb(0,0,255),1)
                });
            HUI_Gradient RYG = new HUI_Gradient(new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(255,0,0),0),
                     new GradientStop(Color.FromRgb(255,255,0),0.5),
                     new GradientStop(Color.FromRgb(0,255,0),1)
                });
            List<HUI_Gradient> presets = new List<HUI_Gradient>() { BW, RGB, RYG };
            return presets;
        }

        private VisualBrush TransparencyGrid()
        {
            VisualBrush vb = new VisualBrush();
            vb.TileMode = TileMode.Tile;
            vb.Viewport = new Rect(0, 0, 10, 10);
            vb.ViewportUnits = BrushMappingMode.Absolute;
            Canvas c = new Canvas();
            Rectangle tile1 = new Rectangle() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)) };
            Rectangle tile2 = new Rectangle() { Width = 5, Height = 5, Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)) };
            Canvas.SetLeft(tile2, 5);
            Canvas.SetTop(tile2, 5);
            c.Children.Add(tile1);
            c.Children.Add(tile2);
            vb.Visual = c;



            return vb;
        }

        private void HUI_GradientEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                handles.ForEach(h => h.ClosePopup());
            }
        }

        public void UpdateGradient()
        {
            gradientDisplay.Fill = new LinearGradientBrush(GradientStops, new Point(0, 0), new Point(1, 0));
            OnPropertyChanged("Gradient Changed");
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Rectangle)) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //    MessageBox.Show("We gotcha");
                handles.Add(new GradientHandle(e.GetPosition(gradientDisplay).X / gradientDisplay.ActualWidth, Color.FromRgb(255, 0, 0), this));
                UpdateGradient();
            }
        }
        public GradientStopCollection GradientStops
        {
            get
            {
                GradientStopCollection coll = new GradientStopCollection();
                foreach (var handle in handles)
                {
                    if (!handle.Hide)
                    {
                        coll.Add(handle.GradientStop);
                    }


                }
                return coll;
            }
        }





    }



}
