using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HumanUI.Components.UI_Containers
{
    /// <summary>
    /// Create a "ViewBox" container - this essentially allows the arbitrary scaling up or down of contained elements. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateViewBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateViewBox_Component class.
        /// </summary>
        public CreateViewBox_Component()
            : base("Create View Box", "ViewBox",
                "Scale a group of UI Elements by placing them in a ViewBox.",
                "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to scale", GH_ParamAccess.list);
            pManager.AddNumberParameter("Width", "W", "The width of the viewbox", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "The height of the viewbox", GH_ParamAccess.item); 
        
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ViewBox", "VB", "The viewbox", GH_ParamAccess.item);
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
            if(!DA.GetData<double>("Width",ref width)) return;
            if(!DA.GetData<double>("Height",ref height)) return;
            //intitalize the viewbox
            Viewbox vb = new Viewbox();
            vb.Width = width;
            vb.Height = height;
            vb.Stretch = Stretch.Uniform;

            //create a stackpanel to contain elements
            StackPanel sp = new StackPanel();

            foreach (UIElement_Goo u in elementsToAdd)
            {
                //make sure elements don't already have a parent and add them to the stackpanel
                HUI_Util.removeParent(u.element);
                sp.Children.Add(u.element);
            }
            //place the stackpanel inside the viewbox
            vb.Child = sp;


            //pass out the viewbox object
            DA.SetData("ViewBox", new UIElement_Goo(vb, "ViewBox", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreateViewBox;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{51123304-F2C4-41EC-B31F-FD8C50E1A113}"); }
        }
    }
}