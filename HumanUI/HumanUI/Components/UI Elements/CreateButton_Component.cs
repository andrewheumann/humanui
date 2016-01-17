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

namespace HumanUI.Components.UI_Elements
{

    /// <summary>
    /// This represents the possible button styles, selectable from a user pulldown
    /// </summary>
    public enum buttonStyle { Default, Square, Circle, Borderless};


    /// <summary>
    /// A component to create a simple button object with either Text or Image content
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateButton_Component : GH_Component
    {

        protected buttonStyle bs = buttonStyle.Default;


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

        /// <summary>
        /// Special constructor for classes that extend this one
        /// </summary>
        public CreateButton_Component(string name, string nickname, string description)
            : base(name, nickname,
                description,
                "Human", "UI Elements")
        {
            UpdateMenu();
        }



        /// <summary>
        /// Updates the black message tag with the current button style.
        /// </summary>
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


        /// <summary>
        /// Adds the button style options to the component right-click menu. 
        /// </summary>
        /// <param name="menu"></param>
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


        /// <summary>
        /// Handles the Make Default Style event of the menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void menu_makeDefaultStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Default;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Handles the Make Square Style event of the menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void menu_makeSquareStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Square;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }


        /// <summary>
        /// Handles the Make Circle Style event of the menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void menu_makeCircleStyle(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Circle;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Handles the Make Borderless event of the menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void menu_makeBorderless(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Button Style Change");
            this.bs = buttonStyle.Borderless;
            this.UpdateMenu();
            this.ExpireSolution(true);
        }



        /// <summary>
        /// Write all required data for deserialization to an IO archive. In this case it's just the button style
        /// </summary>
        /// <param name="writer">Object to write with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("ButtonStyle", (int)bs);
            return base.Write(writer);
        }
        /// <summary>
        /// Read all required data for deserialization from an IO archive. - In this case just the button style. 
        /// </summary>
        /// <param name="reader">Object to read with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
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
            //Initialize the button
            Button btn = new Button();
            //make button not focusable
            btn.Focusable = false;
            
            SetupButton(name, imagePath, hasText, hasIcon, btn, bs);         
            //pass out the button
            DA.SetData("Button", new UIElement_Goo(btn, name,InstanceGuid,DA.Iteration));

        }

        protected static void SetupButton(string name, string imagePath, bool hasText, bool hasIcon, Button btn, buttonStyle bs)
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
            ResourceDictionary ControlsResDict = new ResourceDictionary();
            ControlsResDict.Source =
           new Uri("/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute);

            //based on the user selected button style, assign the appropriate style to the button
            switch (bs)
            {
                case buttonStyle.Default:
                    btn.Style = new Style(typeof(Button), (Style)ControlsResDict["MetroButton"]);
                    break;
                case buttonStyle.Square:
                    btn.Style = new Style(typeof(Button), (Style)ControlsResDict["SquareButtonStyle"]);
                    break;
                case buttonStyle.Circle:
                    btn.Style = new Style(typeof(Button), (Style)ControlsResDict["MetroCircleButtonStyle"]);
                    break;
                case buttonStyle.Borderless:
                    // this one could probably be made to look a little better. I'm using a cheap trick that basically
                    // insets the padding by two pixels in order to hide the border.
                    btn.Margin = new Thickness(0);
                    btn.Padding = new Thickness(-2);
                    btn.BorderThickness = new Thickness(0);
                    btn.BorderBrush = Brushes.Transparent;
                    break;

            }

            //Measure the size of the stackpanel inside the button, size button accordingly
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            sp.Measure(size);
            sp.Margin = new Thickness(2);
            btn.Width = sp.DesiredSize.Width + 20;
            btn.Margin = new Thickness(4);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
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