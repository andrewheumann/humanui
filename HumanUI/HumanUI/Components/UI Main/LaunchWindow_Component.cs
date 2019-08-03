using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Types;
using HumanUIBaseApp;
using GH_IO.Serialization;

namespace HumanUI.Components.UI_Main 
{
    /// <summary>
    /// Represents the ownership status of a window - whether it is a child of Rhino, Grasshopper, or set to be always on top. 
    /// </summary>
     enum childStatus { ChildOfGH, ChildOfRhino, AlwaysOnTop};


     /// <summary>
     /// This component launches an empty HumanUIBaseApp.MainWindow.
     /// </summary>
     /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class LaunchWindow_Component : GH_Component
    {

        private childStatus winChildStatus = childStatus.ChildOfGH;

        protected MainWindow mw;
        bool shouldBeVisible = true;
        bool enableHorizScroll = false;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public LaunchWindow_Component()
            : base("Launch Window", "LaunchWin", "This component launches a new blank control window.", "Human UI", "UI Main")
        {
            UpdateMenu();
        }

        //Alternate Constructor to be overridden by Transparent Window component
        public LaunchWindow_Component(string name, string nickname, string description, string category, string subcategory)
            : base(name,nickname,description,category,subcategory)
        {
            UpdateMenu();   
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Show", "S", "Set this boolean to true to display the control window.", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "The name of the window to display.", GH_ParamAccess.item, "Control Window");
            pManager.AddIntegerParameter("Width", "W", "Starting Width of the window.", GH_ParamAccess.item, 370);
            pManager.AddIntegerParameter("Height", "H", "Starting Height of the window.", GH_ParamAccess.item, 400);
            pManager.AddTextParameter("Font Family", "F", "Optional Font family for UI elements in this window.", GH_ParamAccess.item);
            pManager[4].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Window Object", "W", "The window object. Other components can access this to add controls or gather data from the window.", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            bool show = false;
            int width = 370;
            int height = 400;
            string font = "Segoe UI";
            string windowName = "Control Window";
            if (!DA.GetData<bool>("Show", ref show)) return;
            DA.GetData<string>("Name", ref windowName);
            DA.GetData<int>("Width", ref width);
            DA.GetData<int>("Height", ref height);
            mw.Title = windowName;
            mw.Height = height;
            mw.Width = width;
            mw.HorizontalScrollingEnabled = enableHorizScroll;
            


            //this nebulous "ShouldBeVisible" helps account for the fact that there are other conditions controling window visibility (see SetupWin and HideWindow methods in this class) - lets us separate what the user wants from what should actually happen at any moment.
            if (show)
            {
                shouldBeVisible = true;
                mw.Show();
            }
            else
            {
                shouldBeVisible = false;
                mw.Hide();
            }


            if (DA.GetData<string>("Font Family", ref font))
            {
                try
                {
                    mw.setFont(font);
                }
                catch
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Font not recognized");
                }
            }

            DA.SetData("Window Object", mw);


        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LaunchWindow;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{0A6B8A40-57A4-4D8D-9F09-F34869655D1E}");


        /// <summary>
        /// This method is overridden to make sure that the window is set up and not null before Solve Instance is called.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            if (mw == null || !mw.IsLoaded)
            {
                SetupWin();
            }
            base.BeforeSolveInstance();
        }


        /// <summary>
        /// Sets up the window.
        /// </summary>
        private void SetupWin()
        {
            //try closing a window if it's already up
            try
            {
                mw.Close();
            }
            catch { }

            mw = new MainWindow();
            //Add a listener for window close 
            mw.Closed += mw_Closed;

            //set ownership based on child status 
            SetChildStatus(mw,winChildStatus);

            ElementHost.EnableModelessKeyboardInterop(mw);

            //make sure to hide the window when the user switches active GH document. 
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= HideWindow;
            Grasshopper.Instances.ActiveCanvas.DocumentChanged += HideWindow;

        }

        internal static void SetChildStatus(MainWindow mw, childStatus winChildStatus)
        {
            switch (winChildStatus)
            {
                case childStatus.ChildOfGH:
                    setOwner(Grasshopper.Instances.DocumentEditor, mw);
                    break;
                case childStatus.AlwaysOnTop:
                    mw.Topmost = true;
                    break;
                case childStatus.ChildOfRhino:
                    setOwner(Rhino.RhinoApp.MainWindowHandle(), mw);
                    break;
                default:
                    break;
            }
        }

        //Utility functions to set the ownership of a window object
        static void setOwner(System.Windows.Forms.Form ownerForm, System.Windows.Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = ownerForm.Handle;
        }

        static void setOwner(IntPtr ownerPtr, System.Windows.Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = ownerPtr;
        }



        void mw_Closed(object sender, EventArgs e)
        {
            //remove the listener
            mw.Closed -= mw_Closed;
            //initialize a brand new window. Once it's closed, you can't get it back. 
            SetupWin();
        }

        /// <summary>
        /// Event handler to hide the HUI window. "ShouldBeVisible" tracks the user's choice about visibility - so if 
        /// you switch from one doc to another and the window is supposed to be hidden, it stays hidden. 
        /// </summary>
        private void HideWindow(object sender, Grasshopper.GUI.Canvas.GH_CanvasDocumentChangedEventArgs e)
        {
            if (mw != null)
            {
                if (e.NewDocument == this.OnPingDocument() && e.OldDocument != null) // switching from other document
                {
                    try
                    {
                       
                     if(shouldBeVisible && e.NewDocument != null)   mw.Show();
                    }
                    catch
                    {

                    }
                }
                else if (e.NewDocument == this.OnPingDocument() && e.OldDocument == null) // fresh window
                {
                    try
                    {
                       
                        if (shouldBeVisible) mw.Show();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        mw.Hide();
                    }
                    catch { }
                }

            }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Overriding this function to add component menu for Child status. 
        /// </summary>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Child of Grasshopper", new System.EventHandler(this.menu_makeChildofGH), true,winChildStatus==childStatus.ChildOfGH);
            toolStripMenuItem.ToolTipText = "When selected, the window is made a child of the Grasshopper window - when the Grasshopper window is hidden or minimized, it will disappear.";
            ToolStripMenuItem toolStripMenuItem1 = GH_DocumentObject.Menu_AppendItem(menu, "Child of Rhino", new System.EventHandler(this.menu_makeChildofRhino), true, winChildStatus == childStatus.ChildOfRhino);
            toolStripMenuItem1.ToolTipText = "When selected, the window is made a child of the Rhino window - when the Rhino window is hidden or minimized, it will disappear.";
            ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Always On Top", new System.EventHandler(this.menu_makeAlwaysOnTop), true, winChildStatus == childStatus.AlwaysOnTop);
            toolStripMenuItem2.ToolTipText = "When selected, the window is always on top, floating above other apps.";
            GH_DocumentObject.Menu_AppendSeparator(menu);
            ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem(menu, "Enable Horizontal Scrolling", new System.EventHandler(this.menu_toggleHorizScroll), true, enableHorizScroll);
            toolStripMenuItem.ToolTipText = "When enabled, the window will show a scroll bar when content exceeds the window width";

        }

        private void menu_toggleHorizScroll(object sender, EventArgs e)
        {
            RecordUndoEvent("Horizontal Scrolling Toggle");
            enableHorizScroll = !enableHorizScroll;
            SetupWin();
            ExpireSolution(true);
        }

        // Event handlers for each menu selection
        private void menu_makeChildofGH(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Child Window Status Change");
            this.winChildStatus = childStatus.ChildOfGH;
            this.UpdateMenu();
            this.SetupWin();
            this.ExpireSolution(true);
        }

        private void menu_makeChildofRhino(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Child Window Status Change");
            this.winChildStatus = childStatus.ChildOfRhino;
            this.UpdateMenu();
            this.SetupWin();
            this.ExpireSolution(true);
        }

        private void menu_makeAlwaysOnTop(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Child Window Status Change");
            this.winChildStatus = childStatus.AlwaysOnTop;
            this.UpdateMenu();
            this.SetupWin();
            this.ExpireSolution(true);
        }


     
        /// <summary>
        /// Updates the black menu on the bottom of the component
        /// </summary>
        private void UpdateMenu()
        {
            switch (winChildStatus)
            {
                case childStatus.ChildOfGH:
                    Message = "Child of GH";
                    break;
                case childStatus.AlwaysOnTop:
                    Message = "Always On Top";
                    break;
                case childStatus.ChildOfRhino:
                    Message = "Child of Rhino";
                    break;
                default:
                    break;
            }
           
        }


        /// <summary>
        /// Overrides the RemovedFromDocument method in the hopes of closing/disposing the window to prevent crashes. Doesn't seem to work yet...
        /// </summary>
        public override void RemovedFromDocument(GH_Document document)
        {
            try
            {
                mw.Close();
            }
            catch { }
            base.RemovedFromDocument(document);
        }



        /// <summary>
        /// Adds to the default serialization method to save the current child status so it persists on copy/paste and save/reopen.
        /// </summary>

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("ChildStatus", (int)winChildStatus);
            writer.SetBoolean("EnableHorizScroll", enableHorizScroll);
            return base.Write(writer);
          
        }

        /// <summary>
        /// Adds to the default deserialization method to retrieve the saved child status so it persists on copy/paste and save/reopen.
        /// </summary>
        public override bool Read(GH_IReader reader)
        {
            int readVal = -1;
            reader.TryGetInt32("ChildStatus", ref readVal);

            bool enableHorizScroll = false;
            reader.TryGetBoolean("EnableHorizScroll", ref enableHorizScroll);

            winChildStatus = (childStatus)readVal;
            this.enableHorizScroll = enableHorizScroll;
            this.UpdateMenu();
            return base.Read(reader);
        }



    }
}
