using System;
using System.Collections.Generic;
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
    public class CreateSlider_Component : GH_Component
    {

        private bool showTicks;
        private bool showTooltip;
        private bool showValueReadout;
        private bool showBounds;
        /// <summary>
        /// Initializes a new instance of the CreateSliderComponent class.
        /// </summary>
        public CreateSlider_Component()
            : base("Create Slider", "Slider",
                "Create a slider with a label and a value readout.",
                "Human UI", "UI Elements")
        {
            showTicks = false;
            showTooltip = false;
            showValueReadout = true;
            showBounds = false;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Slider", "Sl", "The slider(s) to add to the window.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Snap Value", "Sn", "An optional value to round/snap slider to. This overrides the native settings on the GH slider.", GH_ParamAccess.list);
            pManager[1].Optional = true;

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
            GH_Structure<IGH_Goo> slidersAsObjects = new GH_Structure<IGH_Goo>();
            List<double> snapValues = new List<double>();

            //an optional value to store slider snapping
            if (!DA.GetDataList<double>("Snap Value", snapValues))
            {
                snapValues.Add(-1);
            }

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

            //lazy iterator for the snap values.
            int i = 0;

            //container for sliders
            var attachedSliders = new List<GH_NumberSlider>();

            try
            {
                //Get all sliders attached to the first parameter
                attachedSliders = Params.Input[0].Sources.Cast<GH_NumberSlider>().ToList();


            }
            catch
            {
                //Assume that sliders were not connected, try to read slider objects from the wrappers instead:
                DA.GetDataTree<IGH_Goo>("Slider", out slidersAsObjects);
                foreach (IGH_Goo goo in slidersAsObjects)
                {
                    if (goo is GH_ObjectWrapper)
                    {
                        GH_ObjectWrapper w = goo as GH_ObjectWrapper;
                        attachedSliders.Add(w.Value as GH_NumberSlider);
                    }
                    
                }

            }

            foreach (GH_NumberSlider sl in attachedSliders)
            {

                // Because we're actually outputting a list of objects (unlike most other UI element components) we have to
                // calc the output index ourselves.
                sliderPanels.Add(new UIElement_Goo(MakeSlider(sl, ref sliderLabels, snapValues[i % snapValues.Count]), sl.ImpliedNickName, InstanceGuid, sliderIndex));
                sliderIndex++;

                // increment snapvalue index
                i++;
            }


            //this sequence makes sure that all the slider labels fed through one component align with one another

            double width = 0;
            foreach (Label l in sliderLabels)
            {
                //measure the label
                l.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
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
        /// Add options for tick marks, the value readout, and tooltips to the component Menu.
        /// </summary>
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem ticksMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Enable Ticks", new System.EventHandler(this.menu_enableTicks), true, showTicks);
            ticksMenuItem.ToolTipText = "Display ticks below the slider.";
            System.Windows.Forms.ToolStripMenuItem tooltipMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Enable Tooltip", new System.EventHandler(this.menu_enableTooltip), true, showTooltip);
            tooltipMenuItem.ToolTipText = "Display a tooltip above the slider displaying the value.";
            System.Windows.Forms.ToolStripMenuItem valueLabelMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Enable Value Label", new System.EventHandler(this.menu_enableValueLabel), true, showValueReadout);
            valueLabelMenuItem.ToolTipText = "Display a label to the right of the slider showing its current value.";
            System.Windows.Forms.ToolStripMenuItem showBoundsMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Show Slider Limits", new System.EventHandler(this.menu_showBounds), true, showBounds);
            showBoundsMenuItem.ToolTipText = "Display a label to the right of the slider showing its current value.";

        }

        private void menu_showBounds(object sender, EventArgs e)
        {
            RecordUndoEvent("Toggle Slider Bounds Display");
            showBounds = !showBounds;
            this.ExpireSolution(true);
        }

        private void menu_enableValueLabel(object sender, EventArgs e)
        {
            RecordUndoEvent("Toggle Slider Value Label");
            showValueReadout = !showValueReadout;
            this.ExpireSolution(true);
        }

        private void menu_enableTooltip(object sender, EventArgs e)
        {
            RecordUndoEvent("Toggle Slider Value Tooltip");
            showTooltip = !showTooltip;
            this.ExpireSolution(true);
        }

        private void menu_enableTicks(object sender, EventArgs e)
        {
            RecordUndoEvent("Toggle Slider Tick Display");
            showTicks = !showTicks;
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Makes the DockPanel containing the slider and labels from the GH_NumberSlider it's passed.
        /// </summary>
        /// <param name="slider">The slider.</param>
        /// <param name="labelList">The label list.</param>
        /// <returns>A DockPanel containing the slider and labels.</returns>
        private DockPanel MakeSlider(GH_NumberSlider slider, ref List<Label> labelList, double snapValue)
        {
            int decimalPlaces = slider.Slider.DecimalPlaces;
            string name = slider.ImpliedNickName;
            if (String.IsNullOrWhiteSpace(name) || name.Length == 0) name = "Slider";
            return createNewSliderWithLabels(slider.Slider.Minimum, slider.Slider.Maximum, slider.Slider.Value, name, slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Integer, decimalPlaces, ref labelList, snapValue);
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
        public DockPanel createNewSliderWithLabels(Decimal min, Decimal max, Decimal startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels, double snapValue)
        {
            return createNewSliderWithLabels((double)min, (double)max, (double)startVal, name, integerSlider, decPlaces, ref sliderLabels, snapValue);
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
        public DockPanel createNewSliderWithLabels(double min, double max, double startVal, string name, bool integerSlider, int decPlaces, ref List<Label> sliderLabels, double snapValue)
        {
            //initialize slider
            Slider slider = new Slider();
            //set min, starting val, and max
            slider.Minimum = min;
            slider.Value = startVal;
            slider.Maximum = max;

            //make it not focusable
            slider.Focusable = false;

            if (showTooltip)
            {
                slider.AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.TopLeft;
                slider.AutoToolTipPrecision = decPlaces;
            }

            //if Integer Slider, set ticks and snap to them
            if (integerSlider)
            {
                slider.TickFrequency = 1.0;
                slider.IsSnapToTickEnabled = true;
                if (showTooltip) slider.AutoToolTipPrecision = 0;

            }

            if (snapValue > 0)
            {
                slider.TickFrequency = snapValue;
                slider.IsSnapToTickEnabled = true;
            }

            if (showTicks) slider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;

            //Create a label for the name of the slider
            Label label = new Label();
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            label.Content = name;

            //Pass it out to slider labels so its width can be reconciled later
            sliderLabels.Add(label);



            //set up a dockpanel to contain the three elements: name label, slider, and readout label. 
            DockPanel internalDockPanel = new DockPanel();
            internalDockPanel.MinWidth = 200;
            internalDockPanel.Height = showBounds ? 40 : 32;
            internalDockPanel.Margin = new Thickness(4);
            //set to stretch so it fills available horizontal width
            internalDockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            //add the name label and dock it left
            internalDockPanel.Children.Add(label);
            DockPanel.SetDock(label, Dock.Left);

            //establish format string for labels
            string numberFormat = integerSlider ? "{0:0}" : String.Concat("{0:F", decPlaces, "}");

            //Set up a value entry box for doubleclick
            SliderEntryTextBox ValueEntryBox = new SliderEntryTextBox(slider);
            slider.AddHandler(Slider.MouseDoubleClickEvent, new MouseButtonEventHandler(ValueEntryBox.TriggerAction), true);
           
            


            if (showValueReadout) //if user has opted to show the value label
            {
                //create a label for the slider readout
                Label readout = new Label();

                //set to max to establish theoretical max width to avoid jumping
                readout.Content = max;

                //format the readout number values
                readout.ContentStringFormat = numberFormat;

                //establish desired size;
                readout.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                //set the width to approx max width
                readout.Width = readout.DesiredSize.Width;

                //bind it to the slider Value
                Binding myBinding = new Binding("Value");
                myBinding.Source = slider;

                readout.SetBinding(Label.ContentProperty, myBinding);
                //dock the readout to the right and add it
                DockPanel.SetDock(readout, Dock.Right);
                internalDockPanel.Children.Add(readout);
            }
            //Create a grid to contain the slider, and any under-slider labels
            Grid SliderGrid = new Grid();
            if (showBounds)
            {
                Label lowerBound = new Label();
                Label upperBound = new Label();

                lowerBound.Height = 40;
                upperBound.Height = 40;

                lowerBound.Content = String.Format(numberFormat, min);
                upperBound.Content = String.Format(numberFormat, max);

                lowerBound.FontSize = 10;
                upperBound.FontSize = 10;
                lowerBound.VerticalContentAlignment = VerticalAlignment.Bottom;
                upperBound.VerticalContentAlignment = VerticalAlignment.Bottom;

                upperBound.HorizontalAlignment = HorizontalAlignment.Right;
                upperBound.HorizontalContentAlignment = HorizontalAlignment.Right;

                lowerBound.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                upperBound.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                //inset slightly so labels sit more directly below ticks
                slider.Margin = new Thickness((lowerBound.DesiredSize.Width / 2) - 5, 0, (upperBound.DesiredSize.Width / 2) - 5, 0);
                SliderGrid.Children.Add(lowerBound);
                SliderGrid.Children.Add(upperBound);


            }

            // Add the slider to the grid - if it goes last it won't lose its interaction
            SliderGrid.Children.Add(ValueEntryBox);
            SliderGrid.Children.Add(slider);

            //add the slider last - the last item added will fill the available space.
            internalDockPanel.Children.Add(SliderGrid);
            internalDockPanel.Name = "GH_Slider"; //this key is used in other methods to figure out that the panel is to be interpreted as a slider.
            return internalDockPanel;
        }






        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{C77ACC8A-FE64-43F0-9485-D23744F6152E}"); }
        }



        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("ShowTicks", showTicks);
            writer.SetBoolean("ShowTooltip", showTooltip);
            writer.SetBoolean("ShowValLabel", showValueReadout);
            writer.SetBoolean("ShowBounds", showBounds);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            showTicks = reader.GetBoolean("ShowTicks");
            showTooltip = reader.GetBoolean("ShowTooltip");
            showValueReadout = reader.GetBoolean("ShowValLabel");
            showBounds = reader.GetBoolean("ShowBounds");
            return base.Read(reader);
        }
    }
}