using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HumanUI
{
    public class CreateGrid_Component : GH_Component, HUI_Expirable
    {
        /// <summary>
        /// Initializes a new instance of the CreateGrid_Component class.
        /// </summary>
        public CreateGrid_Component()
            : base("Create Grid", "Grid",
                "Create a container with absolutely positioned elements. \n Their input order determines their Z order - set the margins \nwith the \"Adjust Element Positioning\" component to locate \nelements inside the grid.",
                "Human", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to place in the grid", GH_ParamAccess.list);
            pManager.AddNumberParameter("Width", "W", "The width of the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "The height of the grid", GH_ParamAccess.item); 
      
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "S", "The combined group of elements", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
            double width = 0;
            double height = 0;
            if (!DA.GetDataList<UIElement_Goo>("UI Elements", elementsToAdd)) return;
            if (!DA.GetData<double>("Width", ref width)) return;
            if (!DA.GetData<double>("Height", ref height)) return;

            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.Name = "GH_Grid";
            grid.Width = width;
            grid.Height = height;
            foreach (UIElement_Goo u in elementsToAdd)
            {
                HUI_Util.removeParent(u.element);
                FrameworkElement fe = u.element as FrameworkElement;
                if(fe != null){
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                }
              
                grid.Children.Add(u.element);
            }

            DA.SetData("Grid", new UIElement_Goo(grid, "Grid", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.createGrid;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1E68A9A8-C28D-4799-854C-337DC4018917}"); }
        }
    }
}