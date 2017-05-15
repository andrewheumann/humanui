using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Grasshopper.Kernel.Parameters;

namespace HumanUI.Components.UI_Containers
{
    /// <summary>
    /// Component to create a "Stack" layout element. This will keep elements from overlapping each other, laying them out either
    /// vertically or horizontally.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateScrollViewer_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateScrollViewer_Component class.
        /// </summary>
        public CreateScrollViewer_Component()
            : base("Create Scroll Viewer", "ScrollViewer",
                "Allows an element to scroll independently of the rest of the window",
                "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to put in the scroll group", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Horizontal Scroll Bar Visibility", "HV", "Whether or not to show the horizontal scroll bar", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Vertical Scroll Bar Visibility", "VV", "Whether or not to show the vertical scroll bar", GH_ParamAccess.item, 0);

            Param_Integer horizviz = pManager[1] as Param_Integer;
            Param_Integer vertviz = pManager[2] as Param_Integer;
            horizviz.AddNamedValue("Auto", 0);
            horizviz.AddNamedValue("Show", 1);
            horizviz.AddNamedValue("Hide", 2);
            vertviz.AddNamedValue("Auto", 0);
            vertviz.AddNamedValue("Show", 1);
            vertviz.AddNamedValue("Hide", 2);

            pManager.AddNumberParameter("Width", "W", "The width of the scrolling revion", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "The height of the scrolling revion", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[4].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ScrollViewer", "S", "The scrolling group of elements", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
            int horizViz = 0;
            int vertViz = 0;
            double height = -1;
            double width = -1;
            if (!DA.GetDataList<UIElement_Goo>("UI Elements", elementsToAdd)) return;
            DA.GetData<int>("Horizontal Scroll Bar Visibility", ref horizViz);
            DA.GetData<int>("Vertical Scroll Bar Visibility", ref vertViz);
            bool hasHeight = DA.GetData<double>("Height", ref height);
            bool hasWidth = DA.GetData<double>("Width", ref width);




            //initialize the stack panel container and the scroll viewer to contain it
            ScrollViewer sv = new ScrollViewer();
            StackPanel sp = new StackPanel();
            sp.Name = "GH_Stack";

            switch (vertViz)
            {
                case 0:
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    break;
                case 1:
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    break;
                case 2:
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    break;
            }

            switch (horizViz)
            {
                case 0:
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    break;
                case 1:
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    break;
                case 2:
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    break;
            }


            if (hasHeight)
            {
                sv.Height = height;
            }

            if (hasWidth)
            {
                sv.Width = width;
            }

            foreach (UIElement_Goo u in elementsToAdd)
            {
                //Make sure it's not in some other container or window
                HUI_Util.removeParent(u.element);
                sp.Children.Add(u.element);
            }
            sv.Content = sp;
            //pass out the stack panel
            DA.SetData("ScrollViewer", new UIElement_Goo(sv, "ScrollViewer", InstanceGuid, DA.Iteration));
        }



        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ScrollViewer;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{038A7B79-5443-4CA1-BF88-C0CD3894C357}");
    }
}