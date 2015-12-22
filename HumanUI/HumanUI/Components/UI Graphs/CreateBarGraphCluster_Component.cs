using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using De.TorstenMandelkow.MetroChart;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace HumanUI
{
    public class CreateBarGraphCluster_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateBarGraphCluster_Component()
            : base("Create Clustered Bar Graph", "BarGraphC",
                "Creates a Clustered Bar Graph from sets of Data and Categories.",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Data", "D", "The list of data items to be graphed.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Names", "N", "The names of the data items to be graphed", GH_ParamAccess.list);
            pManager.AddTextParameter("Title", "T", "The title of the graph", GH_ParamAccess.item, "Title"); // item access?
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the graph", GH_ParamAccess.item, "Subtitle");
            pManager.AddTextParameter("ClusterTitle", "cT", "The title of each bar cluster", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Bar Graph Cluster", "BGC", "The Bar Graph Cluster object", GH_ParamAccess.list);
            
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Collection = new ObservableCollection<ChartItem>();
            string Title = "title";
            string SubTitle = "subtitle";
            List<string> Clustertitle = new List<string>();
            GH_Structure<GH_Number> treeValues = new GH_Structure<GH_Number>();
            //GH_Structure<GH_String> treeNames = new GH_Structure<GH_String>();
            List<string> names = new List<string>();

            //get GH input data
            DA.GetData<string>("Title", ref Title);
            DA.GetData<string>("SubTitle", ref SubTitle);
            DA.GetDataList<string>("ClusterTitle",  Clustertitle);
            DA.GetDataTree<GH_Number>("Data", out treeValues);
            //DA.GetDataTree<GH_String>("Names", out treeNames);
            DA.GetDataList<string>("Names", names);

            //Create the chart and give it a name
            var ChartElem = new ClusteredBarChart();
            ChartElem.ChartTitle = Title;
            ChartElem.ChartSubTitle = SubTitle;

            for (int i = 0; i< treeValues.Branches.Count; i++)
            {
                //package the data into a custom chart model and series
                //List<string> listNames = treeNames[i].ConvertAll(x => x.ToString());
                List<double> listDouble = treeValues[i].ConvertAll(x => x.Value);
                CustomChartModel vm = new CustomChartModel(names.ToList(), listDouble);
                ChartSeries series = new ChartSeries();
                series.SeriesTitle = Clustertitle[i];
                series.DisplayMember = "Category";
                series.ValueMember = "Number";
                series.ItemsSource = vm.Chart;

                //Pass data to the chart
                ChartElem.Series.Add(series);
            }







            //Popup Window for testing
            //Window window = new Window();
            //Grid grid = new Grid();
            //window.Content = grid;
            //grid.Children.Add(ChartElem);
            //window.Show();

            //ChartElem.Height = 300;
            //ChartElem.Width = 300;
            ////Send Data to GH output
            DA.SetData("Bar Graph Cluster", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));
            DA.SetData("Test","listNames");
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
                return Properties.Resources.CreateListBox;
            }
        }

        // class which represent a data point in the chart
        public class ChartItem
        {
            public string Category { get; set; }

            public double Number { get; set; }
            //public ChartItem (string Cat, int Num)
            //{
            //    Category = Cat;
            //    Number = Num;
            //}
        }

        public class CustomChartModel   //loops through 2 lists and sets up a matching pairs of chart items
        {
            public ObservableCollection<ChartItem> Chart { get; private set; }

            public CustomChartModel(List<string> categories, List<double> values)
            {
                Chart = new ObservableCollection<ChartItem>();
                for (int i = 0; i < categories.Count; i++)
                {
                    Chart.Add(new ChartItem() { Category = categories[i], Number = values[i] });
                }
            }

        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{66FA84E1-D224-4B4B-8DA4-E3E40CA815D5}"); }
                                   
        }
    }
}
