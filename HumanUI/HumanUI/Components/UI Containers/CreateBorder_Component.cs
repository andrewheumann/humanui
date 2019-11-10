using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace HumanUI.Components.UI_Containers
{
    /// <summary>
    /// Create a "Border" container - this essentially allows the arbitrary scaling up or down of contained elements. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateBorder_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateBorder_Component class.
        /// </summary>
        public CreateBorder_Component()
            : base("Create Border", "Border",
                "Wrap Elements with a border.",
                "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Element", "E", "The UI element to include", GH_ParamAccess.item);
            pManager.AddNumberParameter("Border Thickness", "T", "Border thickness", GH_ParamAccess.item, 5.0);
            pManager.AddColourParameter("Border Color", "C", "Border color", GH_ParamAccess.item, Color.Black);
            pManager.AddNumberParameter("Corner Radius", "R", "Border corner radius", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Border", "B", "The Border", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            UIElement_Goo elementToAdd = null;
            double thickness = 5;
            Color color = Color.Black;
            double radius = 0;

            if (!DA.GetData<UIElement_Goo>("UI Element", ref elementToAdd)) return;
            if (!DA.GetData("Border Thickness", ref thickness)) return;
            if (!DA.GetData("Border Color", ref color)) return;
            if (!DA.GetData("Corner Radius", ref radius)) return;
            //intitalize the Border
            Border border = new Border
            {
                BorderThickness = new Thickness(thickness),
                BorderBrush = new SolidColorBrush(HUI_Util.ToMediaColor(color)),
                CornerRadius = new CornerRadius(radius)
            };

            HUI_Util.removeParent(elementToAdd.element);
            border.Child = elementToAdd.element;


            //pass out the Border object
            DA.SetData("Border", new UIElement_Goo(border, "Border", InstanceGuid, DA.Iteration));
        }



        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateBorder;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("DFB1703A-45FD-44E6-BE50-E2A4A3C415B4");
    }
}