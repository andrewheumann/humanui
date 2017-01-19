using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using HumanUIBaseApp;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using Grasshopper.Kernel.Types;
using Control = System.Windows.Forms.Control;
using Size = System.Windows.Size;

namespace HumanUI.Components.UI_Main
{
    public class CaptureWindow_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CaptureWindow_Component class.
        /// </summary>
        public CaptureWindow_Component()
          : base("Capture Window or Element to File", "Capture",
              "Capture a HUI Window or individual element to an image",
             "Human UI", "UI Main")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Window or Element", "W", "The Window to Capture", GH_ParamAccess.item);
            pManager.AddTextParameter("File Path", "F", "The file path where the image should be saved", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale Factor", "S", "A scale factor applied to the size of the window or element for hi-resolution capture", GH_ParamAccess.item, 1.0);
            pManager.AddBooleanParameter("Run", "R", "Set to true to activate the capture.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Capture Entire Window", "EW",
                "If a window is supplied, capture the entire window contents inside the scrollable area. Title bar will not show. This parameter is ignored if supplying a specific element.",
                GH_ParamAccess.item, false);
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
            object o = null;
            MainWindow mw = null;
            UIElement_Goo uig = null;
            string filePath = "";
            double scaleFactor = 1.0;
            bool run = false;
            bool insideScrollable = false;

            if (!DA.GetData<object>("Window or Element", ref o)) return;
        

            if (!DA.GetData<string>("File Path", ref filePath)) return;
            if (!DA.GetData<double>("Scale Factor", ref scaleFactor)) return;
            if (!DA.GetData<bool>("Run", ref run)) return;
            if (!DA.GetData<bool>("Capture Entire Window", ref insideScrollable)) return;

            uig = o as UIElement_Goo;
            var wrapper = o as GH_ObjectWrapper;
            if (wrapper != null) mw = wrapper.Value as MainWindow;
            FrameworkElement fe = null;

            var isWindow = mw != null;

            if (mw == null && uig == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There was a problem processing this element/Window");
                return;
            }

            if (!isWindow)
            {
                fe = uig.element as FrameworkElement;
            }

            


            var toCapture = fe;


            if (!run) return;
            if (isWindow)
            {

                var grid = mw.Content as Grid;

                var scrollViewer = grid.Children[0];

                toCapture = insideScrollable ? ((scrollViewer as ScrollViewer).Content as Grid) : mw as FrameworkElement;
            }
          //  toCapture.Measure(Size.Empty);
           // toCapture.

            var size = new Size(toCapture.ActualWidth, toCapture.ActualHeight);
            //while (Math.Abs(size.Width) < 0.001 || Math.Abs(size.Height) < 0.001)
            //{
            //    var parent = toCapture.Parent as FrameworkElement;
            //    if (parent == null) break;
            //    size = new Size(parent.ActualWidth, parent.ActualHeight);
            //}
            var rect = new Rect(new System.Windows.Point(), size);
         //   toCapture.Arrange(rect);

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(toCapture) { Stretch = Stretch.None }, null,
                                     rect);
            }
            visual.Transform = new ScaleTransform(scaleFactor, scaleFactor);

            var rtb = new RenderTargetBitmap((int)(size.Width * scaleFactor), (int)(size.Height * scaleFactor), 96, 96, PixelFormats.Pbgra32);

            toCapture.Measure(toCapture.RenderSize);
         //   toCapture.Arrange(rect);

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.captureWindow;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{900FCBA9-1B83-403E-B909-9293146469D8}");
    }
}