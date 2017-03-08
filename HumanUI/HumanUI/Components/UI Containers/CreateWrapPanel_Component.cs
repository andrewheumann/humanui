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
    /// Component to create a "WrapPanel" layout element. This will keep elements from overlapping each other, laying them out either
    /// vertically or horizontally.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateWrapPanel_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateWrapPanel_Component class.
        /// </summary>
        public CreateWrapPanel_Component()
            : base("Create WrapPanel", "WrapPanel",
                "Creates a group of UI elements WrapPaneled vertically or horizontally.",
                "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to group together", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Horizontal", "H", "Set to true for horizontal arrangement; false for vertical.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("WrapPanel", "S", "The combined group of elements", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
            bool horiz = true;
            if (!DA.GetDataList<UIElement_Goo>("UI Elements", elementsToAdd)) return;
            DA.GetData<bool>("Horizontal", ref horiz);

            //initialize the WrapPanel panel container
            WrapPanel sp = new WrapPanel();
            sp.Name = "GH_WrapPanel";
            sp.Orientation = horiz ? Orientation.Horizontal : Orientation.Vertical;
            foreach (UIElement_Goo u in elementsToAdd)
            {
                //Make sure it's not in some other container or window
                HUI_Util.removeParent(u.element);
                sp.Children.Add(u.element);
            }
            //pass out the WrapPanel panel
            DA.SetData("WrapPanel", new UIElement_Goo(sp, "WrapPanel", InstanceGuid, DA.Iteration));
        }



        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateWrapPanel;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{00F0E6DF-7227-42A7-B148-A9A4E245C928}");
    }
}