using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HumanUI
{
    public class CreateChecklist_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateChecklist_Component()
            : base("Create CheckList", "Checklist",
                "Creates a checklist from which items can be selected.",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("List Items", "L", "The initial list of options to display in the list.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Starting Values", "I", "Boolean values to indicate initial selection for all list items", GH_ParamAccess.list,false);
            }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Checklist", "CL", "The Checklist object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
           
            List<string> listItems = new List<string>();
            List<bool> selectedItems = new List<bool>();
            if (!DA.GetDataList<string>("List Items", listItems)) return;
            DA.GetDataList<bool>("Starting Values", selectedItems);
          
            ListView lv = new ListView();

            int i = 0;
            foreach (string item in listItems)
            {
                CheckBox label = new CheckBox();
                label.Content = item;
                lv.Items.Add(label);
                label.IsChecked = selectedItems[i % selectedItems.Count];
                i++;
            }

            DA.SetData("Checklist", new UIElement_Goo(lv,"Checklist"));
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
            get { return new Guid("{9CDBC907-3316-4A44-ABF9-9126EB54F5E0}"); }
        }
    }
}