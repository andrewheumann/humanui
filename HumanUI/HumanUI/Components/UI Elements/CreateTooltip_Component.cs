using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;

namespace HumanUI
{
    public class CreateTooltip_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Create_Tooltip_Component class.
        /// </summary>
        public CreateTooltip_Component()
          : base("Attach Tooltip to Element", "Tooltip",
              "Attach a tooltip to a UI element",
              "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements to Attach to", "E", "The elements to attach the tooltip to", GH_ParamAccess.item);
            pManager.AddGenericParameter("Tooltip Content", "C", "The content (text or elements) to put in the tooltip", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Show Delay","SD","How long in ms before the tooltip displays",GH_ParamAccess.item,500);
            pManager.AddIntegerParameter("Duration", "D", "How long in ms that the tooltip should show", GH_ParamAccess.item, 2000);
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
            object elem = null;
            object contentElem = null;
            int showDelay = 500;
            int duration = 2000;
            if (!DA.GetData<object>("Elements to Attach to", ref elem)) return;
            if (!DA.GetData<object>("Tooltip Content", ref contentElem)) return;

            DA.GetData<int>("Show Delay", ref showDelay);
            DA.GetData<int>("Duration", ref duration);

            FrameworkElement f = HUI_Util.GetUIElement<FrameworkElement>(elem);
            FrameworkElement contentElement = HUI_Util.GetUIElement<FrameworkElement>(contentElem);
            if(contentElement != null)
            {
                f.ToolTip = contentElement;
            } else
            {
                f.ToolTip = contentElem;
            }
            ToolTipService.SetShowDuration(f, duration);
            ToolTipService.SetInitialShowDelay(f, showDelay);
            ToolTipService.SetBetweenShowDelay(f, showDelay);



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
                return Properties.Resources.CreateTooltip;
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{a6e4cefc-ca10-4dc4-9120-696e02e5cfeb}"); }
        }
    }
}