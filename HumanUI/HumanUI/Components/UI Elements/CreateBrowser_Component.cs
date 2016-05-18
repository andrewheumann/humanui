using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// A component to create an ActiveX web control
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateBrowser_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateBrowser_Component class.
        /// </summary>
        public CreateBrowser_Component()
            : base("Create Browser", "Browser",
                "Creates a web browser window.",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("URL", "U", "The URI/URL of the page to display", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Width", "W", "Width of the window", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Height", "H", "Height of the window", GH_ParamAccess.item);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Browser", "WB", "The created Web Browser", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "";
            int width = -1;
            int height = -1;
            if (!DA.GetData<string>("URL", ref url)) return;

            //initiate the WebBrowser object
            WebBrowser wb = new WebBrowser();
            //point it to a URL
            wb.Source = new Uri(url);

            //optionally set its dimensions
            if(DA.GetData<int>("Width",ref width)){
                wb.Width = width;   
            }
            if (DA.GetData<int>("Height", ref height))
            {
                wb.Height = height;
            }

            //pass out the browser object
            DA.SetData("Browser", new UIElement_Goo(wb, String.Format("Browser: {0}", url), InstanceGuid, DA.Iteration));
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
                return Properties.Resources.createBrowser;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{cf49dbbd-93b5-4f9f-81ea-f585f20a5843}"); }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }





    }
}