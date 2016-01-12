using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Main 
{
    /// <summary>
    /// Component to adjust element positioning
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class AdjustElementPositioning_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AdjustElementPositioning_Component class.
        /// </summary>
        public AdjustElementPositioning_Component()
            : base("Adjust Element Positioning", "AdjustPos",
                "Adjust the margins, sizing, and other positioning information of an element. \nAbsolute positioning can get a little wonky, use at your own risk.",
                "Human", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements to adjust", "E", "The UIElement you want to reposition.", GH_ParamAccess.item);
            pManager.AddTextParameter("Margin","M","The margin value. Input a single number to \naffect margins on all sides, or four values separated by commas\nto set Left, Top, Right, and Bottom individually.",GH_ParamAccess.item);
            pManager.AddBooleanParameter("Absolute Positioning", "Abs", "Set to true to position relative to the upper left corner of the document", GH_ParamAccess.item,false);
            pManager.AddNumberParameter("Width", "W", "Override the element width", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Override the element height", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;

            pManager.AddIntegerParameter("Horizontal Alignment", "HA", "Horizontal alignment", GH_ParamAccess.item);
            pManager[5].Optional = true;
            //set up custom params with selectable default values in menu 
            Param_Integer horizAlign = (Param_Integer)pManager[5];
            horizAlign.AddNamedValue("Left", 0);
            horizAlign.AddNamedValue("Center", 1);
            horizAlign.AddNamedValue("Right", 2);
            horizAlign.AddNamedValue("Stretch", 3);

            pManager.AddIntegerParameter("Vertical Alignment", "VA", "Vertical alignment", GH_ParamAccess.item);
            pManager[6].Optional = true;
            Param_Integer vertAlign = (Param_Integer)pManager[6];
            vertAlign.AddNamedValue("Bottom", 0);
            vertAlign.AddNamedValue("Center", 1);
            vertAlign.AddNamedValue("Top", 2);
            vertAlign.AddNamedValue("Stretch", 3);
			
        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
           
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool absolute = false;
            object elem = null;
            string margin = "0";
            double width = 0;
            double height = 0;
            int horizAlign = 0;
            int vertAlign = 0;
            if (!DA.GetData<object>("Elements to adjust", ref elem)) return;
           
            DA.GetData<bool>("Absolute Positioning", ref absolute);

            //This is the element we are adjusting
            FrameworkElement f = HUI_Util.GetUIElement<FrameworkElement>(elem);

            //if user has supplied margin, set margin
            if (DA.GetData<string>("Margin", ref margin)) f.Margin = thicknessFromString(margin);

            //get the window the element belongs to
            MainWindow m = Window.GetWindow(f) as MainWindow;

            //if user specified width and/or height, set them
            if (DA.GetData<double>("Width", ref width))
            {
                f.Width = width;
            }
            if (DA.GetData<double>("Height", ref height))
            {
                f.Height = height;
            }

            //if user specified vertical and/or horizontal alignment, set it
            if (DA.GetData<int>("Vertical Alignment", ref vertAlign))
            {
                VerticalAlignment alignment = f.VerticalAlignment;
                switch (vertAlign) { 
                    case 0:
                        alignment = VerticalAlignment.Bottom;
                        break;
                    case 1:
                        alignment = VerticalAlignment.Center;
                        break;
                    case 2:
                        alignment = VerticalAlignment.Top;
                        break;
                    case 3:
                        alignment = VerticalAlignment.Stretch;
                        break;
                    default:
                        break;

                }
                f.VerticalAlignment = alignment;
            
            }
            if (DA.GetData<int>("Horizontal Alignment", ref horizAlign))
            {
                HorizontalAlignment alignment = f.HorizontalAlignment;
                switch (horizAlign)
                {
                    case 0:
                        alignment = HorizontalAlignment.Left;
                        break;
                    case 1:
                        alignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        alignment = HorizontalAlignment.Right;
                        break;
                    case 3:
                        alignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;

                }
                f.HorizontalAlignment = alignment;

            }

            // The absolute positioning setting is essentially just placing the items as the child of the window Grid, which is the parent of the Master Stack 
            // Panel everything usually sits in. 
            if (absolute)
            {
                try
                {
                    m.MoveFromStackToGrid(f);
                }
                catch 
                {
                  
                }
            }
            else
            {
                try
                {
                    m.MoveFromGridToStack(f);
                }
                catch { }
            }
            

        }


        /// <summary>
        /// Creates a Thickness from string - either 4 numbers separated by commas, or 1 number applied everywhere, to mirror XAML syntax
        /// - or a Point3d interpreted as the upper left corner.
        /// </summary>
        /// <param name="margin">The margin.</param>
        /// <returns>Thickness</returns>
        Thickness thicknessFromString(string margin)
        {
            if(margin.Contains("{")) { //Check if is a point converted to string
                string[] vals = margin.Split(",{}".ToCharArray());
                if (vals.Length == 3)
                {
                    List<double> margins = new List<double>();
                    foreach (string v in vals)
                    {
                        double tempV;
                        Double.TryParse(v, out tempV);
                        margins.Add(tempV);
                    }
                    try
                    {
                        return new Thickness(margins[0], margins[1], 0, 0); //set point coords to left and top values
                    }
                    catch { }
                }
                else if (vals.Length == 5) //I *think* this is to handle the fact that sometimes the string split will give you empty strings in [0] and [4].
                {
                    List<double> margins = new List<double>();
                    foreach (string v in vals)
                    {
                        double tempV = 0;
                        Double.TryParse(v, out tempV);
                        margins.Add(tempV);
                    }
                    try
                    {
                        return new Thickness(margins[1], margins[2], 0, 0);
                    }
                    catch { }
                } else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, String.Format("Trying to parse a point, but got {0} items",vals.Length));
                }
            }
            else if (margin.Contains(","))
            {
                string[] vals = margin.Split(','); 
                if (vals.Length == 4) //assume all values supplied left, top, right, bottom
                {


                    List<double> margins = new List<double>();
                    foreach (string v in vals)
                    {
                        double tempV;
                        Double.TryParse(v, out tempV);
                        margins.Add(tempV);
                    }
                    try
                    {
                        return new Thickness(margins[0], margins[1], margins[2], margins[3]);
                    }
                    catch { }
                }
            }
            else
            { //assume only one value supplied.
                double tempVal = 0;
                if(Double.TryParse(margin,out tempVal)){
                    return new Thickness(tempVal);
                }
               
            } //if none of the tryParses succeeded, you get this error
             AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Trouble parsing the margin input. Try a single value, a Point, or A,B,C,D format.");
                    return new Thickness(0);
            
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
                return Properties.Resources.AdjustPositioning;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0e1bdb06-2fe7-4fbc-b194-15227efdc8a7}"); }
        }
    }
}