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

        int sliderIndex = 0;

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

            //GH_Structure<IGH_Goo> slidersAsObjects = new GH_Structure<IGH_Goo>();
            //List<double> snapValues = new List<double>();

            ////an optional value to store slider snapping
            //if (!DA.GetDataList<double>("Snap Value", snapValues))
            //{
            //    snapValues.Add(-1);
            //}

            ////reset the slider index if this is the first time solveinstance is called for this run cycle.
            //if (DA.Iteration == 0)
            //{
            //    sliderIndex = 0;
            //}

            //List<Label> sliderLabels = new List<Label>();
            ////container to hold the UIElement_Goo objects wrapping each slider
            //List<UIElement_Goo> sliderPanels = new List<UIElement_Goo>();
            ////get the GH doc
            //GH_Document doc = OnPingDocument();
            //if (doc == null) return;

            ////lazy iterator for the snap values.
            //int i = 0;

            ////container for sliders
            //var attachedSliders = new List<GH_NumberSlider>();

            //try
            //{
            //    //Get all sliders attached to the first parameter
            //    attachedSliders = Params.Input[1].Sources.Cast<GH_NumberSlider>().ToList();


            //}
            //catch
            //{
            //    //Assume that sliders were not connected, try to read slider objects from the wrappers instead:
            //    DA.GetDataTree<IGH_Goo>("GH Slider", out slidersAsObjects);
            //    foreach (IGH_Goo goo in slidersAsObjects)
            //    {
            //        if (goo is GH_ObjectWrapper)
            //        {
            //            GH_ObjectWrapper w = goo as GH_ObjectWrapper;
            //            attachedSliders.Add(w.Value as GH_NumberSlider);
            //        }

            //    }

            //}

            //foreach (GH_NumberSlider sl in attachedSliders)
            //{

            //    // Because we're actually outputting a list of objects (unlike most other UI element components) we have to
            //    // calc the output index ourselves.
            //    sliderPanels.Add(new UIElement_Goo(MakeSlider(sl, ref sliderLabels, snapValues[i % snapValues.Count]), String.IsNullOrWhiteSpace(sl.NickName) ? "" : sl.ImpliedNickName, InstanceGuid, sliderIndex));
            //    sliderIndex++;

            //    // increment snapvalue index
            //    i++;
            //}


            //this sequence makes sure that all the slider labels fed through one component align with one another

            //var width = MaxLabelWidth(sliderLabels);
            ////set the width of all the labels to be wide enough to accommodate the widest one.  
            //foreach (Label l in sliderLabels)
            //{
            //    l.Width = width;
            //}
            ////pass out the sliders
            //DA.SetDataList("Sliders", sliderPanels);
        }

        //return the greatest width from a list of label elements
        //private static double MaxLabelWidth(List<Label> sliderLabels)
        //{
        //    double width = 0;
        //    foreach (Label l in sliderLabels)
        //    {
        //        //measure the label
        //        l.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //        if (width < l.DesiredSize.Width)
        //        {
        //            //update width if it's larger than the current known
        //            width = l.DesiredSize.Width;
        //        }
        //    }
        //    return width;
        //}

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



        //public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        //{
        //    writer.SetBoolean("ShowTicks", showTicks);
        //    writer.SetBoolean("ShowTooltip", showTooltip);
        //    writer.SetBoolean("ShowValLabel", showValueReadout);
        //    writer.SetBoolean("ShowBounds", showBounds);
        //    return base.Write(writer);
        //}

        //public override bool Read(GH_IO.Serialization.GH_IReader reader)
        //{
        //    showTicks = reader.GetBoolean("ShowTicks");
        //    showTooltip = reader.GetBoolean("ShowTooltip");
        //    showValueReadout = reader.GetBoolean("ShowValLabel");
        //    showBounds = reader.GetBoolean("ShowBounds");
        //    return base.Read(reader);
        //}
    }
}