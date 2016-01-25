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

namespace HumanUI
{
    public class SetMultiGraph_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetMultiGraph_Component()
            : base("Set Multi Graph Contents", "SetMultiGraph",
                "Use this to set the contents of a MultiGraph",
                "Human", "UI Graphs")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph to modify", "G", "The Multi Graph object to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("New Graph Values", "D", "The new values to graph in the Multi graph", GH_ParamAccess.tree);
            pManager[1].Optional = true;
            pManager.AddTextParameter("New Graph Names", "N", "The names of the data items to be graphed", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "The title of the graph", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the graph", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddTextParameter("ClusterTitle", "cT", "The title of each bar cluster", GH_ParamAccess.list);
            pManager[5].Optional = true;

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
            List<string> Clustertitle = new List<string>();

            //Get the Graph object and assign it
            if (!DA.GetData<object>("Graph to modify", ref GraphObject)) return;
            var ChartElem = HUI_Util.GetUIElement<ChartBase>(GraphObject);


            //set new title and subtitle or get old ones
            if (!DA.GetData<string>("Title", ref Title)) Title = ChartElem.ChartTitle;
            if(!DA.GetData<string>("SubTitle", ref SubTitle)) SubTitle=ChartElem.ChartSubTitle;
            DA.GetDataList<string>("ClusterTitle", Clustertitle);

            //extract existing data from graph element
            CreateGraph_Component.CustomChartModel dataExtractor = new CreateGraph_Component.CustomChartModel();
            ChartSeries series = new ChartSeries();

            for (int j = 0;j < ChartElem.Series.Count; j++)
            {
                series = ChartElem.Series[j];
                dataExtractor.Chart = series.ItemsSource as ObservableCollection<CreateGraph_Component.ChartItem>;

                //if the data isnt supplied get it from old graph
                if (!DA.GetDataList<double>("New Graph Values", listContents))
                {
                    for (int i = 0; i < dataExtractor.Chart.Count; i++)
                    {
                        listContents.Add(dataExtractor.Chart[i].Number);
                    }
                }

                //if the data isnt supplied get it from old graph
                if (!DA.GetDataList<string>("New Graph Names", names))
                {
                    for (int i = 0; i < dataExtractor.Chart.Count; i++)
                    {
                        names.Add(dataExtractor.Chart[i].Category);
                    }
                }
            }


            //make sure there are the same number of names and values
            if (names.Count != listContents.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "different number of names and values supplied");
            }


            //reconstruct graph data
            CreateGraph_Component.CustomChartModel vm = new CreateGraph_Component.CustomChartModel(names.ToList(), listContents.ToList());

            //assign new values back to graph
            series.ItemsSource = vm.Chart;
            ChartElem.ChartTitle = Title;
            ChartElem.ChartSubTitle = SubTitle;



            //DA.SetData("TEST", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));



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
            get { return new Guid("{12A5A354-FC1B-4CEA-394A-BBEB71A23DB5}"); }
        }
    }
}