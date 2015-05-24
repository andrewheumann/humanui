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
using MahApps.Metro.Controls;
using HumanUIBaseApp;

namespace HumanUI
{

    enum buttonStyle { Default, Square, Circle };


    public class CreateButton_Component : GH_Component, HUI_Expirable
    {

        private buttonStyle bs = buttonStyle.Default;


        /// <summary>
        /// Initializes a new instance of the CreateButton_Component class.
        /// </summary>
        public CreateButton_Component()
            : base("Create Button", "Button",
                "Create a Button object.",
                "Human", "UI Elements")
        {
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            switch (bs)
            {
                case buttonStyle.Default:
                    Message = "";
                    break;
                case buttonStyle.Square:
                    Message = "Square Style";
                    break;
                case buttonStyle.Circle:
                    Message = "Circle Style";
                    break;
                default:
                    break;
            }

        }


        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Default Style", new System.EventHandler(this.menu_makeDefaultStyle), true, bs == buttonStyle.Default);
            toolStripMenuItem.ToolTipText = "When selected, the window is made a child of the Grasshopper window - when the Grasshopper window is hidden or minimized, it will disappear.";
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1 = GH_DocumentObject.Menu_AppendItem(menu, "Square Style", new System.EventHandler(this.menu_makeSquareStyle), true, bs == buttonStyle.Square);
            toolStripMenuItem1.ToolTipText = "When selected, the window is made a child of the Rhino window - when the Rhino window is hidden or minimized, it will disappear.";
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Circle Style", new System.EventHandler(this.menu_makeCircleStyle), true, bs == buttonStyle.Circle);
            toolStripMenuItem2.ToolTipText = "When selected, the window is always on top, floating above other apps.";
        }


        private void menu_makeDefaultStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Default;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }

        private void menu_makeSquareStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Square;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }


        private void menu_makeCircleStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Circle;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }


        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("ButtonStyle", (int)bs);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            int readVal = -1;
            reader.TryGetInt32("ButtonStyle", ref readVal);
            bs = (buttonStyle)readVal;
            this.UpdateMenu();
            return base.Read(reader);
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Button Name", "N", "The text to display on the button", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Image Path", "I", "The image to display on the button.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Button", "B", "The created Button", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Button";
            string imagePath = "";
            bool hasText = DA.GetData<string>("Button Name", ref name);
            bool hasIcon = DA.GetData<string>("Image Path", ref imagePath);
            if (!hasText && !hasIcon) return;
            Button btn = new Button();
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            if (hasIcon)
            {
                Image img = new Image();
                Uri filePath = new Uri(imagePath);
                BitmapImage bi = new BitmapImage(filePath);
                img.Source = bi;
                sp.Children.Add(img);
            }
             if (hasText)
            {
                TextBlock l = new TextBlock();
                l.Text = name;
                sp.Children.Add(l);
            }

             btn.Content = sp;
       
          

             ResourceDictionary ControlsResDict = new ResourceDictionary();
             ControlsResDict.Source =
            new Uri("/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute);

             switch (bs)
             {
                 case buttonStyle.Default:
                     break;
                 case buttonStyle.Square:
                     btn.Style = new Style(typeof(Button), (Style)ControlsResDict["SquareButtonStyle"]);
                     break;
                 case buttonStyle.Circle:
                     btn.Style = new Style(typeof(Button), (Style)ControlsResDict["MetroCircleButtonStyle"]);
                     break;

             }

             

            //btn.Style = MainWindow.getStyleByName("SquareButtonStyle");

            //TODO: figure out how to calculate the width of content and assign it as the button width.
          //   btn.MaxWidth = sp.ExtentWidth;
            btn.Margin = new Thickness(4);

            DA.SetData("Button", new UIElement_Goo(btn, name));

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
                return Properties.Resources.CreateButton;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9A5B87D6-046E-4C33-9ACA-5AF2F7503047}"); }
        }
    }
}