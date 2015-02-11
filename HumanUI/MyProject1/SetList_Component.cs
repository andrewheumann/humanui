using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI
{
    public class SetList_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetList_Component()
            : base("Set List Contents", "SetList",
                "Use this to set the contents of either a List Box or a Pulldown Menu",
                "Human", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List to modify", "L", "The list object to modify", GH_ParamAccess.item);
            pManager.AddTextParameter("New list contents", "C", "The new items to display in the label", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Selected Index", "I", "The optional index to select in the updated list", GH_ParamAccess.item);
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
            int selectedIndex = -1;
            if (!DA.GetData<object>("List to modify", ref ListObject)) return;
            if (!DA.GetDataList<string>("New list contents", listContents)) return;
            
            Selector sel = HUI_Util.GetUIElement<Selector>(ListObject);

             if (!DA.GetData<int>("Selected Index", ref selectedIndex)) selectedIndex = sel.SelectedIndex;


            sel.Items.Clear();
            foreach (string item in listContents)
            {
                Label label = new Label();
                label.Content = item;
                sel.Items.Add(label);
            }

            sel.SelectedIndex = selectedIndex;
            





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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d98497d6-c164-4499-aedb-78d04c09eba4}"); }
        }
    }
}