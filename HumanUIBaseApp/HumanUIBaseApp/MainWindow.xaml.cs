using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace HumanUIBaseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {


        public MainWindow()
        {
            InitializeComponent();
            clearElements();
        }

        public void setWindowName(string name)
        {
            Title = name;

        }

        public void setDataContext(object o)
        {
            this.DataContext = o;
        }


        public static Style getStyleByName(string name)
        {
            return (Style)Application.Current.FindResource(name);
        }

        public void setFont(string fontName)
        {
            FontFamily ff = new System.Windows.Media.FontFamily(fontName);
            TextElement.SetFontFamily(MasterStackPanel, ff);


           
        }
    

        public int AddElement(UIElement elem){
              return MasterStackPanel.Children.Add(elem);
        }

        public void AddElement(UIElement elem, int index)
        {
            MasterStackPanel.Children.Insert(index, elem);
        }

        public int RemoveFromStack(UIElement elem)
        {
            int i = MasterStackPanel.Children.IndexOf(elem);
            MasterStackPanel.Children.Remove(elem);
            return i;
        }

        public void AddToGrid(UIElement elem, int zIndex)
        {
            AbsPosGrid.Children.Add(elem);
            Grid.SetZIndex(elem, zIndex);
        }

        public int RemoveFromGrid(UIElement elem)
        {
            int i = Grid.GetZIndex(elem);
            AbsPosGrid.Children.Remove(elem);
            return i;
        }

        public void MoveFromStackToGrid(UIElement elem)
        {
            int index = RemoveFromStack(elem);
            AddToGrid(elem, index);
        }

        public void MoveFromGridToStack(UIElement elem)
        {
            int index = RemoveFromGrid(elem);
            AddElement(elem, index);
        }


        public void clearElements()
        {
            MasterStackPanel.Children.Clear();
            AbsPosGrid.Children.Clear();
            AbsPosGrid.Children.Add(MasterStackPanel);
        }

    }
}
