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

    


    public class CreateRhinoCmdButton_Component : GH_Component
    {

        private buttonStyle bs = buttonStyle.Default;


        /// <summary>
        /// Initializes a new instance of the CreateRhinoCmdButton_Component class.
        /// </summary>
        public CreateRhinoCmdButton_Component()
            : base("Create Rhino Command Button", "CmdButton",
                "Create a Special Button object to trigger a Rhino command.",
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
                case buttonStyle.Borderless:
                    Message = "Borderless";
                    break;
                default:
                    break;
            }

        }


        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Default Style", new System.EventHandler(this.menu_makeDefaultStyle), true, bs == buttonStyle.Default);
            toolStripMenuItem.ToolTipText = "Use the default button style.";
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1 = GH_DocumentObject.Menu_AppendItem(menu, "Square Style", new System.EventHandler(this.menu_makeSquareStyle), true, bs == buttonStyle.Square);
            toolStripMenuItem1.ToolTipText = "Use a flat, square button style.";
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Circle Style", new System.EventHandler(this.menu_makeCircleStyle), true, bs == buttonStyle.Circle);
            toolStripMenuItem2.ToolTipText = "Use a circle (or ellipse) button style.";
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem(menu, "Borderless Style", new System.EventHandler(this.menu_makeBorderless), true, bs == buttonStyle.Borderless);
            toolStripMenuItem3.ToolTipText = "Use a borderless button style.";

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

        private void menu_makeBorderless(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Borderless;
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
            pManager.AddTextParameter("Rhino Script", "S", "The command line script to execute on button press", GH_ParamAccess.item);
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
            string localCmd = "";
            if (DA.GetData<string>("Rhino Script", ref localCmd))
            {
                commandString = localCmd;
            }

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
                case buttonStyle.Borderless:
                    // Style btnStyle = (Style)Application.Current.FindResource(ToolBar.ButtonStyleKey);
                    //  style.BasedOn = (Style)FindResource(ToolBar.ButtonStyleKey);
                    //  btn.Style = new Style(typeof(Button), btnStyle);
                    // btn.Padding = new Thickness(-5);
                    btn.Margin = new Thickness(0);
                    btn.Padding = new Thickness(-2);
                    btn.BorderThickness = new Thickness(0);
                    btn.BorderBrush = Brushes.Transparent;


                    break;

            }


            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            sp.Measure(size);
            sp.Margin = new Thickness(2);
            btn.Width = sp.DesiredSize.Width + 20;
            btn.Margin = new Thickness(4);
            btn.Click += ExecuteCommand;
            DA.SetData("Button", new UIElement_Goo(btn, name, InstanceGuid, DA.Iteration));

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
                return Properties.Resources.CreateRhinoButton;
            }
        }

        string commandString = "";

        protected void ExecuteCommand(object Sender, EventArgs e)
        {
            Rhino.RhinoApp.RunScript(commandString, false);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{58AEE14D-8214-4E76-8D51-3432CD30B3AC}"); }
        }
    }
}