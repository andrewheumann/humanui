using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grasshopper.Kernel.Special;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a Combobox ("Pulldown menu")
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreatePullDown_Component_DEPRECATED : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreatePullDown_Component_DEPRECATED()
            : base("Create Pulldown Menu", "Pulldown",
                "Creates a pulldown menu from which items can be selected.",
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
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        public override bool Obsolete
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pulldown", "PD", "The pulldown object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


          
           
            List<string> listItems = new List<string>();
            int selectedIndex = 0;

            if (!DA.GetDataList<string>("List Items", listItems)) return;
            DA.GetData<int>("Selected Index",ref selectedIndex);
            //initialize combobox
            ComboBox pd = new ComboBox();
          //for each string add a label object to the combobox
            foreach (string item in listItems)
            {
                TextBlock label = new TextBlock();
                label.Text = item;
                pd.Items.Add(label);
            }
            pd.Margin = new Thickness(4);
            pd.SelectedIndex = selectedIndex;

            //pass out the combobox
            DA.SetData("Pulldown", new UIElement_Goo(pd, "Pulldown", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreatePullDown;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1CA8D537-EF52-487C-828D-034B1BCA7361}"); }
        }
    }
}