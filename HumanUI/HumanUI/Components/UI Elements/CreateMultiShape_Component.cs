using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using System.Windows.Shapes;
using Grasshopper.Kernel.Types;
using System.Windows.Forms;

namespace HumanUI.Components.UI_Elements
{



    /// <summary>
    /// Component to create composite graphics. Unlike "Create Shape" - which takes in multiple curves to represent a single shape (with holes etc)
    /// this component allows for multiple shapes all arranged relative to one another, with different appearance properties.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateMultiShape_Component : GH_Component
    {

        /// <summary>
        /// Initializes a new instance of the CreateMultiShape_Component class.
        /// </summary>
        public CreateMultiShape_Component()
            : base("Create Shapes", "Shapes",
                "Creates shapes from a polylines",
                "Human UI", "UI Elements")
        {
            clickMode = ClickableShapeGrid.ClickMode.None;
            
        }


        internal ClickableShapeGrid.ClickMode clickMode;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Shapes", "S", "The shapes to add as Polyline(s)", GH_ParamAccess.list);
            pManager.AddColourParameter("Fill Color", "FC", "The fill colors. Leave empty for no fill", GH_ParamAccess.list);
            pManager.AddNumberParameter("Stroke Weight", "SW", "The stroke weights. Leave empty or set to 0 for no stroke.", GH_ParamAccess.list);
            pManager.AddColourParameter("Stroke Color", "SC", "The stroke colors", GH_ParamAccess.list, System.Drawing.Color.Black);
            pManager.AddNumberParameter("Scale", "Scl", "Use this value to resize the shape.", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Width", "W", "The width of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Height", "H", "The height of the container for the shape. \nLeave blank to autosize.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape", "S", "The created shape.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            List<Curve> shapeCrvs = new List<Curve>();
            List<System.Drawing.Color> fillCol = new List<System.Drawing.Color>();
            List<double> strokeWeights = new List<double>();
            List<System.Drawing.Color> strokeCol = new List<System.Drawing.Color>();
            double scale = 1.0;
            int width = 0;
            int height = 0;

            bool hasFill = DA.GetDataList<System.Drawing.Color>("Fill Color", fillCol);
            bool hasWeight = DA.GetDataList<double>("Stroke Weight", strokeWeights);
            bool hasStrokeCol = DA.GetDataList<System.Drawing.Color>("Stroke Color", strokeCol);
            DA.GetData<double>("Scale", ref scale);
            if (!DA.GetDataList<Curve>("Shapes", shapeCrvs)) return;

            //Set up a grid to contain all shapes
            ClickableShapeGrid G = new ClickableShapeGrid();
            G.clickMode = clickMode;
            if (DA.GetData<int>("Width", ref width))
            {

                G.Width = width;
            }
            if (DA.GetData<int>("Height", ref height))
            {

                G.Height = height;
            }


            int i = 0;

            //For all the curves the user inputs
            foreach (Curve c in shapeCrvs)
            {


                //Set up a new path object
                Path path = new Path();
                //create a list to contain the single curve
                List<Curve> crvList = new List<Curve>();
                crvList.Add(c);
                //Set the path's data to the curve
                path.Data = CreateShape_Component.pathGeomFromCrvs(crvList, scale, false);
                //set all the properties
                if (hasFill)
                {

                    path.Fill = new SolidColorBrush(HUI_Util.ToMediaColor(fillCol[i % fillCol.Count]));
                }

                if (hasWeight)
                {

                    path.StrokeThickness = strokeWeights[i % strokeWeights.Count];
                }
                if (hasStrokeCol)
                {

                    path.Stroke = new SolidColorBrush(HUI_Util.ToMediaColor(strokeCol[i % strokeCol.Count]));
                }

                i++;
                //add the path object to the grid
                G.Children.Add(path);
            }

            //Pass out the grid
            DA.SetData("Shape", new UIElement_Goo(G, "Shape", InstanceGuid, DA.Iteration));

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
                return Properties.Resources.CreateShapes;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{94288CED-76F6-438A-9216-E60C8290F640}"); }
        }


        /// <summary>
        /// Updates the component message based on the click mode.
        /// </summary>
        void UpdateMessage()
        {
            switch (clickMode)
            {
                case ClickableShapeGrid.ClickMode.ButtonMode:
                    Message = "Button Mode";
                    return;
                case ClickableShapeGrid.ClickMode.None:
                    Message = "";
                    return;
                case ClickableShapeGrid.ClickMode.ToggleMode:
                    Message = "Toggle Mode";
                    return;
                case ClickableShapeGrid.ClickMode.PickerMode:
                    Message = "Picker Mode";
                    return;
            }


        }


        /// <summary>
        /// Write all required data for deserialization to an IO archive.
        /// </summary>
        /// <param name="writer">Object to write with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("ClickMode", (int)clickMode);
            return base.Write(writer);
        }

        /// <summary>
        /// Read all required data for deserialization from an IO archive.
        /// </summary>
        /// <param name="reader">Object to read with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int clickVal = -1;
            if (reader.TryGetInt32("ClickMode", ref clickVal))
            {
                clickMode = (ClickableShapeGrid.ClickMode)clickVal;
            }
            UpdateMessage();
            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            GH_DocumentObject.Menu_AppendItem(menu, "Click Action:");
            GH_DocumentObject.Menu_AppendItem(menu, "None", NoneClicked, true, clickMode == ClickableShapeGrid.ClickMode.None);
            GH_DocumentObject.Menu_AppendItem(menu, "Button Mode", ButtonModeClicked, true, clickMode == ClickableShapeGrid.ClickMode.ButtonMode);
            GH_DocumentObject.Menu_AppendItem(menu, "Toggle Mode", ToggleModeClicked, true, clickMode == ClickableShapeGrid.ClickMode.ToggleMode);
            GH_DocumentObject.Menu_AppendItem(menu, "Picker Mode", PickerModeClicked, true, clickMode == ClickableShapeGrid.ClickMode.PickerMode);
            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void PickerModeClicked(object sender, EventArgs e)
        {
            clickMode = ClickableShapeGrid.ClickMode.PickerMode;
            UpdateMessage();
            ExpireSolution(true);
        }

        private void ToggleModeClicked(object sender, EventArgs e)
        {
            clickMode = ClickableShapeGrid.ClickMode.ToggleMode;
            UpdateMessage();
            ExpireSolution(true);
        }

        private void ButtonModeClicked(object sender, EventArgs e)
        {
            clickMode = ClickableShapeGrid.ClickMode.ButtonMode;
            UpdateMessage();
            ExpireSolution(true);
        }

        private void NoneClicked(object sender, EventArgs e)
        {
            clickMode = ClickableShapeGrid.ClickMode.None;
            UpdateMessage();
            ExpireSolution(true);
        }


    }
}