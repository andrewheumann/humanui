using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_IO.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Elements
{



    /// <summary>
    /// Component to create a special version of a button that always returns true, and only updates on press (not release)
    /// - for special UI interaction cases. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateTrueOnlyButton_Component : CreateButton_Component
    {

        /// <summary>
        /// Initializes a new instance of the CreateTrueOnlyButton_Component class.
        /// </summary>
        public CreateTrueOnlyButton_Component()
            : base("Create True-Only Button", "True Button",
                "Create a True only Button object.")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// This is the method that actually does the work. Overriding the default solveInstance to use a TrueOnlyButton instead of a normal Button.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Button";
            string imagePath = "";
            bool hasText = DA.GetData<string>("Button Name", ref name);
            bool hasIcon = DA.GetData<string>("Image Path", ref imagePath);
            if (!hasText && !hasIcon) return;
            // TrueOnlyButton is a dumb class that extends Button with no additional functionality - but this way the 
            // type-based switch statements in value listener are able to pick up the difference and behave accordingly.
            TrueOnlyButton btn = new TrueOnlyButton();
            SetupButton(name, imagePath, hasText, hasIcon, btn, bs);         
            DA.SetData("Button", new UIElement_Goo(btn, name, InstanceGuid, DA.Iteration));

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateTrueButton;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{5AB78609-C132-45C7-BA44-200A4F2E4188}");
    }
}