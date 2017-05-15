using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// This file is designed as a template for components that handle creation of HUI Elements.
    /// </summary>
    public class CreateMDSlider_Component : GH_Component
    {
        /// <summary>
        /// This file is designed as a template for components that handle creation of HUI Elements.
        /// </summary>
        public CreateMDSlider_Component()
            : base("Create Multidimensional Slider", "MD Slider",
                "Creates a 2D slider ranging from {0,0} to {1,1}",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Starting Value", "S", "The initial point (between 0,0 and 1,1) you want this slider to start with.", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("MD Slider", "S", "The MD Slider.", GH_ParamAccess.item);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d startPt = Point3d.Unset;

            MDSliderElement mdSlider = new MDSliderElement();
            if (DA.GetData<Point3d>("Starting Value", ref startPt))
            {
                mdSlider.SliderPoint = startPt;
            }
           
        
            DA.SetData("MD Slider", new UIElement_Goo(mdSlider, "Multi-Dimensional Slider", InstanceGuid, DA.Iteration));


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMDSlider;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{2E335FB4-447C-42D4-A10B-A7C9AB882757}");
    }
}