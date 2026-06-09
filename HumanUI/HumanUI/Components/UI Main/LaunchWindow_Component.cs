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
        // True when *we* are closing the window (RemovedFromDocument / SetupWin tear-down).
        // The Closing handler uses this to distinguish a user X-click from a programmatic
        // close and only the latter is allowed to actually destroy mw.
        bool _allowProgrammaticClose = false;

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

            // If SetupWin failed in BeforeSolveInstance (e.g. an upstream XAML/resource
            // problem), mw is null and any property access below NREs. Surface a clear
            // runtime message instead of a silent "nothing happens".
            if (mw == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Window failed to initialize. See Rhino command line for details.");
                return;
            }

            mw.Title = windowName;
            mw.Height = height;
            mw.Width = width;
            mw.HorizontalScrollingEnabled = enableHorizScroll;



            //this nebulous "ShouldBeVisible" helps account for the fact that there are other conditions controling window visibility (see SetupWin and HideWindow methods in this class) - lets us separate what the user wants from what should actually happen at any moment.
            if (show)
            {
                shouldBeVisible = true;
                mw.Show();
                // WPF's Show() does not bring the window to front when another HWND
                // already has focus (the canvas the user just clicked on). Without
                // Activate(), the window opens behind Rhino on first toggle -- a
                // long-standing forum complaint.
                mw.Activate();
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
            // Capture the position/size of any existing window before we replace it, so
            // a SetupWin triggered by something other than first-launch (e.g. menu change,
            // a doc-changed reset) doesn't yank the window back to the default location.
            // The user's hand-placed position via SetWindowProperties would otherwise be
            // lost on every internal recreate.
            double? carryLeft = null, carryTop = null, carryWidth = null, carryHeight = null;
            if (mw != null && mw.IsLoaded && !double.IsNaN(mw.Left) && !double.IsNaN(mw.Top))
            {
                carryLeft = mw.Left;
                carryTop = mw.Top;
                carryWidth = mw.Width;
                carryHeight = mw.Height;
            }

            //try closing a window if it's already up. SetupWin is a programmatic teardown
            //(menu change, doc reset) so we allow the Closing handler to let the Close go
            //through rather than intercepting it as a user X-click.
            try
            {
                _allowProgrammaticClose = true;
                mw?.Close();
            }
            catch { }
            finally
            {
                _allowProgrammaticClose = false;
            }

            mw = new MainWindow();
            if (carryLeft.HasValue)
            {
                mw.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                mw.Left = carryLeft.Value;
                mw.Top = carryTop.Value;
                mw.Width = carryWidth.Value;
                mw.Height = carryHeight.Value;
            }
            //Closing intercepts a user X-click so we hide instead of destroying the window.
            //Destroying loses all wired-up element state and forces a full rebuild downstream.
            mw.Closing += mw_Closing;
            //Closed is still wired as a safety net for genuinely closed windows.
            mw.Closed += mw_Closed;

            //set ownership based on child status 
            SetChildStatus(mw,winChildStatus);

            ElementHost.EnableModelessKeyboardInterop(mw);

            // 6 April 2021 - S. Baer
            // Check for the possibility of a null canvas. If the window is only
            // shown using the Grasshopper Player and Grasshopper itself has never
            // been run, then there is no canvas.
            var canvas = Grasshopper.Instances.ActiveCanvas;
            if (canvas!=null)
            {
                //make sure to hide the window when the user switches active GH document.
                canvas.DocumentChanged -= HideWindow;
                canvas.DocumentChanged += HideWindow;
            }
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



        // Intercept a user X-click on the window. Without this the Closed event fires,
        // mw_Closed rebuilds the window in the background, and the user's "close" gets
        // resurrected on the next solve -- the long-running "I closed it but it came
        // back" / "duplicate windows after toggle spam" forum complaint. Programmatic
        // closes (SetupWin tear-down, RemovedFromDocument) set _allowProgrammaticClose
        // first so the actual close goes through.
        void mw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allowProgrammaticClose) return;
            e.Cancel = true;
            shouldBeVisible = false;
            mw.Hide();
        }

        void mw_Closed(object sender, EventArgs e)
        {
            //remove the listeners
            mw.Closing -= mw_Closing;
            mw.Closed -= mw_Closed;
            // Programmatic teardown (SetupWin / RemovedFromDocument) already
            // constructs the replacement window on its own path — if we re-enter
            // SetupWin here we'd build two MainWindow instances (the second one
            // immediately tearing down the first via _allowProgrammaticClose
            // again, and so on). Only resurrect when the window died from a
            // non-programmatic close that bypassed mw_Closing (e.g. an Alt+F4
            // that fired straight to Closed in some host versions).
            if (_allowProgrammaticClose) return;
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
                _allowProgrammaticClose = true;
                mw?.Close();
            }
            catch { }
            finally
            {
                _allowProgrammaticClose = false;
            }
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
