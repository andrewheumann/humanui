using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;

namespace HumanUI.Components.UI_Main
{
    public class HideShowElement_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HideShowElement_Component class.
        /// </summary>
        public HideShowElement_Component()
          : base("Hide/Show Element", "HideShow",
              "Allows you to hide or show an element ",
              "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element to Hide/Show", "E", "The elements to hide/show", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show", "S", "Set to true to show the element, false to hide", GH_ParamAccess.item,true);
            pManager.AddBooleanParameter("Collapse", "C", "If true, no space is reserved for the element - if false, the space remains but the element hides", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Disable", "D", "If true, item will appear \"greyed out\" and a user will be unable to interact with it.", GH_ParamAccess.item, false);
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
            bool show = true;
            bool collapse = true;
            object elem = null;
            bool disable = false;

            if (!DA.GetData<bool>("Show", ref show)) return;
            DA.GetData<bool>("Collapse", ref collapse);
            bool hasDisable = DA.GetData<bool>("Disable", ref disable);


            if (!DA.GetData<object>("Element to Hide/Show", ref elem)) return;

            //Get the "FrameworkElement" (basic UI element) from an object
            FrameworkElement f = HUI_Util.GetUIElement<FrameworkElement>(elem);

            if (hasDisable) f.IsEnabled = !disable;

            f.Visibility = show ? Visibility.Visible : (collapse ? Visibility.Collapsed : Visibility.Hidden);
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.HideShowElement;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{a3c49442-c136-4553-9a0d-637d5fbf27d4}");
    }
}