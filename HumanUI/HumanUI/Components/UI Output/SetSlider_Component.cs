using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Windows.Input;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a slider object, wrapped in a dock panel and containing labels indicating the name and current value
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetSlider_Component : GH_Component
    {

      
        /// <summary>
        /// Initializes a new instance of the CreateSliderComponent class.
        /// </summary>
        public SetSlider_Component()
            : base("Set Slider", "SetSlider",
                "Modify the range and value of a slider.",
                "Human UI", "UI Output")
        {
         
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Slider", "S", "The UI Slider to update", GH_ParamAccess.item);
       //     pManager.AddGenericParameter("GH Slider", "Sl", "The optional grasshopper slider to use to read driving values", GH_ParamAccess.tree);
         //   pManager.AddNumberParameter("Snap Value", "Sn", "An optional value to round/snap slider to. This overrides the native settings on the GH slider.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Value", "V", "The value to set the slider to", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Range", "R", "the new slider range", GH_ParamAccess.item);
            for (int i = 1; i <3; i++)
            {
                pManager[i].Optional = true;
            }


        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

    //    int sliderIndex = 0;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object SliderObject = null;
            Interval range = Interval.Unset;
            double newValue = double.NaN;
            if (!DA.GetData("UI Slider", ref SliderObject)) return;
            var hasRange = DA.GetData("Range", ref range);
            var hasValue = DA.GetData("Value", ref newValue);

            DockPanel dp = HUI_Util.GetUIElement<DockPanel>(SliderObject);

            var slider = dp.Children.OfType<Grid>().First().Children.OfType<Slider>().First();
            if (hasRange)
            {
                slider.Minimum = range.Min;
                slider.Maximum = range.Max;
            }
            if (hasValue)
            {
                slider.Value = newValue;
            }

        }

 

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetSlider;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{B412D7D3-02E2-4A8E-BDCC-2E1F8B2A8834}"); }
        }



    }
}