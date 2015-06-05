using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using System.Windows.Shapes;
using Grasshopper.Kernel.Types;

namespace HumanUI
{
    public class CreateShape_Component : GH_Component
    {
      //  BoundingBox B = BoundingBox.Empty;
       // Grid G = new Grid();
        /// <summary>
        /// Initializes a new instance of the CreateShape_Component class.
        /// </summary>
        public CreateShape_Component()
            : base("Create Shape", "Shape",
                "Creates a simple shape from a polyline",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Shape", "S", "The shape to add as Polyline(s)", GH_ParamAccess.list);
            pManager.AddColourParameter("Fill Color", "FC", "The fill color. Leave empty for no fill", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stroke Weight", "SW", "The stroke weight. Leave empty or set to 0 for no stroke.", GH_ParamAccess.item);
            pManager.AddColourParameter("Stroke Color", "SC", "The stroke color", GH_ParamAccess.item,System.Drawing.Color.Black);
            pManager.AddNumberParameter("Scale","Scl","Use this value to resize the shape.",GH_ParamAccess.item,1.0);
            pManager.AddIntegerParameter("Width", "W", "The width of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Height", "H", "The height of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape", "S", "The created shape.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //TODO - support multiple sets of objects in one shape??
          /*  if (DA.Iteration == 0)
            {
                foreach (IGH_Goo goo in this.Params.Input[0].VolatileData.AllData(true))
                {
                    if (goo is IGH_GeometricGoo)
                    {
                        IGH_GeometricGoo geoGoo = goo as IGH_GeometricGoo;
                        B.Union(geoGoo.GetBoundingBox(Rhino.Geometry.Transform.Identity));
                    }
                }
                G = new Grid();
            }*/
            List<Curve> shapeCrvs = new List<Curve>();
            System.Drawing.Color fillCol = System.Drawing.Color.Transparent;
            double strokeWeight = 0.0;
            System.Drawing.Color strokeCol = System.Drawing.Color.Transparent;
            double scale = 1.0;
            int width = 0;
            int height = 0;

            if (!DA.GetDataList<Curve>("Shape", shapeCrvs)) return;

            Path path = new Path();
            DA.GetData<double>("Scale", ref scale);
            path.Data = pathGeomFromCrvs(shapeCrvs,scale,false);
            if (DA.GetData<System.Drawing.Color>("Fill Color", ref fillCol))
            {

                path.Fill = new SolidColorBrush(HUI_Util.ToMediaColor(fillCol));
            }

            if (DA.GetData<double>("Stroke Weight", ref strokeWeight))
            {

                path.StrokeThickness = strokeWeight;
            }

            if (DA.GetData<System.Drawing.Color>("Stroke Color", ref strokeCol))
            {

                path.Stroke = new SolidColorBrush(HUI_Util.ToMediaColor(strokeCol));
            }

            Grid G = new Grid();
            if (DA.GetData<int>("Width", ref width))
            {

                G.Width = width;
            }
            if (DA.GetData<int>("Height", ref height))
            {

                G.Height = height;
            }
            G.Children.Add(path);


            DA.SetData("Shape", new UIElement_Goo(G, "Shape", InstanceGuid, DA.Iteration));
            
        }


       

        public static Geometry pathGeomFromCrvs(List<Curve> c, double scale,bool rebox)
        {
            string crvString = "";
           rebaseCrvs(c,scale,rebox);
            foreach(Curve crv in c){
                PolylineCurve p = new PolylineCurve();
                Rhino.Geometry.Polyline pl = new Rhino.Geometry.Polyline();
                if (!crv.TryGetPolyline(out pl)) { 
                    p = crv.ToPolyline(0, 0, 0.1, 2.0, 0, 0, crv.GetLength() / 50, 0, true);
                    p.TryGetPolyline(out pl);
                }
                crvString += "M ";
                foreach (Point3d pt in pl)
                {
                    crvString += String.Format("{0:0.000},{1:0.000} ", pt.X, pt.Y);
                }
               

            }
           
            return Geometry.Parse(crvString);
        }

        static void rebaseCrvs(List<Curve> crvs,double scale,bool rebox)
        {
            foreach (Curve c in crvs)
            {
                c.Transform(Rhino.Geometry.Transform.Mirror(Plane.WorldZX));
                c.Transform(Rhino.Geometry.Transform.Scale(Point3d.Origin, scale));
            }
            if (rebox)
            {
                BoundingBox b = BoundingBox.Empty;
                foreach (Curve c in crvs)
                {
                    b.Union(c.GetBoundingBox(true));
                }

                Point3d boxBase = b.PointAt(0, 0, 0);

                foreach (Curve c in crvs)
                {
                    c.Transform(Rhino.Geometry.Transform.Translation(new Vector3d(-boxBase)));
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
                return Properties.Resources.CreateShape;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0ab1c8a7-4182-4a7b-bda3-67c24677182c}"); }
        }
    }
}