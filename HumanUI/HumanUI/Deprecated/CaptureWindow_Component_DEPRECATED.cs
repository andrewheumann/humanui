using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using HumanUIBaseApp;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace HumanUI.Components.UI_Main
{
    public class CaptureWindow_Component_DEPRECATED : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CaptureWindow_Component_DEPRECATED class.
        /// </summary>
        public CaptureWindow_Component_DEPRECATED()
          : base("Capture Window to File", "Capture",
              "Capture a HUI Window to an image",
             "Human UI", "UI Main")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
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
            pManager.AddGenericParameter("Window", "W", "The Window to Capture", GH_ParamAccess.item);
            pManager.AddTextParameter("File Path", "F", "The file path where the image should be saved", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale Factor", "S", "A scale factor applied to the size of the window for hi-resolution capture", GH_ParamAccess.item, 1.0);
            pManager.AddBooleanParameter("Run", "R", "Set to true to activate the capture.", GH_ParamAccess.item, false);
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
            MainWindow mw = null;
            string filePath = "";
            double scaleFactor = 1.0;
            bool run = false;

            if (!DA.GetData<MainWindow>("Window", ref mw)) return;
            if (!DA.GetData<string>("File Path", ref filePath)) return;
            if (!DA.GetData<double>("Scale Factor", ref scaleFactor)) return;
            if (!DA.GetData<bool>("Run", ref run)) return;

            if (mw == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Window is null");
                return;
            }

            if (!run) return;

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(mw), null,
                                      new Rect(new System.Windows.Point(),
                                               new Size(mw.Width, mw.Height)));
            }
            visual.Transform = new ScaleTransform(scaleFactor,scaleFactor);
            var rtb = new RenderTargetBitmap((int)(mw.Width*scaleFactor), (int)(mw.Height*scaleFactor), 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);

            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(rtb));

            using (var stm = File.Create(filePath))
            {
                enc.Save(stm);
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
                return Properties.Resources.captureWindow;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{cbf6b72e-bfda-4d36-b114-80a1e8d942c6}"); }
        }
    }
}