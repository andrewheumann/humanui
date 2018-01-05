using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace HumanUI
{
    public class ClickableShapeGrid : Grid
    {

        public enum ClickMode { ButtonMode, ToggleMode, PickerMode, None };

        public ClickMode clickMode;

        public List<bool> SelectedStates { get; set; }

        public ClickableShapeGrid()
            : base()
        {
            clickMode = ClickMode.None;
            SelectedStates = new List<bool>();

            AddHandler(Grid.MouseMoveEvent, new MouseEventHandler(gridMouseMove));
            AddHandler(Grid.MouseLeaveEvent, new MouseEventHandler(gridMouseMove));
        }

        protected override void OnVisualChildrenChanged(System.Windows.DependencyObject visualAdded, System.Windows.DependencyObject visualRemoved)
        {

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            SelectedStates.Clear();
            foreach (var o in this.Children)
            {
                SelectedStates.Add(false);
            }
            if (clickMode == ClickMode.None) return;
            if (visualAdded is Path p)
            {
                p.AddHandler(Path.MouseDownEvent, new MouseButtonEventHandler(pathMouseDown));
                p.AddHandler(Path.MouseUpEvent, new MouseButtonEventHandler(pathMouseUp));
                // ColorToneEffect tint = new ColorToneEffect(){ DarkColor=(Color)ColorConverter.ConvertFromString( "#FFCBCBCB"), LightColor = (Color)ColorConverter.ConvertFromString("#FF787878"), ToneAmount=0.04, Desaturation = 0.06};
                DropShadowEffect glow = new DropShadowEffect() { BlurRadius = 3, Color = Color.FromRgb(0, 0, 0), ShadowDepth = 0, Opacity = 0 };
                p.Effect = glow;
                //   tint.ToneAmount = 0;
            }
            if (visualRemoved is Path pr)
            {
                pr.RemoveHandler(Path.MouseDownEvent, new MouseButtonEventHandler(pathMouseDown));
                pr.RemoveHandler(Path.MouseUpEvent, new MouseButtonEventHandler(pathMouseUp));
            }
        }

        private void gridMouseMove(object sender, MouseEventArgs e)
        {
            if (clickMode == ClickMode.None) return;
            if (clickMode == ClickMode.ButtonMode)
            {
                if(e.LeftButton != MouseButtonState.Pressed)
                {
                    SelectNone();
                }
                
            }
            MouseMoveOpacityCheck();


        }

        private void SelectNone()
        {
            for (int i = 0; i < SelectedStates.Count; i++)
            {
                SelectedStates[i] = false;
            }
        }

        private void MouseMoveOpacityCheck()
        {
            foreach (Path p in this.Children.OfType<Path>())
            {
                if (p.IsMouseOver)
                {
                    p.Opacity = 0.8;
                }
                else
                {
                    int index = Children.IndexOf(p);
                    p.Opacity = SelectedStates[index] ? .9 : 1;
                }
            }
        }

        private void pathMouseUp(object sender, MouseButtonEventArgs e)
        {
            Path p = sender as Path;
            int index = Children.IndexOf(p);
            switch (clickMode)
            {
                case ClickMode.ButtonMode:
                    SelectedStates[index] = false;

                    break;
                case ClickMode.PickerMode:
                case ClickMode.ToggleMode:
                case ClickMode.None:
                    break;
            }
            updateAppearance(p, index);

        }

        private void pathMouseDown(object sender, MouseButtonEventArgs e)
        {
            Path p = sender as Path;
            int index = Children.IndexOf(p);
            switch (clickMode)
            {
                case ClickMode.ButtonMode:
                    SelectedStates[index] = true;

                    break;
                case ClickMode.PickerMode:
                    SelectNone();
                    SelectedStates[index] = true;
                    break;
                case ClickMode.ToggleMode:
                    SelectedStates[index] = !SelectedStates[index];
                    break;
                case ClickMode.None:
                    break;
            }

            updateAppearance(p, index);
        }

        public void UpdateAppearance()
        {
            foreach (Path p in Children.OfType<Path>())
            {
                int index = Children.IndexOf(p);
                updateAppearance(p, index);
            }
            UpdateLayout();
            MouseMoveOpacityCheck();
   
          
        }

        private void updateAppearance(Path p, int index)
        {

            DropShadowEffect d = p.Effect as DropShadowEffect;
            d.Opacity = SelectedStates[index] ? 1 : 0;

            p.UpdateLayout();
        }



    }
}
