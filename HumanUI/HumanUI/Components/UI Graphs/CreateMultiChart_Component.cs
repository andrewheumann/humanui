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
using Grasshopper.Kernel.Parameters;

namespace HumanUI.Components
{
    public class CreateMultiChart_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateMultiChart_Component()
            : base("Create Multi Chart", "MultiChart",
                "Creates a Multi Chart from sets of Data and Categories.",
                "Human", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Data", "D", "The list of values to be charted.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Names", "N", "The names of the data items to be charted", GH_ParamAccess.list);
            pManager.AddTextParameter("Title", "T", "The title of the chart", GH_ParamAccess.item); // item access?
            pManager[2].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the chart", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("ClusterTitle", "cT", "The title of each chart cluster", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chart Type", "CT", "The type of chart to create", GH_ParamAccess.item, 0);
            Param_Integer ChartTypes = (Param_Integer)pManager[5];
            ChartTypes.AddNamedValue("Horizontal Cluster Bar Chart", 0);
            ChartTypes.AddNamedValue("Vertical Cluster Bar Chart", 1);
            ChartTypes.AddNamedValue("Horizontal Stacked Bar Chart", 2);
            ChartTypes.AddNamedValue("Vertical Stacked Bar Chart", 3);
            ChartTypes.AddNamedValue("Pie Chart", 4);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("MultiChart", "MC", "The MultiChart object", GH_ParamAccess.list);

        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Collection = new ObservableCollection<ChartItem>();
            string Title = "";
            string SubTitle = "";
            List<string> Clustertitle = new List<string>();
            GH_Structure<GH_Number> treeValues = new GH_Structure<GH_Number>();
            //GH_Structure<GH_String> treeNames = new GH_Structure<GH_String>();
            List<string> names = new List<string>();
            int chartType = 0;

            //get GH input data
            bool hasTitle = DA.GetData<string>("Title", ref Title);
            DA.GetData<string>("SubTitle", ref SubTitle);
            DA.GetDataList<string>("ClusterTitle", Clustertitle);
            DA.GetDataTree<GH_Number>("Data", out treeValues);
            //DA.GetDataTree<GH_String>("Names", out treeNames);
            DA.GetDataList<string>("Names", names);

            DA.GetData<int>("Chart Type", ref chartType);
            ChartBase ChartElem = null;
            switch (chartType)
            {
                case 0:
                    var ColumnCluster = new ClusteredColumnChart();
                    ChartElem = ColumnCluster;
                    break;
                case 1:
                    var BarCluster = new ClusteredBarChart();
                    ChartElem = BarCluster;
                    break;
                case 2:
                    var ColumnStack = new StackedColumnChart();
                    ChartElem = ColumnStack;
                    break;
                case 3:
                    var BarStack = new StackedBarChart();
                    ChartElem = BarStack;
                    break;
                case 4:
                    var pieElem = new PieChart();
                    ChartElem = pieElem;

                    break;
                default:
                    var defaultElem = new ClusteredBarChart();
                    ChartElem = defaultElem;
                    break;
            }

            //Give the chart its name

            ChartElem.ChartTitle = Title;
            ChartElem.ChartSubTitle = SubTitle;

           // MultiChartModel mcm = new MultiChartModel();
           // mcm.Series = new ObservableCollection<SeriesModel>();


            for (int i = 0; i < treeValues.Branches.Count; i++)
            {
                //package the data into a custom chart model and series
                List<double> listDouble = treeValues[i].ConvertAll(x => x.Value);
                SeriesModel vm = new SeriesModel(names.ToList(), listDouble,Clustertitle[i]);



                ChartSeries series = new ChartSeries();
                //We have to set the series data context rather than the whole chart
                series.DataContext = vm;

                series.SeriesTitle = Clustertitle[i];
                series.DisplayMember = "Category";
                series.ValueMember = "Number";

                Binding seriesBinding = new Binding();
                seriesBinding.Source = vm;
                seriesBinding.Path = new PropertyPath("Chart");
                BindingOperations.SetBinding(series, ChartSeries.ItemsSourceProperty, seriesBinding);

                ChartElem.Series.Add(series);
                //Pass data to the chart
              //  mcm.Series.Add(vm);

            }

            //Binding seriesSetBinding = new Binding();
            //seriesSetBinding.Source = mcm;
            //seriesSetBinding.Path = new PropertyPath("Series");
            //BindingOperations.SetBinding(ChartElem, ChartBase.SeriesSourceProperty, seriesSetBinding);







            ////Send Data to GH output
            DA.SetData("MultiChart", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));
            //  DA.SetData("Test","listNames");
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
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
                return Properties.Resources.CreateMultiChart;
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
