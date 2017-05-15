using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// A component to set the contents of an existing Checklist object
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetChecklist_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetChecklist_Component()
            : base("Set Checklist Contents", "SetChecklist",
                "Use this to set the contents of a checklist",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Checklist to modify", "L", "The list object to modify", GH_ParamAccess.item);
            pManager.AddTextParameter("New checklist contents", "C", "The new items to display in the checklist", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Selected", "S", "The optional boolean values to control if items are selected", GH_ParamAccess.list);
            pManager[2].Optional = true;
 

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
            object ListObject = null;
            List<string> listContents = new List<string>();
            List<bool> isSelected = new List<bool>();
            if (!DA.GetData<object>("Checklist to modify", ref ListObject)) return;
            if (!DA.GetDataList<string>("New checklist contents", listContents)) return;
            
            //Get the scroll viewer
            ScrollViewer sv = HUI_Util.GetUIElement<ScrollViewer>(ListObject);
            //Get the itemsControl inside the scrollviewer
            ItemsControl ic = sv.Content as ItemsControl;

            if (!DA.GetDataList<bool>("Selected", isSelected))
            {

            }
            
            //clear the items in there now
            ic.Items.Clear();
          
            //set the new items
            for (int i = 0; i < listContents.Count; i++)
            {
                string item = listContents[i];
               
                CheckBox cb = new CheckBox();
                cb.Margin = new System.Windows.Thickness(2);
                cb.Content = item;
                if (isSelected.Count > 0)
                {
                    // this is a cheap shortcut - but allows an approximation of "shortest list" behavior for cases of N and 1. If only one true is  
                    // passed, all boxes true, if a list is passed, will behave as expected. Only list-length mismatches will behave differently - and if 
                    // you're doing that, we have bigger problems anyway.
                    bool isSel = isSelected[i % isSelected.Count]; 
                    cb.IsChecked = isSel;
                }
                ic.Items.Add(cb);
            }
           
          



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetCheckList;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{11A5A354-FC1B-4CEA-894A-BBEB71A23DB5}");
    }
}