using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;

namespace HumanUI
{
    public class TabContainer_Component_DEPRECATED : GH_Component, IGH_VariableParameterComponent
    {

       
        /// <summary>
        /// Initializes a new instance of the TabContainer_Component class.
        /// </summary>
        public TabContainer_Component_DEPRECATED()
            : base("Tabbed View", "Tabs",
                "Creates a series of tabbed views that can contain UI element layouts",
                "Human", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Tab Names", "N", "The labels for the tabs you're creating.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Tab 0", "T0", "The contents of the first tab", GH_ParamAccess.list);
            VariableParameterMaintenance();
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tabs", "T", "The Tab control", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration > 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "It looks like you're trying to do something with data trees here that doesn't make sense.");
                return;
            }

            List<List<UIElement_Goo>> tabList = new List<List<UIElement_Goo>>();


            List<string> tabNames = new List<string>();
            DA.GetDataList<string>("Tab Names", tabNames);

            //get the data from the variable input params
            for (int i = 1; i < Params.Input.Count; i++)
            {
                List<UIElement_Goo> currentTab = new List<UIElement_Goo>();
                DA.GetDataList<UIElement_Goo>(i, currentTab);
                tabList.Add(currentTab);
            }
                //validate that there's a tab name for every variable input
            if (tabList.Count != tabNames.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You don't have the same number of specified tab names and tab inputs.");
            }

            //create the tab control
            TabControl tabControl = new TabControl();
            //TabControlHelper.SetIsUnderlined(tabControl, true);

                //for each tab,
            int tabIndex = 0;
            foreach (List<UIElement_Goo> oneTab in tabList)
            {
                //create a tabItem
                TabItem tabItem = new TabItem();
                string tabName = "New Tab";
                if (tabIndex < tabNames.Count) tabName = tabNames[tabIndex];
                tabItem.Header = tabName;

                //create a stackpanel
                StackPanel sp = new StackPanel();
                sp.Name = "GH_TabItem";
                sp.Orientation = Orientation.Vertical;
                foreach (UIElement_Goo u in oneTab)
                {
                    HUI_Util.removeParent(u.element);
                    sp.Children.Add(u.element);
                }

                tabItem.Content = sp;

                tabControl.Items.Add(tabItem);

                tabIndex++;
            }



                DA.SetData("Tabs", new UIElement_Goo(tabControl,String.Format("Tab Control with {0} tabs",tabList.Count)));
            
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
                return Properties.Resources.TabControl;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{df93e843-3893-4ffc-b2e3-666190768b8e}"); }
        }

    

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            if (index == 0) return false;
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
           if (side == GH_ParameterSide.Output) return false;
           if (Params.Input.Count <= 2) return false;
            if (index == 0) return false;
            return true;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)  
        {
            Param_GenericObject input = new Param_GenericObject();
            Params.RegisterInputParam(input, index);
            return input;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
          //  Params.UnregisterInputParameter(Params.Input[index]);
            return true;
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 1; i < Params.Input.Count;i++ )
            {
                IGH_Param param = Params.Input[i];

                param.NickName = String.Format("T{0}", i-1);
                param.Name = String.Format("Tab {0}", i-1);
                param.Description = String.Format("The ui elements to include in tab {0}", i - 1);
                param.Access = GH_ParamAccess.list;
                param.Optional = true;
                param.DataMapping = GH_DataMapping.Flatten;

            }
        }




    }
}