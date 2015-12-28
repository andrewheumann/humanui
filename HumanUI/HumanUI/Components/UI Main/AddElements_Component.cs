using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Main 
{
    /// <summary>
    /// Component to add elements to a HUI window
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class AddElements_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddElements_Component class.
        /// </summary>
        public AddElements_Component()
            : base("Add Elements", "AddElems",
                "Add WPF Controls to a window",
                "Human", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Window", "W", "The window to which to add the elements", GH_ParamAccess.item);
            pManager.AddGenericParameter("Elements", "E", "The Controls and other elements you want to add to the window", GH_ParamAccess.list);
            pManager[1].DataMapping = GH_DataMapping.Flatten; // doesn't make sense to pass trees of elements to one window
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Added Elements", "E", "The elements added.", GH_ParamAccess.list);
            pManager.AddTextParameter("Element Names", "N", "The names of the added elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //set up the variables for GetData
            MainWindow mw = null;
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
           
            Dictionary<string, UIElement_Goo> resultDict = new Dictionary<string, UIElement_Goo>();
           
            //Get data from component
            if (!DA.GetData<MainWindow>("Window", ref mw)) return;
            if (!DA.GetDataList<UIElement_Goo>("Elements", elementsToAdd)) return;

            //clear out any old elements
            mw.clearElements();

            //for all the elements, remove its parent, add it to the window, and add it to our tracking dictionary
            foreach (UIElement_Goo u in elementsToAdd)
            {
             
                HUI_Util.removeParent(u.element);
                mw.AddElement(u.element);
                HUI_Util.AddToDict(u, resultDict);
            }

            //Pass out the added elements and their names. This is no longer so necessary now that listeners attach to elements directly. 
            DA.SetDataList("Added Elements", resultDict);
            DA.SetDataList("Element Names", resultDict.Keys);
        }





        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
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
                return Properties.Resources.AddElements;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{73b5e187-b35d-45bd-8495-9e06b429bc07}"); }
        }
    }
}