using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HumanUI.Components.UI_Containers
{
    /// <summary>
    /// A component to create a grid container
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateGrid_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateGrid_Component class.
        /// </summary>
        public CreateGrid_Component()
            : base("Create Grid", "Grid",
                "Create a container with absolutely positioned elements. \n Their input order determines their Z order - set the margins \nwith the \"Adjust Element Positioning\" component to locate \nelements inside the grid.\n Use column and row definitions to create more advanced grids.",
                "Human", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to place in the grid", GH_ParamAccess.list);
            pManager.AddNumberParameter("Width", "W", "The width of the grid", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Height", "H", "The height of the grid", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Row Definitions", "RD", "An optional list of Row Heights. Use numbers for absolute sizes and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Column Definitions", "CD", "An optional list of Column Widths. Use numbers for absolute sizes and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Element Row", "ER", "The rows to place the elements in, counting from 0 at the top.", GH_ParamAccess.list);
            pManager[5].Optional = true;
            pManager.AddIntegerParameter("Element Column", "EC", "The columns to place the elements in, counting from 0 at the left.", GH_ParamAccess.list);
            pManager[6].Optional = true;
            pManager.AddIntegerParameter("Element Row Span", "ERS", "How many rows each element should span. This will be 1 by default.", GH_ParamAccess.list, 1);
            pManager[7].Optional = true;
            pManager.AddIntegerParameter("Element Column Span", "ECS", "How many columns each element should span. This will be 1 by default.", GH_ParamAccess.list, 1);
            pManager[8].Optional = true;



        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "S", "The combined group of elements", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();
            double width = 0;
            double height = 0;
            List<string> rowDefinitions = new List<string>();
            List<string> colDefinitions = new List<string>();
            List<int> elementRows = new List<int>();
            List<int> elementCols = new List<int>();
            List<int> elementRowSpans = new List<int>();
            List<int> elementColSpans = new List<int>();



            if (!DA.GetDataList<UIElement_Goo>("UI Elements", elementsToAdd)) return;
            bool hasWidth = DA.GetData<double>("Width", ref width);
            bool hasHeight = DA.GetData<double>("Height", ref height);

            bool hasRowDefs = DA.GetDataList<string>("Row Definitions", rowDefinitions);
            bool hasColDefs = DA.GetDataList<string>("Column Definitions", colDefinitions);

            bool hasElementRows = DA.GetDataList<int>("Element Row", elementRows);
            bool hasElementCols = DA.GetDataList<int>("Element Column", elementCols);
            DA.GetDataList<int>("Element Row Span", elementRowSpans);
            DA.GetDataList<int>("Element Column Span", elementColSpans);

            //initialize the grid
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.Name = "GH_Grid";
            if (hasWidth)
            {
                grid.Width = width;
            }
            else
            {
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            }
            if (hasHeight)
            {
                grid.Height = height;
            }
            else
            {
                grid.VerticalAlignment = VerticalAlignment.Stretch;
            }
           

            //set up a "GridLengthConverter" to handle parsing our strings.
            GridLengthConverter gridLengthConverter = new GridLengthConverter();

            //set up rows and columns if present
            if (hasColDefs)
            {
                foreach (string colDef in colDefinitions)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = (GridLength)gridLengthConverter.ConvertFromString(colDef);
                    grid.ColumnDefinitions.Add(cd);
                }

            }
            if (hasRowDefs)
            {
                foreach (string rowDef in rowDefinitions)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = (GridLength)gridLengthConverter.ConvertFromString(rowDef);
                    grid.RowDefinitions.Add(rd);
                }

            }


            //for all the elements to add
            for (int i = 0; i < elementsToAdd.Count; i++)
            {
                UIElement_Goo u = elementsToAdd[i];
                //make sure it doesn't already have a parent
                HUI_Util.removeParent(u.element);
                FrameworkElement fe = u.element as FrameworkElement;
                if (fe != null)
                {
                    //set its alignment to be relative to upper left - this makes margin-based positioning easy
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Top;

                    //set up row and column positioning + spans
                    if (hasElementCols && elementCols.Count > 0 && elementColSpans.Count > 0)
                    {
                        Grid.SetColumn(fe, elementCols[i % elementCols.Count]); // using hacky fake longest list matching. Will create a repeating pattern if it doesn't know what to do.
                        Grid.SetColumnSpan(fe, elementColSpans[i % elementColSpans.Count]);
                    }
                    if (hasElementRows && elementRows.Count > 0 && elementRowSpans.Count > 0)
                    {
                        Grid.SetRow(fe, elementRows[i % elementRows.Count]); // using hacky fake longest list matching. Will create a repeating pattern if it doesn't know what to do.
                        Grid.SetRowSpan(fe, elementRowSpans[i % elementRowSpans.Count]);

                    }

                }
                //add it to the grid
                grid.Children.Add(u.element);
            }
            //pass the grid out
            DA.SetData("Grid", new UIElement_Goo(grid, "Grid", InstanceGuid, DA.Iteration));
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
                return Properties.Resources.createGrid;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{B618569A-868D-4A88-A035-FAA1416A841F}"); }
        }
    }
}