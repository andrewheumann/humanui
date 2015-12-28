using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Windows.Media;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Xceed.Wpf.Toolkit;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a customizable color picker from the Xceed WPF toolkit
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateColorPicker_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateColorPicker_Component class.
        /// </summary>
        public CreateColorPicker_Component()
            : base("Create Color Picker", "ColorPicker",
                "Creates an interactive color picker, with an optionally supplied set of colors",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Default Color", "D", "The color displayed on the picker at load. Optional.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddColourParameter("Available Colors", "C", "An optional list of possible colors to limit the user's selection. \nIf left blank, the standard set of colors will display.", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Color Picker", "P", @"The Color Picker UI element. Use in conjunction with an ""Add Elements"" component.", GH_ParamAccess.item);
     
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            System.Drawing.Color defaultCol = System.Drawing.Color.Transparent;
            List<System.Drawing.Color> availableCols = new List<System.Drawing.Color>();

            //initialize the picker with default settings
            ColorPicker picker = new ColorPicker();


            //If the user specified a default color, set the picker's selected color.
            if (DA.GetData<System.Drawing.Color>("Default Color", ref defaultCol))
            {
                picker.SelectedColor = HUI_Util.ToMediaColor(defaultCol);
            }


            //if the user specified a list of allowed colors, replace the picker's availableColors list and disable standard colors.
            if (DA.GetDataList<System.Drawing.Color>("Available Colors", availableCols))
            {
                ObservableCollection<ColorItem> cols = new ObservableCollection<ColorItem>();
                foreach (System.Drawing.Color cw in availableCols)
                {
                    Color c = HUI_Util.ToMediaColor(cw);
                    cols.Add(new ColorItem(c, String.Format("{0},{1},{2}", c.R, c.B, c.G)));
                    picker.AvailableColors = cols;
                    picker.ShowStandardColors = false;
                }
            }


            DA.SetData("Color Picker", new UIElement_Goo(picker, "Color Picker", InstanceGuid, DA.Iteration));

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
                return Properties.Resources.ColorPicker;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0c914dd3-ed91-4255-9c47-0148c126ea25}"); }
        }
    }
}