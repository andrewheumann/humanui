using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// This file is designed as a template for components that handle creation of HUI Elements.
    /// </summary>
    public class CreateToggle_Component : GH_Component
    {
        /// <summary>
        /// This file is designed as a template for components that handle creation of HUI Elements.
        /// </summary>
        public CreateToggle_Component()
            : base("Create Toggle", "Toggle",
                "Creates an on-off toggle.",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Default", "D", "Whether toggle is on by default", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Label", "L", "The optional label for the toggle", GH_ParamAccess.item);
            pManager.AddTextParameter("On Label", "On", "The text to display when on", GH_ParamAccess.item);
            pManager.AddTextParameter("Off Label", "Off", "The text to display when off", GH_ParamAccess.item);
            new List<int> { 1, 2, 3 }.ForEach(i => pManager[i].Optional = true);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Toggle", "T", "The Toggle Element.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //  variables for all input parameters
            bool defaultVal = false;
            string label, onVal, offVal;
            label = onVal = offVal = "";

            bool hasLabel = DA.GetData("Label", ref label);
            bool hasOnVal = DA.GetData("On Label", ref onVal);
            bool hasOffVal = DA.GetData("Off Label", ref offVal);
            DA.GetData("Default", ref defaultVal);

            ToggleSwitch ts = new ToggleSwitch();
            ts.HorizontalAlignment = HorizontalAlignment.Left;

            ts.IsChecked = defaultVal;

            if (hasLabel) ts.Header = label;
            if (hasOnVal) ts.OnLabel = onVal;
            if (hasOffVal) ts.OffLabel = offVal;


            DA.SetData("Toggle", new UIElement_Goo(ts, "Toggle Switch", InstanceGuid, DA.Iteration));

       


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createToggle;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{baf92e4a-c2ca-47d1-99c1-5e78631994d5}");
    }
}