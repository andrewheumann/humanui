using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System.Linq;
using Grasshopper.Kernel.Graphs;
using System.Windows;

namespace HumanUI
{
    public class CreateGraphMapper_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateGraphMapper_Component class.
        /// </summary>
        public CreateGraphMapper_Component()
          : base("Create Graph Mapper", "GraphMapper",
              "Creates a Bezier Graph Mapper",
              "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph Mapper", "G", "An optional Graph Mapper to represent the starting configuration. Connect directly or use Metahopper to obtain a reference.", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph Mapper", "G", "The Graph Mapper UI element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_GraphMapper gm = null;

            try
            {
                gm = Params.Input[0].Sources.OfType<GH_GraphMapper>().First();
            } catch
            {
                try
                {
                    DA.GetData<GH_GraphMapper>("Graph Mapper", ref gm);
                } catch
                {

                }
            }

            if(gm == null)
            {
                DA.SetData("Graph Mapper", new UIElement_Goo(new GraphMapperElement(), "Graph Mapper", InstanceGuid, DA.Iteration));
                return;
            } else
            {
               if(gm.Graph.GraphTypeID != new Guid("{7026A6D2-9B94-4314-B6D3-6850EFF942FE}"))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Currently, only Bezier type graphs are supported.");
                    DA.SetData("Graph Mapper", new UIElement_Goo(new GraphMapperElement(), "Graph Mapper", InstanceGuid, DA.Iteration));
                    return;
                }
                GH_BezierGraph graph = gm.Graph as GH_BezierGraph;
                Point c0 = new Point(graph.Grips[0].X, graph.Grips[0].Y);
                Point c1 = new Point(graph.Grips[1].X, graph.Grips[1].Y);
                Point c2 = new Point(graph.Grips[2].X, graph.Grips[2].Y);
                Point c3 = new Point(graph.Grips[3].X, graph.Grips[3].Y);

                DA.SetData("Graph Mapper", new UIElement_Goo(new GraphMapperElement(c0,c1,c2,c3), "Graph Mapper", InstanceGuid, DA.Iteration));
                return;



            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>N
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.GraphMapper;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bc47947b-f83f-4cdd-b7d8-abc546a26c5e}"); }
        }
    }
}