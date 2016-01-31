using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using System.Windows;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a multi-line text block
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateTextBlock_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateTextBlock_Component class.
        /// </summary>
        public CreateTextBlock_Component()
            : base("Create Text Block", "TB",
                "Creates a multi-line text block",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "The text to display in the text block", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Text Size", "S", "The size of the label to display", GH_ParamAccess.item, 12);
            Param_Integer labelSize = (Param_Integer)pManager[1];
            labelSize.AddNamedValue("Micro", 8);
            labelSize.AddNamedValue("Normal", 12);
            labelSize.AddNamedValue("Medium", 16);
            labelSize.AddNamedValue("Large", 18);
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item, 0);
            Param_Integer justification = (Param_Integer)pManager[2];
            justification.AddNamedValue("Left", 0);
            justification.AddNamedValue("Center", 1);
            justification.AddNamedValue("Right", 2);
            justification.AddNamedValue("Justify", 3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Text Block", "TB", "The created text block.", GH_ParamAccess.item);
       
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
            if (!DA.GetData<string>("Text", ref labelContent)) return;
            DA.GetData<int>("Text Size", ref labelSize);
            DA.GetData<int>("Justification", ref justification);
            //initialize the textblock
            TextBlock l = new TextBlock();
            l.Text = labelContent;
            l.FontSize = labelSize;
            l.TextWrapping = TextWrapping.Wrap;
            switch (justification)
            {
                case 0:
                    l.TextAlignment = TextAlignment.Left;
                    break;
                case 1:
                    l.TextAlignment = TextAlignment.Center;
                    break;
                case 2:
                    l.TextAlignment = TextAlignment.Right;
                    break;
                case 3:
                    l.TextAlignment = TextAlignment.Justify;
                    break;
                default:
                    l.TextAlignment = TextAlignment.Left;
                    break;

            }
            // pass out the text block
            DA.SetData("Text Block", new UIElement_Goo(l, String.Format("Text Block: {0}", labelContent), InstanceGuid, DA.Iteration));
           
     
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
                return Properties.Resources.CreateTextBlock;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{088f694c-6b70-4baf-afe4-5bfd46526d6f}"); }
        }
    }
}