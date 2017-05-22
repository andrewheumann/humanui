using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Windows;
using System.Windows.Controls;

using System.Drawing;
using System.Windows.Shapes;

using System.Windows.Media;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// Component to modify an existing single Shape component. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetShape_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetShape_Component class.
        /// </summary>
        public SetShape_Component()
            : base("Set Shape", "SetShape",
                "Replace an existing shape in the window",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape to Modify", "S", "The shape object to modify.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Shape Curve", "SC", "The shape to add as Polyline(s)", GH_ParamAccess.list);
            pManager.AddColourParameter("Fill Color", "FC", "The fill color. Leave empty for no fill", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stroke Weight", "SW", "The stroke weight. Leave empty or set to 0 for no stroke.", GH_ParamAccess.item);
            pManager.AddColourParameter("Stroke Color", "SC", "The stroke color", GH_ParamAccess.item, System.Drawing.Color.Black);
            pManager.AddNumberParameter("Scale", "Scl", "Use this value to resize the shape.", GH_ParamAccess.item,1.0);
            pManager.AddIntegerParameter("Width", "W", "The width of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Height", "H", "The height of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
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
             object ShapeObject = null;
           
             if (!DA.GetData<object>("Shape to Modify", ref ShapeObject)) return;
             Grid G = HUI_Util.GetUIElement<Grid>(ShapeObject);
            //change the shape
            if (G != null)
            {
                List<Curve> shapeCrvs = new List<Curve>();
                System.Drawing.Color fillCol = System.Drawing.Color.Transparent;
                double strokeWeight = 0.0;
                System.Drawing.Color strokeCol = System.Drawing.Color.Transparent;
                double scale = 1.0;
                int width = 0;
                int height = 0;
                
                //Extract the path from the container grid
                Path path = getPath(G.Children);
                //save the old path
                Path oldpath = path;
                //if the user has supplied new shapes
                if (DA.GetDataList<Curve>("Shape Curve", shapeCrvs))
                {

                    Path newPath = new Path();
                    DA.GetData<double>("Scale", ref scale);
                    //use the method from the orig. component to translate curves into Geometry
                    newPath.Data = Components.UI_Elements.CreateShape_Component.pathGeomFromCrvs(shapeCrvs, scale,true);
                    //remove old path from Grid
                    G.Children.Remove(path);
                   
                    path = newPath;
                    G.Children.Add(path);
                }
                //if the user specified a new fill color
                if (DA.GetData<System.Drawing.Color>("Fill Color", ref fillCol))
                {
                    //set fill
                    path.Fill = new SolidColorBrush(HUI_Util.ToMediaColor(fillCol));
                }
                else
                {
                    //otherwise use the old fill color
                    path.Fill = oldpath.Fill;
                }
                //if user supplied stroke weight
                if (DA.GetData<double>("Stroke Weight", ref strokeWeight))
                {
                    //set the stroke weight
                    path.StrokeThickness = strokeWeight;
                }
                else
                {
                    //otherwise use the old stroke weight
                    path.StrokeThickness = oldpath.StrokeThickness;
                }
                //if the user specified a stroke color
                if (DA.GetData<System.Drawing.Color>("Stroke Color", ref strokeCol))
                {
                    //set the stroke color
                    path.Stroke = new SolidColorBrush(HUI_Util.ToMediaColor(strokeCol));
                }
                else
                {
                    //otherwise use the old stroke color
                    path.Stroke = oldpath.Stroke;
                }

               //if the user specified a new object width
                if (DA.GetData<int>("Width", ref width))
                {
                    //set the grid width
                    G.Width = width;
                }
                else
                {
                    //otherwise use the old grid width
                    G.Width = oldpath.Width;
                }
                //if the user specified a new height
                if (DA.GetData<int>("Height", ref height))
                {
                    //set the height
                    G.Height = height;
                }
                else
                {
                    //otherwise use the old height
                    G.Height = oldpath.Height;
                }
               
            
            }
        
        }


        /// <summary>
        /// Utility method to extract the Path object from a UIElementCollection
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns>The first path found</returns>
        Path getPath(UIElementCollection col)
        {
            foreach (FrameworkElement u in col)
            {
                if (u is Path) return u as Path;
            }
            return null;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetShape;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{f6881435-7de3-4098-ada8-f3068ed7331c}");
    }
}