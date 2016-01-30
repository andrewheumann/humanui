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

namespace HumanUI.Components
{
    public class SetGraph_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetGraph_Component()
            : base("Set Graph Contents", "SetGraph",
                "Use this to set the contents of a Graph",
                "Human", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph to modify", "G", "The Pie Graph object to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("New Pie Graph Values", "V", "The new values to graph in the pie graph", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddTextParameter("New Pie Graph Names", "N", "The names of the data items to be graphed", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "The title of the graph", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the graph", GH_ParamAccess.item);
            pManager[4].Optional = true;

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
            string Title = null;
            string SubTitle = null;

            //Get the Graph object and assign it
            if (!DA.GetData<object>("Graph to modify", ref GraphObject)) return;
            var ChartElem = HUI_Util.GetUIElement<ChartBase>(GraphObject);


            //set new title and subtitle or get old ones
            if (!DA.GetData<string>("Title", ref Title)) Title = ChartElem.ChartTitle;
            if (!DA.GetData<string>("SubTitle", ref SubTitle)) SubTitle = ChartElem.ChartSubTitle;


            //extract existing data from graph element
            SeriesModel dataExtractor = new SeriesModel();
            ChartSeries series = ChartElem.Series[0];
            dataExtractor.Chart = series.ItemsSource as ObservableCollection<ChartItem>;

            //if the data isnt supplied get it from old graph
            if (!DA.GetDataList<double>("New Pie Graph Values", listContents))
            {
                for (int i = 0; i < dataExtractor.Chart.Count; i++)
                {
                    listContents.Add(dataExtractor.Chart[i].Number);
                }
            }

            //if the data isnt supplied get it from old graph
            if (!DA.GetDataList<string>("New Pie Graph Names", names))
            {
                for (int i = 0; i < dataExtractor.Chart.Count; i++)
                {
                    names.Add(dataExtractor.Chart[i].Category);
                }
            }



            //make sure there are the same number of names and values
            if (names.Count != listContents.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "different number of names and values supplied");
            }


            //reconstruct graph data
            SeriesModel vm = new SeriesModel(names.ToList(), listContents.ToList());

            //assign new values back to graph
            series.ItemsSource = vm.Chart;
            ChartElem.ChartTitle = Title;
            ChartElem.ChartSubTitle = SubTitle;



            //DA.SetData("TEST", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));



        }

        public override bool Obsolete
        {
            get
            {
                return true;
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
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
                return Properties.Resources.SetCheckList;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{12A5A354-FC1B-4CEA-894A-BBEB71A23DB5}"); }
        }
    }
}