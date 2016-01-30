using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using De.TorstenMandelkow.MetroChart; 

using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Media;
using System.Windows;
using System.Drawing;

namespace HumanUI.Components
{
    public class SetChartAppearance_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetChartAppearance_Component()
            : base("Chart Appearance", "ChartAppearance",
                "Use this to set the appearnce of a Chart",
                "Human", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Chart to modify", "G", "The Pie Chart object to modify", GH_ParamAccess.item);
            pManager.AddColourParameter("Colors", "C", "The Chart colors", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Legend", "L", "Legend on?", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Title", "T", "Title on?", GH_ParamAccess.item);
            pManager[3].Optional = true;
            //pManager.AddColourParameter("Background", "BC", "The background color of the element", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
         //   pManager.AddGenericParameter("TEST", "T", "The Pie Chart object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object ChartObject = null;
            List<double> listContents = new List<double>();
            List<string> names = new List<string>();
            bool Legend = true;
            bool Title = true;
            
            List<System.Drawing.Color> Col = new List<System.Drawing.Color>();
            System.Drawing.Color fgCol = System.Drawing.Color.Transparent;

            //Get the Chart object and assign it
            if (!DA.GetData<object>("Chart to modify", ref ChartObject)) return;
            var ChartElem = HUI_Util.GetUIElement<ChartBase>(ChartObject);

            bool hasLegend = DA.GetData<bool>("Legend", ref Legend);
            bool hasTitle = DA.GetData<bool>("Title", ref Title);
            bool hasColors = DA.GetDataList<System.Drawing.Color>("Colors", Col);

            if (hasColors)
            {
                ResourceDictionaryCollection Palette = new ResourceDictionaryCollection();
                Palette = createResourceDictionary(Col);

                System.Windows.Media.Brush BorderBrush = new SolidColorBrush(HUI_Util.ToMediaColor(Col[0]));
                ChartElem.BorderBrush = BorderBrush;
                ChartElem.Palette = Palette;

            }

            if (hasLegend)
            {
                Visibility LegendVis = Visibility.Hidden;
                if (Legend) LegendVis = Visibility.Visible;
                ChartElem.ChartLegendVisibility = LegendVis;
            }
            if (hasTitle)
            {
                Visibility TitleVis = Visibility.Hidden;
                if (Title) TitleVis = Visibility.Visible;
                ChartElem.ChartTitleVisibility = TitleVis;
            }

         

          

           
           
            
           

        }

        private ResourceDictionaryCollection createResourceDictionary(List<System.Drawing.Color> col)
        {
            ResourceDictionaryCollection palette = new ResourceDictionaryCollection();
            
           
            for (int i = 0; i < col.Count; i++)
            {
                ResourceDictionary PalleteColors = new ResourceDictionary();
                System.Drawing.Color color = col[i];
                SolidColorBrush active = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                string name = "brush" + i;
                PalleteColors.Add(name, active);
                palette.Add(PalleteColors);
            }
          
            
            
            return palette;
        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
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
                return Properties.Resources.ChartAppearance;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{12A5A354-FC1B-4CEA-894A-BBEB99A23DB5}"); }
        }
    }


    //public class ResourceDictionary   //loops through 2 lists and sets up a matching pairs of brushes items
    //{
    //    public Dictionary<string,SolidColorBrush> Chart { get; set; }

    //    public ResourceDictionary(List<System.Drawing.Color> values)
    //    {
    //        Chart = new Dictionary<string, SolidColorBrush>();
    //        for (int i = 0; i < values.Count; i++)
    //        {
    //            //Chart.Add(new SolidColorBrush() { Category = categories[i], Number = values[i] });
    //            Chart.Add(i.ToString(),new SolidColorBrush(System.Windows.Media.Color.FromRgb(values[i].R, values[i].G,values[i].B)));
    //        }
    //    }
    //    public ResourceDictionary()
    //    {
    //        Chart = new Dictionary<string, SolidColorBrush>();
     //   }

    //}
}