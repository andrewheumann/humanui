using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Containers
{
    public class CreateExpander_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateExpander_Component class.
        /// </summary>
        public CreateExpander_Component()
          : base("Create Expander", "Expander",
              "A collapsible expander for content",
              "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to put in the expander", GH_ParamAccess.list);
            pManager.AddTextParameter("Header", "H", "The text to display in the expander header", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Open", "O", "The starting state of the expander - true for open, false for collapsed", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Expander", "E", "The expander element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool open = true;
            string header = "";
            DA.GetData<bool>("Open", ref open);
            DA.GetData<string>("Header", ref header);
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
          
            if (!DA.GetDataList<UIElement_Goo>("UI Elements", elementsToAdd)) return;

            Expander e = new Expander();
            StackPanel sp = new StackPanel();
            sp.Name = "GH_Stack";

            foreach (UIElement_Goo u in elementsToAdd)
            {
                //Make sure it's not in some other container or window
                HUI_Util.removeParent(u.element);
                sp.Children.Add(u.element);
            }

            e.IsExpanded = open;
            e.Content = sp;
            e.Header = header;
            DA.SetData("Expander", new UIElement_Goo(e, "Expander", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.Expander;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3cec9da4-eb68-4063-9325-57850921a8b2}"); }
        }
    }
}