using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System.Windows.Media.Imaging;

namespace HumanUI
{
    public class SetImage_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetImage_Component class.
        /// </summary>
        public SetImage_Component()
            : base("Set Image", "SetImg",
                "Change the content of an existing Image control.",
                "Human", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Image to modify", "I", "The image object to modify", GH_ParamAccess.item);
            pManager.AddTextParameter("New Image Path", "I2", "The path to a new image to replace in the window", GH_ParamAccess.item);
   
        }

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
            object ImageObject = null;
            string newImagePath = "";
            if (!DA.GetData<string>("New Image Path", ref newImagePath)) return;
            if (!DA.GetData<object>("Image to modify", ref ImageObject)) return;
            Image l = null;
            switch (ImageObject.GetType().ToString())
            {
                case "HumanUI.UIElement_Goo":
                    UIElement_Goo goo = ImageObject as UIElement_Goo;
                    l = goo.element as Image;
                    break;
                case "Grasshopper.Kernel.Types.GH_ObjectWrapper":
                    GH_ObjectWrapper wrapper = ImageObject as GH_ObjectWrapper;
                    KeyValuePair<string, UIElement_Goo> kvp = (KeyValuePair<string, UIElement_Goo>)wrapper.Value;
                    l = kvp.Value.element as Image;

                    break;
                default:
                    break;
            }
            if (l != null)
            {
                HUI_Util.SetImageSource(newImagePath, l);
            }
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
                return Properties.Resources.SetImage;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bc15817d-291f-461b-a1a8-f3c66fd053be}"); }
        }
    }
}