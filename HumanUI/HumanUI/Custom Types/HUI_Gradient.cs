using Grasshopper.GUI.Gradient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HumanUI
{
    public class HUI_Gradient
    {

        public static HUI_Gradient FromGHGradient(GH_Gradient g)
        {
            List<Color> cols = new List<Color>();
            List<double> ts = new List<double>();
            var gType = typeof(GH_Gradient);

            var Flags = BindingFlags.Instance
                        | BindingFlags.GetProperty
                        | BindingFlags.SetProperty
                        | BindingFlags.GetField
                        | BindingFlags.SetField
                        | BindingFlags.NonPublic;

            var fields = gType.GetFields(Flags);
            var gripsField = fields.Where(f => f.Name == "m_grips").FirstOrDefault();
            var grips = gripsField.GetValue(g) as List<GH_Grip>;

            foreach (var grip in grips)
            {
                cols.Add(HUI_Util.ToMediaColor(grip.ColourLeft));
                ts.Add(grip.Parameter);
            }
            return new HUI_Gradient(ts, cols);
        }

        public Rectangle Rectangle
        {
            get; set;
        }
        public GradientStopCollection Stops { get; set; }

        public LinearGradientBrush Brush => new LinearGradientBrush(Stops, new Point(0, 0), new Point(1, 0));

        public HUI_Gradient(GradientStopCollection stops)
        {
            Stops = stops;
            Rectangle = new Rectangle();
            Rectangle.Height = 12;
            Rectangle.Width = 10;
            Rectangle.Fill = Brush;
        }
        public HUI_Gradient(List<double> T, List<Color> Color)
        {
            Stops = new GradientStopCollection();
            if (T.Count() == Color.Count())
            {
                for (int i = 0; i < T.Count(); i++)
                {
                    Stops.Add(new GradientStop(Color[i], T[i]));
                }
            }

            Rectangle = new Rectangle();
            Rectangle.Height = 12;
            Rectangle.Width = 100;
            Rectangle.Fill = Brush;
        }


        public override string ToString()
        {
            string gradientString = "";
            foreach (var stop in Stops)
            {
                gradientString += stop.Color.A + "," + stop.Color.R + "," + stop.Color.G + "," + stop.Color.B + "," + stop.Offset.ToString() + Environment.NewLine;
            }
            return gradientString;
        }

        public static HUI_Gradient FromString(string GradientString)
        {
            List<Color> cols = new List<Color>();
            List<double> ts = new List<double>();
            string[] lines = GradientString.Split(Environment.NewLine.ToCharArray());
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line) && !String.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        string[] bits = line.Split(',');

                        Color c = Color.FromArgb(byte.Parse(bits[0]), byte.Parse(bits[1]), byte.Parse(bits[2]), byte.Parse(bits[3]));
                        Double t = Double.Parse(bits[4]);
                        cols.Add(c);
                        ts.Add(t);
                    }
                    catch { }

                }
            }

            return new HUI_Gradient(ts, cols);
        }
    }
}
