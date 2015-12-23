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
    public class SetPieGraph_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetPieGraph_Component()
            : base("Set Pie Graph Contents", "SetPieGraph",
                "Use this to set the contents of a Pie Graph",
                "Human", "UI Graphs")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Pie Graph to modify", "PG", "The Pie Graph object to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("New Pie Graph Values", "V", "The new values to graph in the pie graph", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddTextParameter("New Pie Graph Names", "N", "The names of the data items to be graphed", GH_ParamAccess.list);
            pManager[2].Optional = true;
            // pManager.AddTextParameter("Title", "T", "The title of the graph", GH_ParamAccess.item, "Title");
            // pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the graph", GH_ParamAccess.item, "subTitle");
            //pManager.AddIntegerParameter()


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("TEST", "T", "The Pie Graph object", GH_ParamAccess.item);
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

            //string Title;
            // string SubTitle;

            if (!DA.GetData<object>("Pie Graph to modify", ref GraphObject)) return;
            DA.GetDataList<double>("New Pie Graph Values", listContents);
            DA.GetDataList<string>("New Pie Graph Names", names);
            
            var ChartElem = HUI_Util.GetUIElement<PieChart>(GraphObject);
            //CreatePieGraph_Component.CustomChartModel vm = new CreatePieGraph_Component.CustomChartModel();
            ChartSeries series = ChartElem.Series[0];
            //vm.Chart = series.ItemsSource as ObservableCollection<CreatePieGraph_Component.ChartItem>;
            //if (!(vm.Chart.Count == listContents.Count))
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "the number of names and values are not matching");
            //};
            
            //for (int i = 0; i < vm.Chart.Count; i++)
            //{
            //    vm.Chart[i].Number = listContents[i];
            //    vm.Chart[i].Category = names[i];
            //}
            //series.ItemsSource = vm.Chart;
            //ChartElem.Series.Clear();

            CreatePieGraph_Component.CustomChartModel vm1 = new CreatePieGraph_Component.CustomChartModel(names.ToList(), listContents.ToList());
            //ChartSeries series = new ChartSeries();
            //series.SeriesTitle = "Errors";
            //series.DisplayMember = "Category";
            //series.ValueMember = "Number";
            series.ItemsSource = vm1.Chart;
            ChartElem

            //Pass data to the chart
            //ChartElem.Series.Add(series);


            // ScrollViewer sv = HUI_Util.GetUIElement<ScrollViewer>(ListObject);
            // ItemsControl ic = sv.Content as ItemsControl;





            //if (!DA.GetDataList<bool>("Selected", isSelected))
            //{

            //}


            //ic.Items.Clear();


            //for (int i = 0; i < listContents.Count; i++)
            //{
            //    string item = listContents[i];

            //    CheckBox cb = new CheckBox();
            //    cb.Margin = new System.Windows.Thickness(2);
            //    cb.Content = item;
            //    if (isSelected.Count > 0)
            //    {
            //        bool isSel = isSelected[i % isSelected.Count];
            //        cb.IsChecked = isSel;
            //    }
            //    ic.Items.Add(cb);
            //}


            DA.SetData("TEST", new UIElement_Goo(ChartElem, "Chart Elem", InstanceGuid, DA.Iteration));



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