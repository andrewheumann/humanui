using System;
using System.Collections.Generic;
using System.Windows;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components
{
    public class WindowStatus_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the WindowStatus_Component class.
        /// </summary>
        public WindowStatus_Component()
          : base("Window Status", "WinStat",
              "Gets the current status of the specified Window",
              "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Window", "W", "The window to check", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "The status of the window", GH_ParamAccess.item);
        }


        private Window listenedWindow;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if(listenedWindow != null) RemoveEvents(listenedWindow);
            Window window = null;
            if (!DA.GetData("Window", ref window)) return;
            listenedWindow = window;
            AddEvents(window);
            var status = !window.IsVisible
                ? "Hidden"
                : window.WindowState == WindowState.Minimized ? "Minimized" : "Normal";

            DA.SetData("Status",status);

        }


        private void RemoveEvents(Window window)
        {
            window.Closed -= ExpireThis;
            window.StateChanged -= ExpireThis;
        }

        private void AddEvents(Window window)
        {
            window.Closed += ExpireThis;
            window.StateChanged += ExpireThis;
        }

        private void ExpireThis(object sender, EventArgs e)
        {
            ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{a9ec5ddf-0ed9-4c67-90ac-ae5586390f12}");
    }
}