using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create an updatable image object
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateImage_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateImage_Component()
            : base("Create Image", "Image",
                "Creates an image object to be added to the window",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Image Source", "src", "The file location of the image to add.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Image Width", "W", "The width of the image", GH_ParamAccess.item, 300);
            pManager.AddNumberParameter("Image Height", "H", "The height of the image. Set to 0 or leave blank to scale image proportionally to its width.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Image","I",@"The image object. Use in conjunction with an ""Add Elements"" component.",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double width = 0;
            double height = 0;
            string imgSrc = "";
            string name = "Image";
            if(!DA.GetData<string>("Image Source", ref imgSrc)) return;
            DA.GetData<double>("Image Width", ref width);
            DA.GetData<double>("Image Height", ref height);
            try
            {
                //initialize bitmap object from source
                Bitmap b = new Bitmap(imgSrc);
                // if the user hasn't specified a height 
                // but has spec'd width, calc the height as a function of width and the bitmap dimensions.
                if (height == 0) 
                {
                    height = (b.Height / (double)b.Width) * width;
                }
            }
            catch (Exception e) //pass any errors (like invalid url, etc) back to the user.
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,e.ToString());
            }
            //set up the image object
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            try
            {
                Uri filePath = new Uri(imgSrc);
                BitmapImage bi = new BitmapImage(filePath);
                name = System.IO.Path.GetFileNameWithoutExtension(imgSrc);
                img.Source = bi;
                img.Width = width;
                img.Height = height;
                img.Margin = new Thickness(4);
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            }
            catch (Exception e) { //pass out any errors
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.ToString());
            }
            //output the image object
            DA.SetData("Image", new UIElement_Goo(img, name, InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreateImage;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3c76e033-c9a8-4b6d-94c4-8ccdfac09834}"); }
        }
    }
}