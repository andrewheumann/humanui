using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Windows.Media;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Xceed.Wpf.Toolkit;

namespace HumanUI.Components.UI_Output
{
    public class SetColorPicker_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetColorPicker_Component class.
        /// </summary>
        public SetColorPicker_Component()
          : base("Set Color Picker", "SetColorPicker",
                "Use this to set a color picker",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //The first input should be the UI element itself -as direct output from
            // the corresponding "Create" Component
            pManager.AddGenericParameter("Color picker to modify", "CP", "The Color Picker object to modify", GH_ParamAccess.item);
            pManager.AddColourParameter("Default Color", "D", "The color displayed on the picker. Optional.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddColourParameter("Available Colors", "C", "An optional list of possible colors to limit the user's selection. \nIf left blank, the existing set of available colors will not be changed.", GH_ParamAccess.list);
            pManager[2].Optional = true;

            //Add any additional variables necessary to modify the content of the element.
            //These usually look an awful lot like the Input Params of the "Create" component.
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //"Set" components usually do not have any outputs.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a basic object to hold the UI Element prior to type conversion
            object myUIElement = null;
            System.Drawing.Color defaultCol = System.Drawing.Color.Transparent;
            List<System.Drawing.Color> availableCols = new List<System.Drawing.Color>();

            if (!DA.GetData<object>(0, ref myUIElement)) return;

            ColorPicker picker = HUI_Util.GetUIElement<ColorPicker>(myUIElement);
            

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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("77ec992b-a0d8-440c-b9ad-91098a40cd78"); }
        }
    }
}