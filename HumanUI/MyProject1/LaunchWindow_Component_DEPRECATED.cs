using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Types;
using HumanUIBaseApp;
using GH_IO.Serialization;

namespace HumanUI
{
    public class LaunchWindow_Component_DEPRECATED : GH_Component
    {

       

        private bool makeChild = true;

        MainWindow mw;


        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public LaunchWindow_Component_DEPRECATED()
            : base("Launch Window", "LaunchWin", "This component launches a new blank control window.", "Human", "UI Main")
     
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
            pManager.AddIntegerParameter("Width", "W", "Starting Width of the window.", GH_ParamAccess.item,370);
            pManager.AddIntegerParameter("Height", "H", "Starting Height of the window.", GH_ParamAccess.item,400);
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
            if (show)
            {
                mw.Show();
            }
            else
            {
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
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.LaunchWindow;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{f9d46462-5227-4c4e-9268-0b678960d7a9}"); }
        }

        protected override void BeforeSolveInstance()
        {
            if (mw == null || !mw.IsLoaded)
            {
                SetupWin();
            }
            base.BeforeSolveInstance();
        }


        private void SetupWin()
        {
          //  WindowReset(EventArgs.Empty);
            try
            {
                mw.Close();
            }
            catch { }
            ExpireInputs(null,EventArgs.Empty);
                mw = new MainWindow();
                mw.InitializeComponent();
                mw.Closed += mw_Closed;
                if (makeChild)
                {
                    setOwner(Grasshopper.Instances.DocumentEditor, mw);
                }
                else
                {
                    mw.Topmost = true;
                 //   WindowInteropHelper helper = new WindowInteropHelper(mw);
                   // helper.Owner = Rhino.RhinoApp.MainWindowHandle();
         
                }
                Grasshopper.Instances.ActiveCanvas.DocumentChanged -= HideWindow;
                Grasshopper.Instances.ActiveCanvas.DocumentChanged += HideWindow;
          
        }

        static void setOwner(System.Windows.Forms.Form ownerForm, System.Windows.Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = ownerForm.Handle;
        }

        

        void mw_Closed(object sender, EventArgs e)
        {
            mw.Closed -= mw_Closed;
            SetupWin();
        }

        private void HideWindow(object sender, Grasshopper.GUI.Canvas.GH_CanvasDocumentChangedEventArgs e)
        {
            if (mw != null)
            {
                if (e.NewDocument == this.OnPingDocument())
                {
                    try
                    {
                        mw.Show();
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

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Make Child Window", new System.EventHandler(this.menu_makeChild), true, this.makeChild);
            toolStripMenuItem.ToolTipText = "When checked, the exploded geometry of the block is calculated. Disable this to speed up component functionality, to retrieve only the block name, attributes, and instance bounding box.";
        }

        private void menu_makeChild(object sender, System.EventArgs e)
        {
            base.RecordUndoEvent("Child Window Toggle");
            this.makeChild = !this.makeChild;
            this.UpdateMenu();
            this.SetupWin();
            this.ExpireSolution(true);
        }


        private void UpdateMenu()
        {
            if (this.makeChild)
            {
                base.Message = "Child Window";
            }
            else
            {
                base.Message = "Independent Window";
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("makeChild", this.makeChild);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            reader.TryGetBoolean("makeChild", ref this.makeChild);
            this.UpdateMenu();
            return base.Read(reader);
        }


        private void ExpireInputs(object sender, EventArgs e)
        {
            GH_Document doc = OnPingDocument();
            foreach (IGH_ActiveObject ao in doc.ActiveObjects())
            {
                if (ao is HUI_Expirable)
                {
                    ao.ExpireSolution(true);
                }

            }
        }


    }
}
