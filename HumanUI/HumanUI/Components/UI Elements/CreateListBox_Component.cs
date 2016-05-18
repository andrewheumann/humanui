using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a "List Box" / Scrollable selector
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateListBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateListBox_Component()
            : base("Create List Box", "ListBox",
                "Creates a list box from which items can be selected.",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("List Items", "L", "The initial list of options to display in the list.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Selected Index", "I", "The initially selected index. Defaults to the first item.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Height", "H", "List box height in pixels.", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("List Box", "LB", "The list box object", GH_ParamAccess.item);
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }



        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
           
            List<string> listItems = new List<string>();
            int selectedIndex = 0;
            double height = 100;
            if (!DA.GetDataList<string>("List Items", listItems)) return;
            DA.GetData<double>("Height", ref height);
            DA.GetData<int>("Selected Index",ref selectedIndex);

            //Initialize the list box
            ListBox lb = new ListBox();
            lb.Height = height;
            //for all the strings, add a new textbox as an item to the list box
            foreach (string item in listItems)
            {
                TextBlock textbox = new TextBlock();
                textbox.Text = item;
                lb.Items.Add(textbox);
            }
            //set the selected index
            lb.SelectedIndex = selectedIndex;
            //pass out the listbox object
            DA.SetData("List Box", new UIElement_Goo(lb, "List Box", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreateListBox;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2dddb05e-5503-4506-8f9e-5c0f4c35f8b0}"); }
        }
    }
}