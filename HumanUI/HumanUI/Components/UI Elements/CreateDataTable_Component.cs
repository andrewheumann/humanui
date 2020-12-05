using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Windows.Controls;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel.Parameters;
using DataGrid = System.Windows.Controls.DataGrid;

namespace HumanUI.Components
{
    public class CreateDataTable_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DataTable class.
        /// </summary>
        public CreateDataTable_Component()
          : base("Create Data Table", "DataTable",
              "Creates a Data Table view",
              "Human UI", "UI Elements")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "D", "The data to display in the table, organized in branches by column.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Column Headings", "C", "The heading for each column, one for each column in Data", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Allow Sorting", "S", "Set to true to allow sorting by column values", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Column Sizing Mode", "CS", "Sizing Mode for columns. Pick from predefined \"special\" values or supply a fixed numerical width.", GH_ParamAccess.item, -1);

            Param_Integer sizingMode = pManager[3] as Param_Integer;
            sizingMode.AddNamedValue("Equal", -1);
            sizingMode.AddNamedValue("Size to Cells", -2);
            sizingMode.AddNamedValue("Size to Header", -3);
            sizingMode.AddNamedValue("Auto", -4);



        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DataTable", "DT", "The Data Table element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_String> data;
            List<string> columnHeadings = new List<string>();
            bool allowSorting = true;
            int columnWidth = -1;

            if (!DA.GetDataTree("Data", out data)) return;
            bool hasColumnHeadings = DA.GetDataList("Column Headings", columnHeadings);
            DA.GetData("Allow Sorting", ref allowSorting);
            bool hasColumnWidth = DA.GetData("Column Sizing Mode", ref columnWidth);


            DataGrid dg = new DataGrid();
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

            DataTable dt = ConvertListToDataTable(dataToConvert, columnHeadings);
          
            dg.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            dg.AutoGenerateColumns = true;
            if (columnWidth == -1)
            {
                dg.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            else if (columnWidth == -2)
            {
                dg.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);
            }
            else if (columnWidth == -3)
            {
                dg.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
            }
            else if (columnWidth == -4)
            {
                dg.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }
            else
            {
                dg.ColumnWidth = new DataGridLength(columnWidth);
            }

            dg.IsReadOnly = true;
            dg.ItemsSource = dt.AsDataView();
            dg.SelectionMode = DataGridSelectionMode.Single;
            dg.CanUserSortColumns = allowSorting;
            //DataGridLengthConverter DGLC = new DataGridLengthConverter();
            //if (hasColumnWidths)
            //{
            //    for (int i=0;i< dg.Columns.Count;i++)
            //    {
            //        var col = dg.Columns[i];

            //        col.Width = (DataGridLength)DGLC.ConvertFromString(columnWidths[i % columnWidths.Count]);
            //    }
            //}

            DA.SetData("DataTable", new UIElement_Goo(dg, "Data Table", InstanceGuid, DA.Iteration));


        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
           
            //Set properties on the columns during auto-generation 
            switch (e.Column.Header.ToString())
            {
                
                case "HUI_Index":
                    e.Column.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }


        internal static DataTable ConvertListToDataTable(List<List<string>> list, List<string> columnHeadings)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Count() > columns)
                {
                    columns = array.Count();
                }
            }
            columns++;

            table.Columns.Add("HUI_Index");

            // Add columns.
            for (int i = 1; i < columns; i++)
            {
                var heading = columnHeadings[i - 1];
                if (String.IsNullOrWhiteSpace(heading) || heading == ".") heading = '\u2800'.ToString();
                table.Columns.Add(heading,typeof(string));

            }

            int index = 0;
            // Add rows.
            foreach (var array in list)
            {

                List<string> rowValues = new List<string>();
                rowValues.Add(index.ToString());
                index++;
                rowValues.AddRange(array);
                table.Rows.Add(rowValues.ToArray());
            }

            return table;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DataTable;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{b29e654e-b952-4d58-acf2-a60c4358d6e3}");
    }
}