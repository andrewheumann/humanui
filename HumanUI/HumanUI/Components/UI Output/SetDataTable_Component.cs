using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Linq;
using System.Data;

namespace HumanUI.Components
{
    public class SetDataTable_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetDataTable_Component class.
        /// </summary>
        public SetDataTable_Component()
          : base("Set Data Table", "SetDataTable",
              "Update the contents of a Data Table",
              "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data Table to Set", "DT", "The data table to set", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "D", "The data to display in the table, organized in branches by column.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Column Headings", "C", "The heading for each column, one for each column in Data", GH_ParamAccess.list);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object dataTableObj = null;
            if (!DA.GetData<object>(0, ref dataTableObj)) return;
            GH_Structure<GH_String> data;
            List<string> columnHeadings = new List<string>();

            bool hasData = DA.GetDataTree("Data", out data);
            bool hasColumnHeadings = DA.GetDataList("Column Headings", columnHeadings);


            DataGrid dg = HUI_Util.GetUIElement<DataGrid>(dataTableObj);


            List<List<string>> dataToConvert = new List<List<string>>();
            //check for rectangular data:




            for (int i = 0; i < data.Branches.Count; i++)
            {
                for (int j = 0; j < data.Branches[i].Count; j++)
                {
                    try
                    {
                        dataToConvert[j].Add(data.Branches[i][j].Value);
                    }
                    catch
                    {
                        dataToConvert.Add(new List<string>());
                        dataToConvert[j].Add(data.Branches[i][j].Value);
                    }

                }
            }
            if (!hasColumnHeadings)
            {
                DataView v = dg.ItemsSource as DataView;
                List<string> headings = new List<string>();
                foreach(DataColumn col in v[0].Row.Table.Columns)
                {
                    if(col.ColumnName != "HUI_Index")
                    {
                        headings.Add(col.ColumnName);
                    }
                
                }
                columnHeadings = headings;
            }
            if (columnHeadings.Count != data.Branches.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Column Heading Count doesn't match the data");
                return;
            }
            if (columnHeadings.Distinct().Count() != columnHeadings.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Column headings must be unique.");
                return;
            }

            DataTable dt = CreateDataTable_Component.ConvertListToDataTable(dataToConvert, columnHeadings);


            dg.ItemsSource = dt.AsDataView();


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
                return Properties.Resources.SetDataTable;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c2462873-d58e-4bb4-a1cd-31e351c133b3}"); }
        }
    }
}