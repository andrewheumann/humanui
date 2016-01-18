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
using Grasshopper.Kernel.Parameters;

namespace HumanUI
{
    public class CreateGraph_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateGraph_Component()
            : base("Create Graph", " Graph",
                "Creates a Graph from Data and Categories.",
                "Human", "UI Graphs")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Data", "D", "The list of data items to be graphed.", GH_ParamAccess.list);
            pManager.AddTextParameter("Names", "N", "The names of the data items to be graphed", GH_ParamAccess.list); 
            pManager.AddTextParameter("Title", "T", "The title of the graph", GH_ParamAccess.item, "Title");
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the graph", GH_ParamAccess.item, "subTitle");
            pManager.AddIntegerParameter("Graph Type", "GT", "The type of Graph to create", GH_ParamAccess.item, 0);
            Param_Integer graphTypes = (Param_Integer)pManager[4];
            graphTypes.AddNamedValue("Pie Graph", 0);
            graphTypes.AddNamedValue("Horizontal Bar Graph", 1);
            graphTypes.AddNamedValue("Vertical Bar Graph", 2);
            graphTypes.AddNamedValue("Doughnut Graph", 3);
            graphTypes.AddNamedValue("Gauge Graph", 4);
            pManager.AddNumberParameter("GraphSize", "S", "The size of the graph", GH_ParamAccess.item, 300);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph", "G", "The Graph object", GH_ParamAccess.item);
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
            List<double> listContents = new List<double>();
            List<string> names = new List<string>();
            int chartType = 0;
            double size = 300;

            //get GH input data
            DA.GetDataList<double>("Data",  listContents);
            DA.GetDataList<string>("Names", names);
            DA.GetData<string>("Title", ref Title);
            DA.GetData<string>("SubTitle", ref SubTitle);
            DA.GetData<double>("GraphSize", ref size);
            DA.GetData<int>("Graph Type", ref chartType);
            ChartBase ChartElem = null;
            switch (chartType)
            {
                case 0:
                    var pieElem = new PieChart();
                    ChartElem = pieElem;
                    break;
                case 1:
                    var barElem = new ClusteredBarChart();
                    ChartElem = barElem;
                    break;
                case 2:
                    var columnElem = new ClusteredColumnChart();
                    ChartElem = columnElem;
                    break;
                case 3:
                    var doughnutElem = new DoughnutChart();
                    ChartElem = doughnutElem;
                    break;
                case 4:
                    var guageElem = new RadialGaugeChart();
                    ChartElem = guageElem;
                    
                    break;
                default:
                    var defaultElem = new PieChart();
                    ChartElem = defaultElem;
                    break;
            }
            //Create the chart and give it a name
            
            ChartElem.ChartTitle = Title;
            ChartElem.ChartSubTitle = SubTitle;
                       
            //package the data into a custom chart model and series
            CustomChartModel vm = new CustomChartModel(names.ToList(), listContents.ToList());
            ChartSeries series = new ChartSeries();
            series.SeriesTitle = " ";
            series.DisplayMember = "Category";
            series.ValueMember = "Number";
            series.ItemsSource = vm.Chart;

            //Pass data to the chart
            ChartElem.Series.Add(series);
            ChartElem.ToolTipFormat = "{}Caption: {0}, Value: '{1}', Series: '{2}', Percentage: {3:P2}";
            

            ChartElem.MinWidth = 10;
            ChartElem.MinHeight = 10;
           
            DA.SetData("Graph", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreatePieGraph;
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
            public ObservableCollection<ChartItem> Chart { get;  set; }

            public CustomChartModel(List<string> categories, List<double> values)
            {
                Chart = new ObservableCollection<ChartItem>();
                for (int i = 0; i < categories.Count; i++)
                {
                    Chart.Add(new ChartItem() { Category = categories[i], Number = values[i] });
                }
            }
            public CustomChartModel()
            {
                Chart = new ObservableCollection<ChartItem>();
            }

        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1A96F054-26DD-45C6-B09D-2760B496BB0A}"); }
                                   
        }
    }
}
