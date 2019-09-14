using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to set a multidimensional slider object
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetMdSlider_Component : GH_Component
    {

        /// <summary>
        /// Initializes a new instance of the SetMdSlider_Component class.
        /// </summary>
        public SetMdSlider_Component()
            : base("Set Multidimensional Slider", "SetMDSlider",
                "Modify the value of a multidimensional slider.",
                "Human UI", "UI Output")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI MD Slider", "S", "The UI Multidimensional Slider to update", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "The Point to set the multidimensional slider to", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

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
            object mdSliderObject = null;
            var newPoint = new Point3d();

            if (!DA.GetData("UI MD Slider", ref mdSliderObject)) return;
            var hasValue = DA.GetData("Point", ref newPoint);


            if (newPoint.X > 1.0 || newPoint.Y > 1.0 || newPoint.X < 0 || newPoint.Y < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Point is out of bounds");
            }


            var mdSlider = HUI_Util.GetUIElement<MDSliderElement>(mdSliderObject);

            if (mdSlider != null && hasValue)
            {
                mdSlider.SliderPoint = newPoint;
            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetMDSlider;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{04C34E5B-2A9F-4062-8330-EFAB05A86818}");
    }
}
