using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;

namespace HumanUI.Components.UI_Containers
{
    public class CreateSimpleGrid_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateSimpleGrid_Component class.
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
            pManager.AddIntegerParameter("Grid Membership", "M", "List of index numbers to place elements in different Grids. List length must match the number of Element paths.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Width", "W", "The width of the grid", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Height", "H", "The height of the grid", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Row Definitions", "RD", "An optional repeating pattern of Row Heights - use 'Auto' to inherit, numbers for absolute sizes, and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list, "Auto");
            pManager[4].Optional = true;
            pManager.AddTextParameter("Column Definitions", "CD", "An optional flat list of Column Widths - use 'Auto' to inherit, use numbers for absolute sizes, and numbers with * for ratios (like 1* and 2* for a 1/3 2/3 split)", GH_ParamAccess.list, "Auto");
            pManager[5].Optional = true;


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
            List<int> memberships = new List<int>();


            if (!DA.GetDataTree<IGH_Goo>(0, out elementsToAdd)) return;
            bool hasMemberships = DA.GetDataList<int>("Grid Membership", memberships);
            bool hasWidth = DA.GetData<double>("Width", ref width);
            bool hasHeight = DA.GetData<double>("Height", ref height);

            bool hasRowDefs = DA.GetDataList<string>("Row Definitions", rowDefinitions);
            bool hasColDefs = DA.GetDataList<string>("Column Definitions", colDefinitions);

            if (hasMemberships)
            {
                if (memberships.Count != elementsToAdd.Branches.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Grid Membership list length must equal the number of Elements branches.");
                    return;
                }
            }
            else
            {
                // Create a default list of a single branch grouping to apply to all trees
                memberships = new List<int>() { 0 };
            }

            int[] defaultMembership = { 0 };

            // Create an array of the membership values
            int[] membershipArray = hasMemberships ? memberships.ToArray() : defaultMembership;

            // Create empty list of branch metadata and populate with values
            List<BranchMetadata> branchMetaData = new List<BranchMetadata>();

            for (int i = 0; i < elementsToAdd.Branches.Count(); i++)
            {
                int myMembership = hasMemberships ? membershipArray[i] : 0;

                BranchMetadata bm = new BranchMetadata(i, myMembership);
                branchMetaData.Add(bm);
            }

            // Sort and group the metadata
            var branchGroupings = branchMetaData
                .OrderBy(b => b.membershipIndex)
                .GroupBy(b => b.membershipIndex);


            // create an empty List of grids and populate 

            List<Grid> grids = new List<Grid>();

            foreach (var group in branchGroupings)
            {

                //initialize a grid
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

                // Add grid to List
                grids.Add(grid);

            }

            // Populate the Grids (should this be done before adding the grids to the List?)

            foreach (var group in branchGroupings)
            {
                // Count the number of branches in this array
                int currentBranchCount = group.Count();

                //set up a "GridLengthConverter" to handle parsing our strings.
                GridLengthConverter gridLengthConverter = new GridLengthConverter();

                //set up rows and columns if present
                if (hasColDefs)
                {
                    for (int i = 0; i < currentBranchCount; i++)
                    {
                        ColumnDefinition cd = new ColumnDefinition();
                        cd.Width = (GridLength)gridLengthConverter.ConvertFromString(colDefinitions[i % colDefinitions.Count]);  // use repeating pattern of supplied list
                        // Note: group.Key is the index of the group/grid
                        grids[group.Key].ColumnDefinitions.Add(cd);
                    }

                }

                if (hasRowDefs)
                {
                    int maxCount = 0;

                    // Find the count of the longest list
                    foreach (BranchMetadata md in group)
                    {
                        // get the count of data from the branch
                        var myCount = elementsToAdd.get_Branch(elementsToAdd.get_Path(md.branchIndex)).Count;

                        if (myCount > maxCount)
                        {
                            maxCount = myCount;
                        }
                    }

                    // Build up the row heights based on a repeating pattern
                    for (int i = 0; i < maxCount; i++)
                    {
                        RowDefinition rd = new RowDefinition();
                        rd.Height = (GridLength)gridLengthConverter.ConvertFromString(rowDefinitions[i % rowDefinitions.Count]);  // use repeating pattern of supplied list
                        // Note: group.Key is the index of the group/grid
                        grids[group.Key].RowDefinitions.Add(rd);
                    }


                }

                // Set up a counter for iterating through the appropriate number of columns
                int currentColumn = 0;

                // Populate the Grids with Elements
                foreach (BranchMetadata md in group)
                {
                    // Get each branch referenced in the metadata
                    var branch = elementsToAdd.get_Branch(elementsToAdd.get_Path(md.branchIndex));


                    //for all the elements in each branch
                    for (int j = 0; j < branch.Count; j++)
                    {
                        UIElement_Goo u = branch[j] as UIElement_Goo;
                        //make sure it doesn't already have a parent
                        HUI_Util.removeParent(u.element);
                        FrameworkElement fe = u.element as FrameworkElement;
                        if (fe != null)
                        {
                            // set its alignment to stretch
                            // this will allow elements like sliders, pulldowns, and separators to fill the cell
                            fe.HorizontalAlignment = HorizontalAlignment.Stretch;
                            fe.VerticalAlignment = VerticalAlignment.Stretch;

                            //set up row and column positioning
                            Grid.SetColumn(fe, currentColumn);
                            Grid.SetRow(fe, j);
                        }

                        //add it to the grid
                        grids[group.Key].Children.Add(u.element);

                    }

                    // Increment the column index
                    currentColumn++;
                }
            }

            // Create the list of Elements and add each grid
            List<UIElement_Goo> output = new List<UIElement_Goo>();
            foreach (Grid g in grids)
            {
                output.Add(new UIElement_Goo(g, "Simple Grid", InstanceGuid, DA.Iteration));
            }

            //pass the grids out
            DA.SetDataList("Simple Grid", output);
        }

        // Branch metadata class to allow for easy sorting/grouping via Linq
        public class BranchMetadata
        {
            public int branchIndex;
            public int membershipIndex;

            public BranchMetadata(int branchIndex, int membershipIndex)
            {
                this.branchIndex = branchIndex;
                this.membershipIndex = membershipIndex;

            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.simpleGrid;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4df77b45-0d74-44ea-9445-6d5d8b1d17ad"); }
        }
    }
}