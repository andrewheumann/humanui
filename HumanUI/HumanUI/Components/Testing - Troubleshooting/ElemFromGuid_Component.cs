using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.Testing
{
    
    /// <summary>
    /// Utility component for testing purposes, hidden from user. Gets GUID and Index from a UI Element
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class ElemFromGuid_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ElemFromGuid class.
        /// </summary>
        public ElemFromGuid_Component()
            : base("ElemFromGuid", "Nickname",
                "Description",
                "Human", "UI Testing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "E", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("GUID", "G", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "i", "", GH_ParamAccess.item);
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            UIElement_Goo gooTest = null;
            DA.GetData<UIElement_Goo>(0, ref gooTest);
            DA.SetData(0, gooTest.instanceGuid);
            DA.SetData(1, gooTest.index);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d6b1d3c9-f9a9-4523-8ecd-1def6fdc753f}"); }
        }
    }
}