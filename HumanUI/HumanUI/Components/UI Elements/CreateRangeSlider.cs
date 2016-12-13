using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// This file is designed as a template for components that handle creation of HUI Elements.
    /// </summary>
    public class CreateRangeSlider : GH_Component
    {
        /// <summary>
        /// This file is designed as a template for components that handle creation of HUI Elements.
        /// </summary>
        public CreateRangeSlider()
            : base("Create Range Slider", "RangeSlider",
                "Creates a double-slider that describes a range",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("Slider Range", "R",
                "The range that defines the min and max of the slider extents", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Starting Range", "SR", "The initial value selected on the slider",
                GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Range Slider", "RS", "The Range Slider Element.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Interval sliderRange = Interval.Unset;
            Interval startingRange = Interval.Unset;

            if (!DA.GetData("Slider Range", ref sliderRange)) return;
            if (!DA.GetData("Starting Range", ref startingRange)) return;
            RangeSlider rs = new RangeSlider
            {
                MinRangeWidth = 0,
                Minimum = sliderRange.Min,
                Maximum = sliderRange.Max,
                LowerValue = startingRange.Min,
                UpperValue = startingRange.Max,
                AutoToolTipPlacement = AutoToolTipPlacement.BottomRight
            };

            DA.SetData("Range Slider", new UIElement_Goo(rs, "Range Slider", InstanceGuid, DA.Iteration));

            

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateRangeSlider;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{FF8EBC79-B1CE-430B-8734-E1993F7E477F}");
    }
}