using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// Component to modify the contents of an existing Label
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetLabel_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetLabel_Component class.
        /// </summary>
        public SetLabel_Component()
            : base("Set Label Contents", "SetLabel",
                "Modify the contents of an existing label object.",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Label to modify", "L", "The label object to modify", GH_ParamAccess.item);
            pManager.AddTextParameter("New Label contents", "C", "The new text to display in the label", GH_ParamAccess.item);
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
            object LabelObject = null;
            string newLabelContents = "";
            if (!DA.GetData<string>("New Label contents", ref newLabelContents)) return;
            if (!DA.GetData<object>("Label to modify", ref LabelObject)) return;
            Label l = HUI_Util.GetUIElement<Label>(LabelObject);
            //set label content
            if (l != null)
            {
                l.Content = newLabelContents;
            }

          
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetLabel;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{07b9d48c-bfc5-4f49-a449-50ffe4e6d4c7}");
    }
}