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

namespace HumanUI
{
    public class SetGraphAppearance_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetGraphAppearance_Component()
            : base("Graph Appearnce", "GraphAppearance",
                "Use this to set the appearnce of a Graph",
                "Human", "UI Graphs")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph to modify", "G", "The Pie Graph object to modify", GH_ParamAccess.item);
            pManager.AddColourParameter("Colors", "C", "The graph colors", GH_ParamAccess.list);
            //pManager.AddColourParameter("Background", "BC", "The background color of the element", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
           // pManager.AddGenericParameter("TEST", "T", "The Pie Graph object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object GraphObject = null;
            List<double> listContents = new List<double>();
            List<string> names = new List<string>();
            string Title=null;
            string SubTitle=null;
            List<System.Drawing.Color> Col = new List<System.Drawing.Color>();
            System.Drawing.Color fgCol = System.Drawing.Color.Transparent;

            //Get the Graph object and assign it
            if (!DA.GetData<object>("Graph to modify", ref GraphObject)) return;
            var ChartElem = HUI_Util.GetUIElement<ChartBase>(GraphObject);

            DA.GetDataList<System.Drawing.Color>("Colors", Col);
           // DA.GetData<System.Drawing.Color>("Background", ref fgCol);

            ResourceDictionaryCollection Pallete = new ResourceDictionaryCollection();

            Pallete = createResourceDictionary(Col);

            
           
            ChartElem.Palette = Pallete;

         
            

            ////extract existing data from graph element
            //CreateGraph_Component.CustomChartModel dataExtractor = new CreateGraph_Component.CustomChartModel();
            //ChartSeries series = ChartElem.Series[0];
            //dataExtractor.Chart = series.ItemsSource as ObservableCollection<CreateGraph_Component.ChartItem>;

            ////if the data isnt supplied get it from old graph
            //if (!DA.GetDataList<double>("New Pie Graph Values", listContents))  
            //{  for (int i = 0; i < dataExtractor.Chart.Count; i++)
            //    {
            //        listContents.Add(dataExtractor.Chart[i].Number);
            //    }
            //}

            ////if the data isnt supplied get it from old graph
            //if (!DA.GetDataList<string>("New Pie Graph Names", names))    
            //{
            //    for (int i = 0; i < dataExtractor.Chart.Count; i++)
            //    {
            //        names.Add(dataExtractor.Chart[i].Category);
            //    }
            //}



            ////make sure there are the same number of names and values
            //if (names.Count != listContents.Count)
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "different number of names and values supplied");
            //}


            ////reconstruct graph data
            //CreateGraph_Component.CustomChartModel vm = new CreateGraph_Component.CustomChartModel(names.ToList(), listContents.ToList());

            ////assign new values back to graph
            //series.ItemsSource = vm.Chart;
            //ChartElem.ChartTitle = Title;
            //ChartElem.ChartSubTitle = SubTitle;



            //DA.SetData("TEST", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));



        }

        private ResourceDictionaryCollection createResourceDictionary(List<System.Drawing.Color> col)
        {
            ResourceDictionaryCollection pallete = new ResourceDictionaryCollection();
            //ObservableCollection<ResourceDictionary> colorCollector = new ObservableCollection<ResourceDictionary>;
            ResourceDictionary PalleteColors = new ResourceDictionary();
            SolidColorBrush temp = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 70, 0));
            PalleteColors.Add("Brush0", temp );
            SolidColorBrush temp2= new SolidColorBrush(System.Windows.Media.Color.FromRgb(70, 0, 0));
            PalleteColors.Add("Brush1", temp2);
            //for (int i = 0; i < col.Count; i++)
            //{
            //    System.Drawing.Color color = col[i];
            //    SolidColorBrush active = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R,color.G, color.B));
            //    string name = "brush" + i;
            //    PalleteColors.Add(name, active);
                
                
            //}
            //colorCollector.Add("test" , PalleteColors);
            pallete.Add(PalleteColors);
            pallete.
            return pallete;
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
                return Properties.Resources.SetCheckList;
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