using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows;


namespace HumanUI
{
    public class CreateTextBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateTextBox_Component class.
        /// </summary>
        public CreateTextBox_Component()
            : base("Create Text Box", "TextBox",
                "Create a box for text entry, with a button to pass its value.",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Label", "L", "Optional label for the Text Box", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Default Text", "D", "The starting text in the text box", GH_ParamAccess.item,"");
            pManager.AddBooleanParameter("Update Button", "U", "Set to true to associate text box \nwith a button for updates. Otherwise event listening will \nassociate with every change in text box content.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Text Box", "TB", "The created text box.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool includeButton = true;
            string label = "";
            string defaultValue = "";
            if(!DA.GetData<string>("Default Text", ref defaultValue)) return;
            DA.GetData<string>("Label",ref label);
            DA.GetData<bool>("Update Button", ref includeButton);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            TextBox tb = new TextBox();
            tb.Text = defaultValue;
            tb.Width = 200;
            Button b = new Button();
            b.Width = 50;
            sp.Margin = new Thickness(4);
            Label l = new Label();
            l.Content = label;
            sp.Children.Add(l);
            sp.Children.Add(tb);
            if (includeButton)
            {
                sp.Children.Add(b);
                sp.Name = "GH_TextBox";
            }
            else
            {
                sp.Name = "GH_TextBox_NoButton";
            }
            
            DA.SetData("Text Box", new UIElement_Goo(sp,String.Format("TextBox: {0}",label)));
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
                return Properties.Resources.CreateTextBox;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c8d203fe-7e84-416a-b93e-d1bd746f3f66}"); }
        }
    }
}