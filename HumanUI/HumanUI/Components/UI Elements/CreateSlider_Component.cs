using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a slider object, wrapped in a dock panel and containing labels indicating the name and current value
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
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
            //reset the slider index if this is the first time solveinstance is called for this run cycle.
            if (DA.Iteration == 0)
            {
                sliderIndex = 0;
            }

            List<Label> sliderLabels = new List<Label>();
            //container to hold the UIElement_Goo objects wrapping each slider
            List<UIElement_Goo> sliderPanels = new List<UIElement_Goo>();
            //get the GH doc
            GH_Document doc = OnPingDocument();
            if (doc == null) return;

            // iterate over doc activeObjects
            foreach (IGH_ActiveObject ao in doc.ActiveObjects())
            {
                // If it's a slider that's connected to this component
                if (DependsOn(ao) && ao is GH_NumberSlider)
                {
                    // Because we're actually outputting a list of objects (unlike most other UI element components) we have to
                    // calc the output index ourselves.
                    sliderPanels.Add(new UIElement_Goo(MakeSlider(ao as GH_NumberSlider, ref sliderLabels), (ao as GH_NumberSlider).ImpliedNickName, InstanceGuid, sliderIndex));
                    sliderIndex++;
                }
            }

            
            //this sequence makes sure that all the slider labels fed through one component align with one another

            double width = 0;
            foreach (Label l in sliderLabels)
            {
                //measure the label
                l.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
                if (width < l.DesiredSize.Width)
                {
                    //update width if it's larger than the current known
                    width = l.DesiredSize.Width;
                }
            }
            //set the width of all the labels to be wide enough to accommodate the widest one.  
            foreach (Label l in sliderLabels)
            {
                l.Width = width;
            }
            //pass out the sliders
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

        /// <summary>
        /// Makes the DockPanel containing the slider and labels from the GH_NumberSlider it's passed.
        /// </summary>
        /// <param name="slider">The slider.</param>
        /// <param name="labelList">The label list.</param>
        /// <returns>A DockPanel containing the slider and labels.</returns>
        private DockPanel MakeSlider(GH_NumberSlider slider, ref List<Label> labelList)
        {
            int decimalPlaces = slider.Slider.DecimalPlaces;
            string name = slider.ImpliedNickName;
            if (String.IsNullOrWhiteSpace(name) || name.Length == 0) name = "Slider";
            return createNewSliderWithLabels(slider.Slider.Minimum, slider.Slider.Maximum, slider.Slider.Value, name, slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Integer,decimalPlaces, ref labelList);
        }

        /// <summary>
        /// Override to accept decimal values instead of doubles
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="startVal">The start value.</param>
        /// <param name="name">The name.</param>
        /// <param name="integerSlider">if set to <c>true</c> snap to integers.</param>
        /// <param name="decPlaces">The decimal places.</param>
        /// <param name="sliderLabels">The slider labels.</param>
        /// <returns></returns>
        public DockPanel createNewSliderWithLabels(Decimal min, Decimal max, Decimal startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels)
        {
            return createNewSliderWithLabels((double)min, (double)max, (double)startVal, name, integerSlider, decPlaces, ref sliderLabels);
        }

        /// <summary>
        /// Creates a dockpanel containing the new slider with labels.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="startVal">The start value.</param>
        /// <param name="name">The name of the slider.</param>
        /// <param name="integerSlider">if set to <c>true</c>, snap to integer values.</param>
        /// <param name="decPlaces">The decimal places.</param>
        /// <param name="sliderLabels">The slider labels.</param>
        /// <returns>a Dockpanel containing the slider and labels</returns>
        public DockPanel createNewSliderWithLabels(double min, double max, double startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels)
        {
            //initialize slider
            Slider slider = new Slider();
            //set min, starting val, and max
            slider.Minimum = min;
            slider.Value = startVal;
            slider.Maximum = max;
            //if Integer Slider, set ticks and snap to them
            if (integerSlider)
            {
                slider.TickFrequency = 1.0;
                slider.IsSnapToTickEnabled = true;

            }

            //Create a label for the name of the slider
            Label label = new Label();
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            label.Content = name;

            //Pass it out to slider labels so its width can be reconciled later
            sliderLabels.Add(label);

            //create a label for the slider readout
            Label readout = new Label();
            readout.Content = "";
            //bind it to the slider Value
            Binding myBinding = new Binding("Value");
            myBinding.Source = slider;
            readout.SetBinding(Label.ContentProperty, myBinding);
            //format the readout number values
            readout.ContentStringFormat = integerSlider ? "{0:0}" : String.Concat("{0:F", decPlaces, "}");
            //set up a dockpanel to contain the three elements: name label, slider, and readout label. 
            DockPanel internalPanel = new DockPanel();
            internalPanel.MinWidth = 200;
            internalPanel.Height = 32;
            internalPanel.Margin = new Thickness(4);
            //set to stretch so it fills available horizontal width
            internalPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            //add the name label and dock it left
            internalPanel.Children.Add(label);
            DockPanel.SetDock(label, Dock.Left);
            //dock the readout to the right and add it
            DockPanel.SetDock(readout, Dock.Right);
            internalPanel.Children.Add(readout);
            //add the slider last - the last item added will fill the available space. 
            internalPanel.Children.Add(slider);
            internalPanel.Name = "GH_Slider"; //this key is used in other methods to figure out that the panel is to be interpreted as a slider.
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