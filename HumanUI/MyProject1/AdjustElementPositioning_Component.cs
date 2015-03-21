using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows;
using HumanUIBaseApp;

namespace HumanUI
{
    public class AdjustElementPositioning_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AdjustElementPositioning_Component class.
        /// </summary>
        public AdjustElementPositioning_Component()
            : base("Adjust Element Positioning", "AdjustPos",
                "Adjust the margins and other positioning information of an element. \nAbsolute positioning can get a little wonky, use at your own risk.",
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
            Param_Integer horizAlign = (Param_Integer)pManager[5];
            horizAlign.AddNamedValue("Left", 0);
            horizAlign.AddNamedValue("Center", 1);
            horizAlign.AddNamedValue("Right", 2);
            pManager.AddIntegerParameter("Vertical Alignment", "VA", "Horizontal alignment", GH_ParamAccess.item);
            pManager[6].Optional = true;
            Param_Integer vertAlign = (Param_Integer)pManager[6];
            vertAlign.AddNamedValue("Bottom", 0);
            vertAlign.AddNamedValue("Center", 1);
            vertAlign.AddNamedValue("Top", 2);
			
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
            FrameworkElement f = HUI_Util.GetUIElement<FrameworkElement>(elem);

            if (DA.GetData<string>("Margin", ref margin)) f.Margin = thicknessFromString(margin);
            MainWindow m = Window.GetWindow(f) as MainWindow;

            if (DA.GetData<double>("Width", ref width))
            {
                f.Width = width;
            }
            if (DA.GetData<double>("Height", ref height))
            {
                f.Height = height;
            }

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
                    default:
                        break;

                }
                f.HorizontalAlignment = alignment;

            }


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

        Thickness thicknessFromString(string margin)
        {
            if (margin.Contains(","))
            {
                string[] vals = margin.Split(',');
                if (vals.Length == 4)
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
            {
                double tempVal = 0;
                if(Double.TryParse(margin,out tempVal)){
                    return new Thickness(tempVal);
                }
               
            }
             AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Trouble parsing the margin input. Try a single value or A,B,C,D format.");
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