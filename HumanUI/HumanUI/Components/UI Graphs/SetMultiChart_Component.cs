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
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using System.Windows;

namespace HumanUI.Components
{
    public class SetMultiChart_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetMultiChart_Component()
            : base("Set Multi Chart Contents", "SetMultiChart",
                "Use this to set the contents of a MultiChart",
                "Human", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Chart to modify", "G", "The Multi Chart object to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("New Chart Values", "D", "The new values to include in the Multi Chart", GH_ParamAccess.tree);
            pManager.AddTextParameter("New Chart Names", "N", "The names of the data items to be Charted", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "The title of the Chart", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the Chart", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddTextParameter("ClusterTitle", "cT", "The title of each bar cluster", GH_ParamAccess.list);
            pManager[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // pManager.AddGenericParameter("TEST", "T", "The Pie Chart object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object ChartObject = null;
            GH_Structure<GH_Number> NewValueTree = new GH_Structure<GH_Number>();
            List<string> names = new List<string>();
            string Title = "";
            string SubTitle = "";
            List<string> Clustertitle = new List<string>();

            //Get the Chart object and assign it
            if (!DA.GetData<object>("Chart to modify", ref ChartObject)) return;
            var ChartElem = HUI_Util.GetUIElement<ChartBase>(ChartObject);


            //set new title and subtitle or get old ones
            if (DA.GetData<string>("Title", ref Title))
            {
                ChartElem.ChartTitleVisibility = System.Windows.Visibility.Visible;
                ChartElem.ChartTitle = Title;
            }
            if (DA.GetData<string>("SubTitle", ref SubTitle)) ChartElem.ChartSubTitle = SubTitle;

            bool ClusterTitleSupplied = DA.GetDataList<string>("ClusterTitle", Clustertitle);

            bool valuesSupplied = DA.GetDataTree<GH_Number>("New Chart Values", out NewValueTree);

            bool namesSupplied = DA.GetDataList<string>("New Chart Names", names);


         //   MultiChartModel mcm = ChartElem.DataContext as MultiChartModel;

            //check if the cluster titles have changed to avoid unnecessary full redraws.
            bool clusterTitleChanged = false;

            if (ClusterTitleSupplied)
            {
                var alreadyClusterTitles = ChartElem.Series.Select(s => s.SeriesTitle).ToArray();
                if (Clustertitle.Count() != alreadyClusterTitles.Length)
                {
                    clusterTitleChanged = true;
                }
                else
                {
                    for (int i = 0; i < alreadyClusterTitles.Length; i++)
                    {
                        if (alreadyClusterTitles[i] != Clustertitle[i])
                        {
                            clusterTitleChanged = true;
                            break;
                        }
                    }
                }
            }


            //check if the names array is different. We don't want to fire a full update unless we absolutely have to.
            bool haveNamesChanged = false;
            var firstModel = ChartElem.Series[0].DataContext as SeriesModel;
            var alreadyNames = firstModel.Chart.Select(item => item.Category).ToArray();
            if (namesSupplied)
            {
               
              
                
                //if the length of names is different, we know it's changed
                if (alreadyNames.Length != names.Count())
                {
                    haveNamesChanged = true;
                }
                else //if the length is the same, check each item
                {
                    for (int i = 0; i < alreadyNames.Count(); i++)
                    {
                        if (alreadyNames[i] != names[i])
                        {
                            haveNamesChanged = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                names = alreadyNames.ToList();
            }

            bool removedCharts = false;

            //Remove extra data
            if (NewValueTree.Branches.Count < ChartElem.Series.Count())
            {
                removedCharts = true;
                for (int k = ChartElem.Series.Count() - 1; k >= NewValueTree.Branches.Count; k--)
                {
                    ChartElem.Series.RemoveAt(k);
                }
            }

            bool addedCharts = false;


            for (int j = 0; j < NewValueTree.Branches.Count; j++)
            {
                List<Double> valueList = NewValueTree.Branches[j].Select(n => n.Value).ToList();
                
                //modify existing series
                if (ChartElem.Series.Count > j)
                {
                    var ChartSeries = ChartElem.Series[j];
                    var model = ChartSeries.DataContext as SeriesModel;

                    if(ClusterTitleSupplied) ChartSeries.SeriesTitle = Clustertitle[j];

                  
                    //make sure there are the same number of names and values
                    if (valuesSupplied && namesSupplied && (names.Count != valueList.Count))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Different number of names and values supplied.");
                        return;
                    }

                    //if the data isnt supplied modify it
                    if (valuesSupplied)
                    {
                        //replace the ones already in there
                        for (int i = 0; i < model.Chart.Count; i++)
                        {
                            if (valueList.Count > i)
                            {
                                model.Chart[i].Number = (float)valueList[i];
                            }
                        }
                        //add any on the end
                        if (valueList.Count > model.Chart.Count)
                        {
                            for (int i = model.Chart.Count; i < valueList.Count; i++)
                            {
                                model.Chart.Add(new ChartItem() { Number = (float)valueList[i], Category = "UNSET" });
                            }
                        }

                        //subtract any off the end 
                        if (valueList.Count < model.Chart.Count)
                        {
                            for (int i = model.Chart.Count - 1; i >= valueList.Count; i--)
                            {
                                model.Chart.RemoveAt(i);
                            }
                        }
                    }


                    //if the data is supplied and different than what's in the chart, modify it
                    if (namesSupplied && haveNamesChanged)
                    {
                        //replace the ones already in there
                        for (int i = 0; i < model.Chart.Count; i++)
                        {
                            if (names.Count > i)
                            {
                                model.Chart[i].Category = names[i];
                            }
                        }
                        //add any on the end
                        if (names.Count > model.Chart.Count)
                        {
                            for (int i = model.Chart.Count; i < names.Count; i++)
                            {
                                model.Chart.Add(new ChartItem() { Category = names[i], Number = float.NaN });
                            }
                        }

                        //subtract any off the end 
                        if (names.Count < model.Chart.Count)
                        {
                            for (int i = model.Chart.Count - 1; i >= names.Count; i--)
                            {
                                model.Chart.RemoveAt(i);
                            }
                        }

                      
                    }
                    if (haveNamesChanged || clusterTitleChanged)
                    {
                        //refresh display - the binding works when the values change but not when categories do.
                        var Values = model.Chart.ToArray();

                        model.Chart.Clear();

                        foreach (var item in Values)
                        {
                            model.Chart.Add(item);
                        }
                    }


                }
                else
                {
                    addedCharts = true;
                    //Add new series
                    SeriesModel vm = new SeriesModel(names.ToList(), valueList);



                    ChartSeries series = new ChartSeries();
                    //We have to set the series data context rather than the whole chart
                    series.DataContext = vm;

                    series.SeriesTitle = Clustertitle[j];
                    series.DisplayMember = "Category";
                    series.ValueMember = "Number";

                    Binding seriesBinding = new Binding();
                    seriesBinding.Source = vm;
                    seriesBinding.Path = new PropertyPath("Chart");
                    BindingOperations.SetBinding(series, ChartSeries.ItemsSourceProperty, seriesBinding);

                    ChartElem.Series.Add(series);


                    
                
                }
            }



            if (addedCharts || removedCharts)
            {
                
                var oldSeries = ChartElem.Series;
                ChartElem.SeriesSource = null;
              //  ChartElem.Series.Clear();
                ChartElem.SeriesSource = oldSeries;
            }







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
                return Properties.Resources.SetMultiChart;
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