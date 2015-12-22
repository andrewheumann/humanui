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

namespace HumanUI
{
    public class SetShapes_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetShapes_Component class.
        /// </summary>
        public SetShapes_Component()
            : base("Set Shapes", "SetShapes",
                "Replace an existing shape in the window",
                "Human", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape to Modify", "S", "The shape object to modify.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Shape Curves", "SC", "The shapes to add as Polyline(s)", GH_ParamAccess.list);
            pManager.AddColourParameter("Fill Colors", "FC", "The fill colors. Leave empty for no fill", GH_ParamAccess.list);
            pManager.AddNumberParameter("Stroke Weights", "SW", "The stroke weight. Leave empty or set to 0 for no stroke.", GH_ParamAccess.list);
            pManager.AddColourParameter("Stroke Colors", "SC", "The stroke color", GH_ParamAccess.list, System.Drawing.Color.Black);
            pManager.AddNumberParameter("Scale", "Scl", "Use this value to resize the shape.", GH_ParamAccess.item, 1.0);
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
                List<System.Drawing.Color> fillCol = new List<System.Drawing.Color>();
                List<double> strokeWeights = new List<double>();
                List<System.Drawing.Color> strokeCol = new List<System.Drawing.Color>();
                double scale = 1.0;
                int width = 0;
                int height = 0;
                Path oldpath = getPath(G.Children);
               

                bool hasWeight = false;
                bool hasStrokeCol = false;
                bool hasFill = false;

                DA.GetData<double>("Scale", ref scale);

                if (DA.GetDataList<System.Drawing.Color>("Fill Colors", fillCol))
                {

                    hasFill = true;
                }


                if (DA.GetDataList<double>("Stroke Weights", strokeWeights))
                {

                    hasWeight = true;
                }


                if (DA.GetDataList<System.Drawing.Color>("Stroke Colors", strokeCol))
                {

                    hasStrokeCol = true;
                }
              


                if (DA.GetDataList<Curve>("Shape Curves", shapeCrvs))
                {
                    G.Children.Clear();

                    int i = 0;
                    foreach (Curve c in shapeCrvs)
                    {



                        Path path = new Path();
                        List<Curve> crvList = new List<Curve>();
                        crvList.Add(c);
                        path.Data = CreateShape_Component.pathGeomFromCrvs(crvList, scale, false);
                        if (hasFill)
                        {

                            path.Fill = new SolidColorBrush(HUI_Util.ToMediaColor(fillCol[i % fillCol.Count]));
                        }
                        else
                        {
                            path.Fill = oldpath.Fill;
                        }

                        if (hasWeight)
                        {

                            path.StrokeThickness = strokeWeights[i % strokeWeights.Count];
                        }
                        else
                        {
                            path.StrokeThickness = oldpath.StrokeThickness;
                        }
                        if (hasStrokeCol)
                        {

                            path.Stroke = new SolidColorBrush(HUI_Util.ToMediaColor(strokeCol[i % strokeCol.Count]));
                        }
                        else
                        {
                            path.Stroke = oldpath.Stroke;
                        }

                        i++;
                        G.Children.Add(path);
                    }



/*
                    Path newPath = new Path();
                    DA.GetData<double>("Scale", ref scale);
                    newPath.Data = CreateShape_Component.pathGeomFromCrvs(shapeCrvs, scale, true);
                    G.Children.Remove(path);

                    path = newPath;
                    G.Children.Add(path);*/
                }




             


                if (DA.GetData<int>("Width", ref width))
                {

                    G.Width = width;
                }
                else
                {
                    G.Width = oldpath.Width;
                }
                if (DA.GetData<int>("Height", ref height))
                {

                    G.Height = height;
                }
                else
                {
                    G.Height = oldpath.Height;
                }


            }

        }


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
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.SetShapes;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{EDC4A536-7412-46F2-B56F-6D8668D6B983}"); }
        }
    }
}