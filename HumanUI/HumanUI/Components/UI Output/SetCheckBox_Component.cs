using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// Component to set the contents of an existing TextBox object
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetCheckBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetLabel_Component class.
        /// </summary>
        public SetCheckBox_Component()
            : base("Set CheckBox", "SetCheckBox",
                "Modify an existing Check Box object.",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Check Box to modify", "CB", "The check box object to modify", GH_ParamAccess.item);
            pManager[pManager.AddTextParameter("New Check Box Label", "L", "The new label to display next to the check box", GH_ParamAccess.item)].Optional = true;
            pManager[pManager.AddBooleanParameter("New Value", "V", "The new value (checked/unchecked) for the box.", GH_ParamAccess.item)].Optional = true;
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
            object CheckBoxObject = null;
            bool isSelected = false;
           
            string newLabelContents = "";
            bool SetLabel=DA.GetData<string>("New Check Box Label", ref newLabelContents);
            if (!DA.GetData<object>("Check Box to modify", ref CheckBoxObject)) return;
            bool SetChecked = DA.GetData<bool>("New Value", ref isSelected);
            // Since HUI textboxes are actually stackpanels with textboxes inside (since they may or may not also contain a button)
            // we have to grab the stackpanel first and then find the textbox inside it. 
            //Panel sp = HUI_Util.GetUIElement<Panel>(TextBlockObject);
            //TextBox tb = HUI_Util.findTextBox(sp);
            CheckBox cb = HUI_Util.GetUIElement<CheckBox>(CheckBoxObject);
            //

            if (cb != null)
            {
                //set the text of the textbox. 
               if(SetLabel) cb.Content = newLabelContents;
               if(SetChecked) cb.IsChecked = isSelected;
            }


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetCheckbox;

        /// <summary>        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{59b523a4-d32c-4a21-883c-a9cb828bf880}");
    }
}