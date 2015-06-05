using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;

namespace HumanUI
{
    public class CreateRadioButton_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateRadioButton_Component class.
        /// </summary>
        public CreateRadioButton_Component()
            : base("Create Radio Button", "RadioBtn",
                "Creates a single radio button. Be sure to assign a radio button group for proper switching behavior",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Radio Button Name", "N", "The name of the radio button.", GH_ParamAccess.item);
            pManager.AddTextParameter("Radio Button Group", "G", "The group the radio button belongs to. \n Only one button in a group can be selected at a time.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Initially Selected", "S", "Whether or not this button should be selected initially.", GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Radio Button", "RB", "The created radio button.", GH_ParamAccess.item);
             }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
              string btnName = "";
            string groupName = "";
            if (!DA.GetData<string>("Radio Button Name", ref btnName)) return;
            if (!DA.GetData<string>("Radio Button Group", ref groupName)) return;
            bool isSelected = false;
            DA.GetData<bool>("Initially Selected", ref isSelected);
            RadioButton rb = new RadioButton();
            rb.Margin = new System.Windows.Thickness(2);
            rb.Content = btnName;
            rb.GroupName = groupName;
            rb.IsChecked = isSelected;

            DA.SetData("Radio Button", new UIElement_Goo(rb, String.Format("Radio Button: {0}", btnName), InstanceGuid, DA.Iteration));
            
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
                return Properties.Resources.CreateRadioButton;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5B7E6AF2-EB03-477A-ABEB-3C0065593296}"); }
        }
    }
}