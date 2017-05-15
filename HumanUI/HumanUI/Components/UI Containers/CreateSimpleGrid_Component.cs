using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Grasshopper.Kernel.Types;

namespace HumanUI.Components.UI_Containers
{
    /// <summary>
    /// A component to create a simple grid container
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateSimpleGrid_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateSimpleGrid_Component()
          : base("Create Simple Grid", "SimpleGrid",
                "Create a container with elements in a grid according to the path structure provided. Each branch path will be treated as a column and Elements will be placed in the column from top to bottom. Use the \"Adjust Element Positioning\" component to locate elements inside the grid cell. Use column and row definitions to control sizing.",
                "Human UI", "UI Containers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddGenericParameter("UI Elements", "E", "The UI elements to place in the grid. Each path branch will form a column of the provided UI Elements from top to bottom.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Width", "W", "The width of the grid", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Height", "H", "The height of the grid", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Row Definitions", "RD", "An optional repeating pattern of Row Heights - use 'Auto' to inherit, numbers for absolute sizes, and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list, "Auto");
            pManager[3].Optional = true;
            pManager.AddTextParameter("Column Definitions", "CD", "An optional flat list of Column Widths - use 'Auto' to inherit, use numbers for absolute sizes, and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list, "Auto");
            pManager[4].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Simple Grid", "S", "The combined group of elements", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Grasshopper.Kernel.Data.GH_Structure<IGH_Goo> elementsToAdd = new Grasshopper.Kernel.Data.GH_Structure<IGH_Goo>();
            double width = 0;
            double height = 0;
            List<string> rowDefinitions = new List<string>();
            List<string> colDefinitions = new List<string>();
            
            if (!DA.GetDataTree<IGH_Goo>(0, out elementsToAdd)) return;
            bool hasWidth = DA.GetData<double>("Width", ref width);
            bool hasHeight = DA.GetData<double>("Height", ref height);

            bool hasRowDefs = DA.GetDataList<string>("Row Definitions", rowDefinitions);
            bool hasColDefs = DA.GetDataList<string>("Column Definitions", colDefinitions);


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
                for (int i = 0; i < elementsToAdd.PathCount; i++)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = (GridLength)gridLengthConverter.ConvertFromString(colDefinitions[i % colDefinitions.Count]);  // use repeating pattern of supplied list
                    grid.ColumnDefinitions.Add(cd);
                }

            }

            if (hasRowDefs)
            {
                int maxCount = 0;

                // Find the count of the longest list
                for (int i = 0; i < elementsToAdd.PathCount; i++)
                {
                    if (elementsToAdd.Branches[i].Count > maxCount)
                    {
                        maxCount = elementsToAdd.Branches[i].Count;
                    }
                }

                // Build up the row heights based on a repeating pattern
                for (int i = 0; i < maxCount; i++)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = (GridLength)gridLengthConverter.ConvertFromString(rowDefinitions[i % rowDefinitions.Count]);  // use repeating pattern of supplied list
                    grid.RowDefinitions.Add(rd);
                }


            } else
            {

            }


            for (int i = 0; i < elementsToAdd.PathCount; i++)
            {
                List<IGH_Goo> branch = elementsToAdd.Branches[i];
                //for all the elements in each branch
                for (int j = 0; j < branch.Count; j++)
                {
                    UIElement_Goo u = branch[j] as UIElement_Goo;
                    //make sure it doesn't already have a parent
                    HUI_Util.removeParent(u.element);
                    FrameworkElement fe = u.element as FrameworkElement;
                    if (fe != null)
                    {
                        //set its alignment to be relative to upper left - this makes margin-based positioning easy
                        fe.HorizontalAlignment = HorizontalAlignment.Left;
                        fe.VerticalAlignment = VerticalAlignment.Top;

                        //set up row and column positioning
                        Grid.SetColumn(fe, i); 
                        Grid.SetRow(fe, j); 
                    }
                    //add it to the grid
                    grid.Children.Add(u.element);
                }

            }

          
            //pass the grid out
            DA.SetData("Simple Grid", new UIElement_Goo(grid, "Simple Grid", InstanceGuid, DA.Iteration));
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
            get { return new Guid("{d6339a12-2494-4770-bc52-1649fc8c35da}"); }
        }
    }
}