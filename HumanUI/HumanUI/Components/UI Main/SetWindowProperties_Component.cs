using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using MahApps.Metro;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Main 
{
    /// <summary>
    /// Component to Modify various properties of a HUI Window.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SetWindowProperties_Component : GH_Component
    {
        // These are all the accent colors supported by Mahapps.Metro, found here: 
        // https://github.com/MahApps/MahApps.Metro/tree/master/MahApps.Metro/Styles/Accents
        private string[] ACCENT_COLORS = new string[] { "Amber", "Blue", "Brown", "Cobalt", "Crimson", "Cyan", "Emerald", "Green", "Indigo", "Lime", "Magenta", "Mauve", "Olive", "Orange", "Pink", "Purple", "Red", "Sienna", "Steel", "Taupe", "Teal", "Violet", "Yellow" }; 


        /// <summary>
        /// Initializes a new instance of the SetWindowProperties_Component class.
        /// </summary>
        public SetWindowProperties_Component()
            : base("Set Window Properties", "WinProps",
                "Modify various properties of a Window.",
                "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Window", "W", "The window to modify", GH_ParamAccess.item);
            pManager.AddPointParameter("Starting Location", "L", "The point (screen coordinates, so 0,0 is upper left) \nat which to locate the window's upper left corner.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Theme", "T", "The base theme for the window.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            //custom param for "theme"
            Param_Integer themeParam = (Param_Integer)pManager[2];
            themeParam.AddNamedValue("Light", 0);
            themeParam.AddNamedValue("Dark", 1);
            pManager.AddTextParameter("Accent Color", "A", "The color accent for the window.  Use the component \nmenu item \"Create Accent List\" so you don't have to guess", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Show Title Bar", "TB", "Set to false to hide the window's title bar.", GH_ParamAccess.item, true);
            pManager[4].Optional = true;
            pManager.AddColourParameter("Background Color", "BG", "Set the background color of the window.", GH_ParamAccess.item);
            pManager[5].Optional = true;
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
     
           

            Point3d startLoc = Point3d.Unset;
            int theme = 0;
            string colorName = "";
            if (!DA.GetData<MainWindow>("Window", ref mw)) return;
            //if the user has spec'd a location, move the window
            if (DA.GetData<Point3d>("Starting Location", ref startLoc))
            {
                mw.Left = startLoc.X;
                mw.Top = startLoc.Y;
            }
            //set the theme
            if (DA.GetData<int>("Theme", ref theme))
            {
                switch (theme)
                {
                    case 0:
                        ThemeManager.ChangeAppTheme(mw, "BaseLight");
                        break;
                    case 1:
                        ThemeManager.ChangeAppTheme(mw, "BaseDark");
                        break;
                    default:
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "That's not a valid theme - only 0 or 1 (light or dark) can be used.");
                        break;
                }
            }

            //set title bar visibility
            bool showTitleBar = true;
            DA.GetData<bool>("Show Title Bar", ref showTitleBar);
            SetTitleBarVisibility(mw, showTitleBar);

            //set background color
            System.Drawing.Color backgroundColor = System.Drawing.Color.Transparent;
            if (DA.GetData<System.Drawing.Color>("Background Color", ref backgroundColor))
            {
                mw.Background = new SolidColorBrush(HUI_Util.ToMediaColor(backgroundColor));
            }

            //set accent color
            if (DA.GetData<string>("Accent Color", ref colorName))
            {
                //get the current accent and theme so that theme can be preserved while accent changes
                Tuple<AppTheme,Accent> currStyle = ThemeManager.DetectAppStyle(mw);
               
                ThemeManager.ChangeAppStyle(mw, ThemeManager.GetAccent(colorName), currStyle.Item1);
            }




        }

        /// <summary>
        /// Sets the title bar visibility.
        /// </summary>
        /// <param name="mw">The main window object.</param>
        /// <param name="showTitleBar">if set to <c>true</c> show title bar.</param>
        private static void SetTitleBarVisibility(MainWindow mw, bool showTitleBar)
        {
            mw.ShowMaxRestoreButton = showTitleBar;
            mw.ShowMinButton = showTitleBar;
            mw.ShowTitleBar = showTitleBar;
            mw.ShowCloseButton = showTitleBar;
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
                return Properties.Resources.SetWindowProperties;
            }
        }
        /// <summary>
        /// Adds to the context menu an option to create a pre-populated accent color list object
        /// </summary>
        /// <param name="menu"></param>
        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Create Accent List", createAccentList);
            toolStripMenuItem.ToolTipText = "Click this to create a pre-populated list of available accent colors.";
            base.AppendAdditionalMenuItems(menu);
        }

        /// <summary>
        /// Creates a value list pre-populated with possible accent colors and adds it to the Grasshopper Document, located near the component pivot.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void createAccentList(object sender, System.EventArgs e)
        {
            //initialize object
            GH_ValueList vl = new GH_ValueList();
            //clear default contents
            vl.ListItems.Clear();
            //add all the accent colors as both "Keys" and values
            foreach (string color in ACCENT_COLORS)
            {
                GH_ValueListItem vi = new GH_ValueListItem(color, String.Format("\"{0}\"", color));
                vl.ListItems.Add(vi);
            }
            //set component nickname
            vl.NickName = "Accent Colors";
            //get active GH doc
            GH_Document doc = OnPingDocument();
            // place the object
            doc.AddObject(vl, false, doc.ObjectCount + 1);
            //get the pivot of the "accent" param
            PointF currPivot = Params.Input[3].Attributes.Pivot;
            //set the pivot of the new object
            vl.Attributes.Pivot = new PointF(currPivot.X - 120, currPivot.Y - 11);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{B2CA4D57-1F81-4CE5-AED6-2A39FB285814}"); }
        }
    }
}
