using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using HumanUI.Properties;
using Rhino.Geometry;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;

namespace HumanUI.Components.UI_Elements
{
    public class CreateSeparator_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateSeparator_Component class.
        /// </summary>
        public CreateSeparator_Component()
          : base("Create Separator", "Separator",
              "Create a line separator object.",
              "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Thickness", "T", "The thickness of the separator.", GH_ParamAccess.item, 0.5);
            pManager.AddBooleanParameter("Horizontal", "H", "Separator is horizontal.", GH_ParamAccess.item, true);
            pManager.AddColourParameter("Color", "C", "The color of the separator (optional).", GH_ParamAccess.item, System.Drawing.Color.Black);
            pManager.AddNumberParameter("Width", "W", "The width of the separator (optional). Default is Stretch.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "The height of the separator (optional). Default is Stretch.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Separator", "S", "The created Separator", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double thickness = 0.5;
            var horizontal = true;
            var color = new GH_Colour();
            double width = 0;
            double height = 0;
            if (!DA.GetData<double>("Thickness", ref thickness)) return;
            if (!DA.GetData<bool>("Horizontal", ref horizontal)) return;

            DA.GetData<GH_Colour>("Color", ref color);
            DA.GetData<double>("Width", ref width);
            DA.GetData<double>("Height", ref height);

            
            var rect = new Rectangle();

            SetupSeparator(thickness, horizontal, ToMediaColor(color.Value), width, height, rect);

            // pass out the separator
            DA.SetData("Separator", new UIElement_Goo(rect, "Separator", InstanceGuid, DA.Iteration));

        }

        public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        protected static void SetupSeparator(double thickness, bool horizontal, System.Windows.Media.Color color, double width, double height, Rectangle rect)
        {
            var brush = new System.Windows.Media.SolidColorBrush(color);

            rect.Fill = brush;
            rect.HorizontalAlignment = HorizontalAlignment.Stretch;
            rect.VerticalAlignment = VerticalAlignment.Stretch;


            if (horizontal)
            {
                rect.HorizontalAlignment = HorizontalAlignment.Stretch;
                rect.VerticalAlignment = VerticalAlignment.Center;
                rect.Height = thickness;

                if (height > 0) rect.Margin = new Thickness(0, height/2, 0, height/2);
            }
            else
            {
                rect.VerticalAlignment = VerticalAlignment.Stretch;
                rect.HorizontalAlignment = HorizontalAlignment.Center;
                rect.Width = thickness;

                if (width > 0) rect.Margin = new Thickness(width / 2, 0, width / 2, 0);
            }

            if (width > 0 && horizontal)
            {
                rect.Width = width;
                rect.HorizontalAlignment = HorizontalAlignment.Center;
            }

            if (height > 0 && !horizontal)
            {
                rect.Height = height;
                rect.VerticalAlignment= VerticalAlignment.Center;
            }


        }
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.CreateSeparator;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a7a0c814-ab68-45e0-9a9e-5515b0c4adc6");
    }
}