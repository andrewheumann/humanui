using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_IO.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HumanUI.Properties;
using MahApps.Metro.Controls;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Output
{
    public class SetButton_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetButton_Component class.
        /// </summary>
        public SetButton_Component()
          : base("Set Button", "SetBtn",
                "Change the content of an existing Button element.",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Button to modify", "B", "The button object to modify", GH_ParamAccess.item);
            pManager.AddTextParameter("Button Name", "N", "The new text to display on the button", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Image Path", "I", "The new image to display on the button.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object ButtonObject = null;
            
            string newButtonName = "";
            string newButtonImage = "";
            if (!DA.GetData<object>("Button to modify", ref ButtonObject)) return;
            bool hasText = DA.GetData<string>("Button Name", ref newButtonName);
            bool hasIcon = DA.GetData<string>("Image Path", ref newButtonImage);

            Button btn = HUI_Util.GetUIElement<Button>(ButtonObject);

            if (btn == null) return;

            if (!hasText && !hasIcon) return;
            
            //Initialize the button
            //Button btn = new Button();
            
            //make button not focusable
            //btn.Focusable = false;

            UpdateButton(newButtonName, newButtonImage, hasText, hasIcon, btn);


        }

        protected static void UpdateButton(string name, string imagePath, bool hasText, bool hasIcon, Button btn)
        {
            //Initialize a stackPanel to be contained inside the button
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            if (hasIcon)
            {
                //get img from file path, and if the button has an icon, add the image to the stack panel.
                Image img = new Image();
                Uri filePath = new Uri(imagePath);
                BitmapImage bi = new BitmapImage(filePath);
                img.Source = bi;
                sp.Children.Add(img);
            }
            if (hasText)
            {
                //if the button has associated text, create a text block and add it to the stack panel
                TextBlock l = new TextBlock();
                l.Text = name;
                sp.Children.Add(l);
            }
            //put the stack panel inside the button
            btn.Content = sp;


            //Retrieve the MahApps.Metro style dictionary
           // ResourceDictionary ControlsResDict = new ResourceDictionary();
           // ControlsResDict.Source =
           //new Uri("/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute);

           // //based on the user selected button style, assign the appropriate style to the button
           // switch (bs)
           // {
           //     case buttonStyle.Default:
           //         btn.Style = new Style(typeof(Button), (Style)ControlsResDict["MetroButton"]);
           //         break;
           //     case buttonStyle.Square:
           //         btn.Style = new Style(typeof(Button), (Style)ControlsResDict["SquareButtonStyle"]);
           //         break;
           //     case buttonStyle.Circle:
           //         btn.Style = new Style(typeof(Button), (Style)ControlsResDict["MetroCircleButtonStyle"]);
           //         break;
           //     case buttonStyle.Borderless:
           //         // this one could probably be made to look a little better. I'm using a cheap trick that basically
           //         // insets the padding by two pixels in order to hide the border.
           //         btn.Margin = new Thickness(0);
           //         btn.Padding = new Thickness(-2);
           //         btn.BorderThickness = new Thickness(0);
           //         btn.BorderBrush = Brushes.Transparent;
           //         break;

           // }

            //Measure the size of the stackpanel inside the button, size button accordingly
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            sp.Measure(size);
            //sp.Margin = new Thickness(2);
            btn.Width = sp.DesiredSize.Width + 20;
            //btn.Margin = new Thickness(4);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.SetButton;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f7b661aa-71b0-483e-acd6-95663c78d7df"); }
        }
    }
}