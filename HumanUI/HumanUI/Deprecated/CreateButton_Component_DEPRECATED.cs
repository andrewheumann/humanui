using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

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

namespace HumanUI
{
    public class CreateButton_Component_DEPRECATED : GH_Component
    {

       
        /// <summary>
        /// Initializes a new instance of the CreateButton_Component class.
        /// </summary>
        public CreateButton_Component_DEPRECATED()
            : base("Create Button", "Button",
                "Create a Button object.",
                "Human", "UI Elements")
        {
        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden ;
            }
        }

        public override bool Obsolete
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Button Name", "N", "The text to display on the button", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Button", "B", "The created Button", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Button";

            if (!DA.GetData<string>("Button Name", ref name)) return;
            Button btn = new Button();
            btn.Content = name;
  
            btn.MaxWidth = 150;
            btn.Margin = new Thickness(4);

            DA.SetData("Button", new UIElement_Goo(btn,name, InstanceGuid, DA.Iteration));

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
                return Properties.Resources.CreateButton;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b29e7c03-27ee-446c-894c-476261a807d1}"); }
        }
    }
}