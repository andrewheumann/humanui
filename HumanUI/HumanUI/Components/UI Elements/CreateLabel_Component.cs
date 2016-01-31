using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Elements
{
    public class CreateLabel_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateLabel_Component class.
        /// </summary>
        public CreateLabel_Component()
            : base("Create Label", "Label",
                "Creates a label in the window.",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Label Text", "T", "The text to display in the label", GH_ParamAccess.item);
          
            pManager.AddIntegerParameter("Label Size", "S", "The size of the label to display", GH_ParamAccess.item,12);
            Param_Integer labelSize = (Param_Integer)pManager[1];
            labelSize.AddNamedValue("Micro", 8);
            labelSize.AddNamedValue("Normal", 12);
            labelSize.AddNamedValue("Heading", 18);
            labelSize.AddNamedValue("Major Heading", 24);
      
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item, 0);
            Param_Integer justification = (Param_Integer)pManager[2];
            justification.AddNamedValue("Left", 0);
            justification.AddNamedValue("Center", 1);
            justification.AddNamedValue("Right", 2);
           
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Label", "L", "The created labels.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string labelContent = "";
            int labelSize = 12;
            int justification = 0;
            if (!DA.GetData<string>("Label Text", ref labelContent)) return;
            DA.GetData<int>("Label Size", ref labelSize);
            DA.GetData<int>("Justification", ref justification);
           //set up the label
            Label l = new Label();
         //populate its properties
            l.Content = labelContent;
            l.FontSize = labelSize;
            l.HorizontalContentAlignment = (HorizontalAlignment)justification;
           
            //pass out the label object
            DA.SetData("Label", new UIElement_Goo(l, String.Format("Label: {0}", labelContent), InstanceGuid, DA.Iteration));
           
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
                return Properties.Resources.CreateLabel;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b844ab20-b7ae-4a21-99d5-83c5666a7432}"); }
        }
    }
}