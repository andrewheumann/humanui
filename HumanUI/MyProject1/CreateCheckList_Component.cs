using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HumanUI
{
    public class CreateCheckList_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateCheckList_Component class.
        /// </summary>
        public CreateCheckList_Component()
            : base("Create Checklist", "Checklist",
                "Creates a listbox containing checkboxes.",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Checklist Items", "L", "The initial list of options to display in the checklist.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Selected", "S", "The initially selected state of all boxes. Defaults to unchecked for all.", GH_ParamAccess.list, false);
            pManager.AddNumberParameter("Height", "H", "Optional checklist box height in pixels.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Checklist", "CL", "The checklist object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> listItems = new List<string>();
            List<bool> selected = new List<bool>();
            double height = 100;
            if (!DA.GetDataList<string>("Checklist Items", listItems)) return;
            
            DA.GetDataList<bool>("Selected", selected);
            ScrollViewer sv = new ScrollViewer();
            sv.CanContentScroll = true;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            ItemsControl ic = new ItemsControl();

            if(DA.GetData<double>("Height", ref height)) {
                sv.Height = height;
            }
            for (int i = 0; i < listItems.Count;i++ )
            {
                string item = listItems[i];
                bool isSelected = selected[i % selected.Count];
                CheckBox cb = new CheckBox();
                cb.Margin = new System.Windows.Thickness(2);
                cb.Content = item;
                cb.IsChecked = isSelected;
                ic.Items.Add(cb);
            }
            sv.Content = ic;
            DA.SetData("Checklist", new UIElement_Goo(sv, "Checklist"));
      
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
                return Properties.Resources.createChecklist;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6e21dbe5-ecb8-4530-8a22-7cd713cf40d5}"); }
        }
    }
}