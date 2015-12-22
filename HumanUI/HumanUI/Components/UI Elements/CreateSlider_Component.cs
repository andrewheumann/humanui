using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HumanUI
{
    public class CreateSlider_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateSliderComponent class.
        /// </summary>
        public CreateSlider_Component()
            : base("Create Slider", "Slider",
                "Create a slider with a label and a value readout.",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Slider", "S", "The slider(s) to add to the window.", GH_ParamAccess.tree);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Sliders", "S", @"The Slider UI elements. Use in conjunction with an ""Add Elements"" component.", GH_ParamAccess.list);
        }

        int sliderIndex = 0;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
            {
                sliderIndex = 0;
            }
            List<Label> sliderLabels = new List<Label>();
            List<UIElement_Goo> sliderPanels = new List<UIElement_Goo>();
            GH_Document doc = OnPingDocument();
            if (doc == null) return;

            foreach (IGH_ActiveObject ao in doc.ActiveObjects())
            {
                if (DependsOn(ao) && ao is GH_NumberSlider)
                {
                    sliderPanels.Add(new UIElement_Goo(MakeSlider(ao as GH_NumberSlider, ref sliderLabels), (ao as GH_NumberSlider).ImpliedNickName, InstanceGuid, sliderIndex));
                    sliderIndex++;
                }
            }


            double width = 0;
            foreach (Label l in sliderLabels)
            {
                
                l.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
                if (width < l.DesiredSize.Width)
                {
                    width = l.DesiredSize.Width;
                }
            }
            foreach (Label l in sliderLabels)
            {
                l.Width = width;
            }

            DA.SetDataList("Sliders", sliderPanels);
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
                return Properties.Resources.CreateSlider;
            }
        }

        private DockPanel MakeSlider(GH_NumberSlider slider, ref List<Label> labelList)
        {
            int decimalPlaces = slider.Slider.DecimalPlaces;
            string name = slider.ImpliedNickName;
            if (String.IsNullOrWhiteSpace(name) || name.Length == 0) name = "Slider";
            return createNewSliderWithLabels(slider.Slider.Minimum, slider.Slider.Maximum, slider.Slider.Value, name, slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Integer,decimalPlaces, ref labelList);
        }

        public DockPanel createNewSliderWithLabels(Decimal min, Decimal max, Decimal startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels)
        {
            return createNewSliderWithLabels((double)min, (double)max, (double)startVal, name, integerSlider, decPlaces, ref sliderLabels);
        }

        public DockPanel createNewSliderWithLabels(double min, double max, double startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels)
        {
            Slider slider = new Slider();
            slider.Minimum = min;
            slider.Value = startVal;
            slider.Maximum = max;
            if (integerSlider)
            {
                slider.TickFrequency = 1.0;
                slider.IsSnapToTickEnabled = true;

            }
            Label label = new Label();
        //    label.MinWidth = 100;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            label.Content = name;

            sliderLabels.Add(label);

            Label readout = new Label();
            readout.Content = "";
            Binding myBinding = new Binding("Value");
            myBinding.Source = slider;
            readout.SetBinding(Label.ContentProperty, myBinding);

            readout.ContentStringFormat = integerSlider ? "{0:0}" : String.Concat("{0:F", decPlaces, "}");
            DockPanel internalPanel = new DockPanel();
            internalPanel.MinWidth = 200;
            internalPanel.Height = 32;
            internalPanel.Margin = new Thickness(4);
            internalPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            internalPanel.Children.Add(label);
            DockPanel.SetDock(label, Dock.Left);
            DockPanel.SetDock(readout, Dock.Right);
            internalPanel.Children.Add(readout);
            
            internalPanel.Children.Add(slider);
            internalPanel.Name = "GH_Slider";
            return internalPanel;
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{E4F276AF-46FB-478E-B10A-B95E7B04DFF0}"); }
        }
    }
}