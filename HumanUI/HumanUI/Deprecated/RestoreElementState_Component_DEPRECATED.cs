using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;

namespace HumanUI
{
    /// <exclude />
    public class RestoreElementState_Component_DEPRECATED : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RestoreElementState_Component_DEPRECATED class.
        /// </summary>
        public RestoreElementState_Component_DEPRECATED()
            : base("Restore Element States", "Restore",
                "Restore the saved states of UI elements",
                "Human", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Restore", "R", "Set to true to restore the state. \nProbably not a good idea to leave this set to true while you're adding new states.", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("Saved States", "SS", "The collection of saved states", GH_ParamAccess.item);
            pManager.AddTextParameter("State Name to restore", "N", "The name of the state to restore", GH_ParamAccess.item);
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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool restore = false;
            string setName = "";
            StateSet_Goo states = null;
            if (!DA.GetData<StateSet_Goo>("Saved States", ref states)) return;
            if (!DA.GetData<string>("State Name to restore", ref setName)) return;
            DA.GetData<bool>("Restore", ref restore);
            if (!restore) return;
            State stateToRestore = states.states[setName];
            //restore state
            foreach (KeyValuePair<UIElement_Goo, object> elementState in stateToRestore.stateDict)
            {
                //    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, string.Format("Setting element {0} to {1}", HUI_Util.extractBaseElement(elementState.Key.element).ToString(), elementState.Value));
                UIElement element = elementState.Key.element;

                var parent = System.Windows.Media.VisualTreeHelper.GetParent(element);
                //    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Parent is: "+parent.ToString());

                if (parent == null)
                {
                    Guid id = elementState.Key.instanceGuid;
                    int index = elementState.Key.index;

                    UIElement_Goo newGoo = Components.UI_Main.SaveElementState_Component.getElementGoo(this.OnPingDocument(), id, index);
                    element = newGoo.element;
                }

                HUI_Util.TrySetElementValue(HUI_Util.extractBaseElement(element), elementState.Value);
            }


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
                return Properties.Resources.RestoreState;
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d106b262-7a20-4151-b59a-872300f7ee9c}"); }
        }
    }
}