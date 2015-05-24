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
    public class TabContainer_Component : GH_Component, IGH_VariableParameterComponent
    {

       
        /// <summary>
        /// Initializes a new instance of the TabContainer_Component class.
        /// </summary>
        public TabContainer_Component()
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
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Tab Text Size", "S", "The font size for tab elements", GH_ParamAccess.item);
             pManager[1].Optional = true;
            pManager.AddTextParameter("Tab Icon Source","I","The path to an icon image - one per tab",GH_ParamAccess.list);
           pManager[2].Optional = true;
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

            double fontSize = 26;
            List<string> tabNames = new List<string>();
            List<string> iconPaths = new List<string>();
            int tabCount = 0;
             DA.GetDataList<string>("Tab Icon Source", iconPaths);
            if(DA.GetDataList<string>("Tab Names", tabNames)){
                tabCount = tabNames.Count;
            } else {
                tabCount = iconPaths.Count;
            }
            



            //get the data from the variable input params
            for (int i = 3; i < Params.Input.Count; i++)
            {
                List<UIElement_Goo> currentTab = new List<UIElement_Goo>();
                DA.GetDataList<UIElement_Goo>(i, currentTab);
                tabList.Add(currentTab);
            }
                //validate that there's a tab name for every variable input
            if (tabList.Count != tabCount)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You don't have the same number of specified tab names (or icons) and tab inputs.");
            }

            //create the tab control
            TabControl tabControl = new TabControl();
            //TabControlHelper.SetIsUnderlined(tabControl, true);

            bool setSize = DA.GetData<double>("Tab Text Size", ref fontSize);

        //    MetroWindow imaginaryWindow = new MetroWindow();
            ResourceDictionary ControlsResDict = new ResourceDictionary();
             ControlsResDict.Source = 
            new Uri("/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute);
             ResourceDictionary ColorsResDict = new ResourceDictionary();
             ColorsResDict.Source =
            new Uri("/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute);

         //    imaginaryWindow.Resources.Add("a dict",myresourcedictionary);

          //  Grid grid = new Grid();
          //  imaginaryWindow.Content = grid;
            // MetroTabItem tabItemForStyle = new MetroTabItem();
          //   grid.Children.Add(tabItemForStyle);

         //    imaginaryWindow.Show();

           
         

            Style customTabStyle = new Style(typeof(TabItem), (Style) ControlsResDict["MetroTabItem"]);
            customTabStyle.TargetType = typeof(TabItem);

            //is selected trigger
            Trigger selectionTrigger = new Trigger();
            selectionTrigger.Property = TabItem.IsSelectedProperty;
            selectionTrigger.Value = true;
           
            Setter setter = new Setter();
            setter.Property = TextBlock.ForegroundProperty;
            setter.Value = ColorsResDict["AccentColorBrush"];
       
            selectionTrigger.Setters.Add(setter);

            //hover trigger
            Trigger hoverTrigger = new Trigger();
            hoverTrigger.Property = TabItem.IsMouseOverProperty;
            hoverTrigger.Value = true;

            Setter hoverSetter = new Setter();
            hoverSetter.Property = TabItem.OpacityProperty;
            hoverSetter.Value = 0.65;

            hoverTrigger.Setters.Add(hoverSetter);






            customTabStyle.Triggers.Add(selectionTrigger);
            customTabStyle.Triggers.Add(hoverTrigger);

                //for each tab,
            int tabIndex = 0;
            foreach (List<UIElement_Goo> oneTab in tabList)
            {
                //create a tabItem
                TabItem tabItem = new TabItem();
                tabItem.Style = customTabStyle;
                 StackPanel tabHeader = new StackPanel();
                   if(tabIndex<iconPaths.Count){
                string imagePath = iconPaths[tabIndex];
                    Image img = new Image();
                    Uri filePath = new Uri(imagePath);
                BitmapImage bi = new BitmapImage(filePath);
                    img.Source = bi;
                       tabHeader.Children.Add(img);
                       int pixWidth = bi.PixelWidth;
                       int pixHeight = bi.PixelHeight;
                       img.Height = fontSize;
                       img.Width = fontSize * (pixWidth / pixHeight);
                }

                string tabName = "New Tab";
                if (tabIndex < tabNames.Count) tabName = tabNames[tabIndex];

               
                tabHeader.Orientation = Orientation.Horizontal;
                if(tabName!="New Tab"){
                    TextBlock tb = new TextBlock();
                    tb.Text = tabName;
                    tb.FontSize = fontSize;
                    tabHeader.Children.Add(tb);
                }

                if (iconPaths.Count == 0)
                {
                    if (setSize) ControlsHelper.SetHeaderFontSize(tabItem, fontSize);
                    tabItem.Header = tabName;
                }
                else
                {
                    tabItem.Header = tabHeader;
                }

                

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
            get { return new Guid("{EAF93260-86B3-4AE7-82D2-58E683DEAE7B}"); } 
        }

    

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            if (index == 0) return false;
            if (index == 1) return false;
            if (index == 2) return false;
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
           if (side == GH_ParameterSide.Output) return false;
           if (Params.Input.Count <= 4) return false;
            if (index == 0) return false;
            if (index == 1) return false;
            if (index == 2) return false;
            return true;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)  
        {
            Param_GenericObject input = new Param_GenericObject();
            input.Optional = true;
            Params.RegisterInputParam(input, index);
            return input;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
          //  Params.UnregisterInputParameter(Params.Input[index]);
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 3; i < Params.Input.Count;i++ )
            {
                IGH_Param param = Params.Input[i];

                param.NickName = String.Format("T{0}", i-3);
                param.Name = String.Format("Tab {0}", i-3);
                param.Description = String.Format("The ui elements to include in tab {0}", i - 3);
                param.Access = GH_ParamAccess.list;
                param.Optional = true;
                param.DataMapping = GH_DataMapping.Flatten;

            }
        }




    }
}