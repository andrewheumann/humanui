using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using MahApps.Metro;
using HumanUIBaseApp;

namespace HumanUI
{
    /// <exclude />
    public class SetWindowProperties_Component_DEPRECATED : GH_Component
    {
        private string[] ACCENT_COLORS = new string[] { "Amber", "Blue", "Brown", "Cobalt", "Crimson", "Cyan", "Emerald", "Green", "Indigo", "Lime", "Magenta", "Mauve", "Olive", "Orange", "Pink", "Purple", "Red", "Sienna", "Steel", "Taupe", "Teal", "Violet", "Yellow" };


        /// <summary>
        /// Initializes a new instance of the SetWindowProperties_Component class.
        /// </summary>
        public SetWindowProperties_Component_DEPRECATED()
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
            Param_Integer themeParam = (Param_Integer)pManager[2];
            themeParam.AddNamedValue("Light", 0);
            themeParam.AddNamedValue("Dark", 1);
            pManager.AddTextParameter("Accent Color", "A", "The color accent for the window.  Use the component \nmenu item \"Create Accent List\" so you don't have to guess", GH_ParamAccess.item);
            pManager[3].Optional = true;
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

            if (DA.GetData<Point3d>("Starting Location", ref startLoc))
            {
                mw.Left = startLoc.X;
                mw.Top = startLoc.Y;
            }
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

            if (DA.GetData<string>("Accent Color", ref colorName))
            {
                Tuple<AppTheme,Accent> currStyle = ThemeManager.DetectAppStyle(mw);
                ThemeManager.ChangeAppStyle(mw, ThemeManager.GetAccent(colorName), currStyle.Item1);
            }




        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetWindowProperties;

        public override bool Obsolete => true;

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Create Accent List", createAccentList);
            toolStripMenuItem.ToolTipText = "Click this to create a pre-populated list of available accent colors.";
            base.AppendAdditionalMenuItems(menu);
        }

        private void createAccentList(object sender, System.EventArgs e)
        {
            GH_ValueList vl = new GH_ValueList();
            vl.ListItems.Clear();
            foreach (string color in ACCENT_COLORS)
            {
                GH_ValueListItem vi = new GH_ValueListItem(color, String.Format("\"{0}\"", color));
                vl.ListItems.Add(vi);
            }
            vl.NickName = "Accent Colors";
            GH_Document doc = OnPingDocument();
            doc.AddObject(vl, false, doc.ObjectCount + 1);
            PointF currPivot = Params.Input[3].Attributes.Pivot;
            vl.Attributes.Pivot = new PointF(currPivot.X - 120, currPivot.Y - 11);
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        /// 

        public override Guid ComponentGuid => new Guid("{aa3816cc-918e-4383-9125-8f00922f154a}");
    }
}