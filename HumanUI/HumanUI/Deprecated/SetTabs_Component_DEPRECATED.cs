using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    public class SetTabs_Component_DEPRECATED : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetTabs_Component class.
        /// </summary>
        public SetTabs_Component_DEPRECATED()
          : base("Set Tabbed View", "SetTab",
              "Sets the properties of a tabbed view",
              "Human UI", "UI Output")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        public override bool Obsolete => true;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Tabbed View", "T", "The tabbed view to modify", GH_ParamAccess.item);
            pManager[pManager.AddTextParameter("Tab Names", "N", "The list of names corresponding to each tab",
                GH_ParamAccess.list)].Optional = true;
            pManager[pManager.AddBooleanParameter("Show Tabs", "S", "Provide a list of boolean values to selectively hide/show tabs.", GH_ParamAccess.list)].Optional = true;
        
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
            object tabContainer = null;
            List<string> tabNames = new List<string>();
            List<bool> hideTabs = new List<bool>();
            if (!DA.GetData("Tabbed View", ref tabContainer)) return;
            bool hasNames = DA.GetDataList("Tab Names", tabNames);
            bool hasHideTabs = DA.GetDataList("Show Tabs", hideTabs);
            TabControl tabControl = HUI_Util.GetUIElement<TabControl>(tabContainer);

            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                var item = tabControl.Items[i];
                var tabItem = item as TabItem;
                if (hasNames && tabNames.Count > i)
                {
                   tabItem.Header = tabNames[i];
                }
                if (hasHideTabs && hideTabs.Count > i)
                {
                    tabItem.Visibility = hideTabs[i]
                        ? System.Windows.Visibility.Visible
                        : System.Windows.Visibility.Collapsed;
                    if (!hideTabs[i] && tabControl.SelectedIndex == i)
                    {
                        tabControl.SelectedIndex = (i + 1)%tabControl.Items.Count;
                    }
                }
               
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetTab;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{0a32b740-1539-4417-ba73-c4885329bd77}");
    }
}