using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a 3D Viewport object and populate it with a mesh.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class Create3DView_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Create3DView_Component class.
        /// </summary>
        public Create3DView_Component()
            : base("Create 3D View", "3DView",
                "Creates an orbitable 3d viewport with a custom-defined mesh",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh to display", "M", "The mesh(es) to display in the viewport", GH_ParamAccess.list);
            pManager.AddColourParameter("Mesh Colors", "C", "The color with which to display the mesh.", GH_ParamAccess.list,System.Drawing.Color.Red);
            pManager.AddNumberParameter("View Width", "W", "The width of the 3d viewport", GH_ParamAccess.item,300);
            pManager.AddNumberParameter("View Height", "H", "The height of the 3d viewport", GH_ParamAccess.item,300);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("3DView", "V", "The 3D view containing your mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> m = new List<Mesh>();
            List<System.Drawing.Color> cols = new List<System.Drawing.Color>();
            double width = 300;
            double height = 300;

            if (!DA.GetDataList<Mesh>("Mesh to display", m)) return;
            DA.GetDataList<System.Drawing.Color>("Mesh Colors", cols);
            DA.GetData<double>("View Width", ref width);
            DA.GetData<double>("View Height", ref height);

            //set up a 3d viewport
            HelixViewport3D vp3 = new HelixViewport3D();
            //set its 3d properties
            vp3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vp3.Width = width;
            vp3.Height = height;
            //these override the default mouse click behavior so that one can orbit/pan with left click instead of right. 
            vp3.RotateGesture.MouseAction = System.Windows.Input.MouseAction.LeftClick;
            vp3.PanGesture.MouseAction = System.Windows.Input.MouseAction.LeftClick;
            vp3.ZoomExtentsWhenLoaded = true;
           
            //set up default lighting
            vp3.Children.Add(new SunLight());
            //create a 3d model
            ModelVisual3D mv3 = new ModelVisual3D();
            // populate the content of the model with a new instance of our custom class
            mv3.Content = new _3DViewModel(m,cols).Model;
            //set up a grid (but this is currently not added to the model)
            GridLinesVisual3D grid = new GridLinesVisual3D();
            grid.Width = 8;
            grid.Length = 8;
            grid.MinorDistance = 1;
            grid.MajorDistance = 1;
            grid.Thickness = 0.01;
            vp3.Children.Add(mv3);
        //    vp3.Children.Add(grid);

            //pass out the 3d viewport object
            DA.SetData("3DView", new UIElement_Goo(vp3,"3D View", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.Create3dView;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5d84c99e-fe9c-4546-8cb0-9f6fe58e011d}"); }
        }
    }
}