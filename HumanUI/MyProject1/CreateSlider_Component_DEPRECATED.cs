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
    public class CreateSlider_Component_DEPRECATED : GH_Component, HUI_Expirable
    {
        /// <summary>
        /// Initializes a new instance of the CreateSliderComponent class.
        /// </summary>
        public CreateSlider_Component_DEPRECATED()
            : base("Create Slider", "Slider",
                "Create a slider with a label and a value readout.",
                "Human", "UI Elements")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
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

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<UIElement_Goo> sliderPanels = new List<UIElement_Goo>();
            GH_Document doc = OnPingDocument();
            if (doc == null) return;
            foreach (IGH_ActiveObject ao in doc.ActiveObjects())
            {
                if (DependsOn(ao) && ao is GH_NumberSlider)
                {
                    sliderPanels.Add(new UIElement_Goo(MakeSlider(ao as GH_NumberSlider),(ao as GH_NumberSlider).ImpliedNickName));
                }
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

        private StackPanel MakeSlider(GH_NumberSlider slider)
        {
            string name = slider.ImpliedNickName;
            if (String.IsNullOrWhiteSpace(name)||name.Length==0) name = "Slider";
            return createNewSliderWithLabels(slider.Slider.Minimum, slider.Slider.Maximum, slider.Slider.Value, name,slider.Slider.Type== Grasshopper.GUI.Base.GH_SliderAccuracy.Integer);
        }

        public StackPanel createNewSliderWithLabels(Decimal min, Decimal max, Decimal startVal, string name, bool integerSlider)
        {
           return createNewSliderWithLabels((double)min,(double)max,(double)startVal,name,integerSlider);
        }

        public StackPanel createNewSliderWithLabels(double min, double max, double startVal, string name,bool integerSlider)
        {
            Slider slider = new Slider();
            slider.Minimum = min;
            slider.Value = startVal;
            slider.Maximum = max;
            slider.Width = 200;
            if (integerSlider)
            {
                slider.TickFrequency = 1.0;
                slider.IsSnapToTickEnabled = true;

            }
            Label label = new Label();
            label.Width = 100;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            label.Content = name;
            Label readout = new Label();
            readout.Content = "";
            Binding myBinding = new Binding("Value");
            myBinding.Source = slider;
            readout.SetBinding(Label.ContentProperty, myBinding);
            readout.ContentStringFormat = integerSlider ? "{0:0}" :"{0:0.0}";
            StackPanel internalPanel = new StackPanel();
            internalPanel.Height = 32;
            internalPanel.Margin = new Thickness(4);
          //  internalPanel.Width = MasterStackPanel.Width;
            internalPanel.Orientation = Orientation.Horizontal;
            internalPanel.Children.Add(label);
            internalPanel.Children.Add(slider);
            internalPanel.Children.Add(readout);
            internalPanel.Name = "GH_Slider";
            return internalPanel;
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{16231ddc-6473-42d4-b81f-e0c5e90e8fbd}"); }
        }
    }
}