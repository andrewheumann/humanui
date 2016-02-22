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
    public class SetChart_LiveUpdate : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetList_Component class.
        /// </summary>
        public SetChart_LiveUpdate()
            : base("Set Chart Contents", "SetChart",
                "Use this to set the contents of a Chart",
                "Human UI", "UI Graphs + Charts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Chart to modify", "C", "The Chart object to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("New Chart Values", "V", "The new values to include in the Chart", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddTextParameter("New Chart Names", "N", "The names of the data items to be charted", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "The title of the Chart", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("SubTitle", "sT", "The subtitle of the Chart", GH_ParamAccess.item);
            pManager[4].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // pManager.AddGenericParameter("TEST", "T", "The Chart object", GH_ParamAccess.item);
        }


        private SeriesModel chartModel;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object ChartObject = null;
            List<double> listContents = new List<double>();
            List<string> names = new List<string>();
            string Title = null;
            string SubTitle = null;

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

            chartModel = ChartElem.DataContext as SeriesModel;

            if (chartModel == null)
            {
                chartModel = new SeriesModel();
            }

            bool valuesSupplied = DA.GetDataList<double>("New Chart Values", listContents);

            bool namesSupplied = DA.GetDataList<string>("New Chart Names", names);


            //make sure there are the same number of names and values
            if (valuesSupplied && namesSupplied && (names.Count != listContents.Count))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Different number of names and values supplied.");
                return;
            }


            //if the data isnt supplied modify it
            if (valuesSupplied)
            {
                //replace the ones already in there
                for (int i = 0; i < chartModel.Chart.Count; i++)
                {
                    if(listContents.Count>i){
                    chartModel.Chart[i].Number = (float)listContents[i];
                    }
                }
                //add any on the end
                if (listContents.Count > chartModel.Chart.Count)
                {
                    for (int i = chartModel.Chart.Count; i < listContents.Count;i++ )
                    {
                        chartModel.Chart.Add(new ChartItem() { Number = (float)listContents[i], Category = "UNSET" });
                    }
                }

                //subtract any off the end 
                if (listContents.Count < chartModel.Chart.Count)
                {
                    for (int i = chartModel.Chart.Count - 1; i >= listContents.Count; i--)
                    {
                        chartModel.Chart.RemoveAt(i);
                    }
                }
            }

            //check if the names array is different. We don't want to fire a full update unless we absolutely have to.

            bool hasChanged = false;

            var alreadyNames = chartModel.Chart.Select(item => item.Category).ToArray();
           //if the length of names is different, we know it's changed
            if (alreadyNames.Length != names.Count())
            {
                hasChanged = true;
            }
            else //if the length is the same, check each item
            {
                for (int i = 0; i < alreadyNames.Count(); i++)
                {
                    if (alreadyNames[i] != names[i])
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }
           



                //if the data is supplied and different than what's in the chart, modify it
                if (namesSupplied && hasChanged)
                {
                    //replace the ones already in there
                    for (int i = 0; i < chartModel.Chart.Count; i++)
                    {
                        if (names.Count > i)
                        {
                            chartModel.Chart[i].Category = names[i];
                        }
                    }
                    //add any on the end
                    if (names.Count > chartModel.Chart.Count)
                    {
                        for (int i = chartModel.Chart.Count; i < names.Count; i++)
                        {
                            chartModel.Chart.Add(new ChartItem() { Category = names[i], Number = float.NaN });
                        }
                    }

                    //subtract any off the end 
                    if (names.Count < chartModel.Chart.Count)
                    {
                        for (int i = chartModel.Chart.Count - 1; i >= names.Count; i--)
                        {
                            chartModel.Chart.RemoveAt(i);
                        }
                    }

                    //refresh display - the binding works when the values change but not when categories do.
                    var Values = chartModel.Chart.ToArray();

                    chartModel.Chart.Clear();

                    foreach (var item in Values)
                    {
                        chartModel.Chart.Add(item);
                    }



                }



           
           

        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
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
                return Properties.Resources.SetChart;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1C4AC2EC-3090-4D03-8D17-923417129692}"); }
        }
    }
}