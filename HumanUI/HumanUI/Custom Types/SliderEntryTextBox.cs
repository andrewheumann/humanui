using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HumanUI
{
    public class SliderEntryTextBox : TextBox
    {
        internal Slider slider;


        public SliderEntryTextBox(Slider s)
            : base()
        {
            slider = s;
            Visibility = System.Windows.Visibility.Hidden;
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            Panel.SetZIndex(this, -1);
            Binding heightBinding = new Binding();
            heightBinding.Source = slider;
            heightBinding.Path = new System.Windows.PropertyPath("ActualHeight");
            heightBinding.Mode = BindingMode.OneWay;
            this.SetBinding(TextBox.HeightProperty, heightBinding);
            this.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(valueEntry_KeyDown));
            this.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(escape));
        }

        private void escape(object sender, RoutedEventArgs e)
        {
            HideEntryBox();
        }

        public void TriggerAction(object sender, MouseButtonEventArgs e)
        {
            ShowEntryBox();
        }

        private void valueEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetVal();
            }

            if (e.Key == Key.Escape)
            {
                HideEntryBox();
            }
        }

        public void ShowEntryBox()
        {
            Text = slider.Value.ToString();
            Visibility = System.Windows.Visibility.Visible;
            Focus();
            SelectAll();
            Grid.SetZIndex(this, 3);
        }


        private void SetVal()
        {
            double value = double.NaN;

            if (double.TryParse(this.Text, out value))
            {
                if (value > slider.Maximum)
                {
                    slider.Value = slider.Maximum;
                }
                if (value < slider.Minimum)
                {
                    slider.Value = slider.Minimum;
                }
                if (value < slider.Maximum && value > slider.Minimum)
                {
                    slider.Value = value;
                }

            }

            HideEntryBox();
        }

        private void HideEntryBox()
        {
            Visibility = System.Windows.Visibility.Hidden;
            Grid.SetZIndex(this, -1);
            Text = slider.Value.ToString();
        }


    }
}
