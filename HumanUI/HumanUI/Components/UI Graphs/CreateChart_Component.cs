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
using System.ComponentModel;

namespace HumanUI.Components
{
    public class CreateChart_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreateChart_Component()
            : base("Create Chart", " Chart",
                "Creates a Chart from Data and Categories.",
                "Human UI", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Data", "D", "The list of values to be charted.", GH_ParamAccess.list);
            pManager.AddTextParameter("Names", "N", "The names of the data items to be charted", GH_ParamAccess.list); 
            pManager.AddTextParameter("Title", "T", "The title of the Chart", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the Chart", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Chart Type", "CT", "The type of Chart to create", GH_ParamAccess.item, 0);
            Param_Integer ChartTypes = (Param_Integer)pManager[4];
            ChartTypes.AddNamedValue("Pie Chart", 0);
            ChartTypes.AddNamedValue("Horizontal Bar Chart", 1);
            ChartTypes.AddNamedValue("Vertical Bar Chart", 2);
            ChartTypes.AddNamedValue("Doughnut Chart", 3);
            ChartTypes.AddNamedValue("Gauge Chart", 4);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Chart", "C", "The Chart object", GH_ParamAccess.item);
        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
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
            List<double> listContents = new List<double>();
            List<string> names = new List<string>();
            int chartType = 0;
            

            //get GH input data
            DA.GetDataList<double>("Data",  listContents);
            DA.GetDataList<string>("Names", names);
            bool hasTitle = DA.GetData<string>("Title", ref Title);
            bool hasSubTitle = DA.GetData<string>("SubTitle", ref SubTitle);
            
            DA.GetData<int>("Chart Type", ref chartType);
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
                    var gaugeElem = new RadialGaugeChart();
                    ChartElem = gaugeElem;
                    
                    break;
                default:
                    var defaultElem = new PieChart();
                    ChartElem = defaultElem;
                    break;
            }
            //Create the chart and give it a name
            
            ChartElem.ChartTitle = Title;
            ChartElem.ChartTitleVisibility = hasTitle ? Visibility.Visible : Visibility.Hidden;
            ChartElem.ChartSubTitle = SubTitle;
                       
          

            //package the data into a custom chart model and series
            SeriesModel vm = new SeriesModel(names.ToList(), listContents.ToList());

            ChartElem.DataContext = vm;


            ChartSeries series = new ChartSeries();
            series.SeriesTitle = " ";
            series.DisplayMember = "Category";
            series.ValueMember = "Number";

            //set up the data binding for the series - this is useful so it can be reset later without redrawing the whole Chart
            Binding seriesBinding = new Binding();
            seriesBinding.Source = vm;
            seriesBinding.Path = new PropertyPath("Chart");
            BindingOperations.SetBinding(series, ChartSeries.ItemsSourceProperty, seriesBinding);
    
            
            
           // series.ItemsSource = vm.Chart;

            //Pass data to the chart
            ChartElem.Series.Add(series);
            ChartElem.ToolTipFormat = "{}Caption: {0}, Value: '{1}', Series: '{2}', Percentage: {3:P2}";
            

            ChartElem.MinWidth = 10;
            ChartElem.MinHeight = 10;
           
            DA.SetData("Chart", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.CreateChart;
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
