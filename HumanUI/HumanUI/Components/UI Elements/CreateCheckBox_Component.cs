using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// A component to create a single checkbox object
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateCheckBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateCheckBox_Component class.
        /// </summary>
        public CreateCheckBox_Component()
            : base("Create Checkbox", "Checkbox",
                "Creates a single checkbox",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Label", "L", "The label for the checkbox.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Starting Value", "V", "The starting value (checked/unchecked) for the box.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Checkbox", "CB", "The created checkbox.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool isSelected = false;
            string label = "";
            if (!DA.GetData<string>("Label", ref label)) return;
            DA.GetData<bool>("Starting Value", ref isSelected);

            //create the checkbox object
            CheckBox cb = new CheckBox();
            cb.Margin = new System.Windows.Thickness(2);
            cb.Content = label;
            cb.IsChecked = isSelected;
            //pass out the checkbox object
            DA.SetData("Checkbox", new UIElement_Goo(cb, String.Format("Checkbox: {0}", label), InstanceGuid, DA.Iteration));

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
                return Properties.Resources.CreateCheckbox;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c2c5cc88-9812-4769-b482-bd6f32697836}"); }
        }
    }
}