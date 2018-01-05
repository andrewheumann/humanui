using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows;
using HumanUIBaseApp;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using De.TorstenMandelkow.MetroChart;


namespace HumanUI.Components.UI_Main
{
    /// <summary>
    /// Adjust the color and appearance of individual elements
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class AdjustElementAppearance_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AdjustElementAppearance_Component class.
        /// </summary>
        public AdjustElementAppearance_Component()
            : base("Adjust Element Appearance", "AdjustElem",
                "Adjust the color and appearance of individual elements.",
                "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements to Adjust", "E", "The elements to adjust", GH_ParamAccess.item);
            pManager.AddColourParameter("Foreground", "FC", "The foreground color of the element", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddColourParameter("Background", "BC", "The background color of the element", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Font Size", "S", "The font size of the element", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }



        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            System.Drawing.Color? fgCol = null;
            System.Drawing.Color? bgCol = null;
            double fontSize = -1;
            object elem = null;

            if (!DA.GetData("Elements to Adjust", ref elem)) return;

            var hasfgCol = DA.GetData("Foreground", ref fgCol);
            var hasbgCol = DA.GetData("Background", ref bgCol);
            var hasFontSize = DA.GetData("Font Size", ref fontSize);


            //Get the "FrameworkElement" (basic UI element) from an object
            FrameworkElement f = HUI_Util.GetUIElement<FrameworkElement>(elem);
            Selector selector = f as Selector;
            ScrollViewer sv = f as ScrollViewer;
            ChartBase cb = f as ChartBase;


            if (f is Expander exp)
            {
                if (hasfgCol) exp.Foreground = new SolidColorBrush(HUI_Util.ToMediaColor(fgCol.Value));
                if (hasbgCol) exp.Background = new SolidColorBrush(HUI_Util.ToMediaColor(bgCol.Value));
                if (!hasFontSize) return;
                var header = exp.Header;
                TextBlock myHeader = null;
                if (header is string headString)
                {
                    myHeader = new TextBlock();
                    myHeader.Text = headString;

                }
                else
                {
                    myHeader = exp.Header as TextBlock;

                }
                myHeader.FontSize = fontSize;
                exp.Header = myHeader;
                return;
            }


            // var ChartElem = HUI_Util.GetUIElement<ChartBase>(ChartObject);
            if (f is Grid g)
            {
                foreach (UIElement child in g.Children)
                {
                    ColorTextElement(child, fgCol, bgCol, fontSize);
                }
                if (hasbgCol) g.Background = new SolidColorBrush(HUI_Util.ToMediaColor(bgCol.Value));
            }

            //if it's a panel color its children
            if (f is Panel panel)
            {
                foreach (UIElement child in panel.Children)
                {
                    ColorTextElement(child, fgCol, bgCol, fontSize);
                }
                if (hasbgCol) panel.Background = new SolidColorBrush(HUI_Util.ToMediaColor(bgCol.Value));
            }

            //if it's a selector, color its items
            else if (selector != null)
            {
                foreach (UIElement child in selector.Items)
                {
                    ColorTextElement(child, fgCol, bgCol, fontSize);
                }
            }

            //if it's an itemscontrol, color its items
            else if (sv != null)
            {
                if (sv.Content is ItemsControl ic)
                {
                    foreach (var item in ic.Items)
                    {
                        if (item is UIElement uie)
                        {
                            ColorTextElement(uie, fgCol, bgCol, fontSize);
                        }

                    }
                }

            }

            //otherwise assume it's just a root level element
            else
            {
                ColorTextElement(f, fgCol, bgCol, fontSize);
            }



        }

        //color a UIElement. Runs through a list of possible types it recognizes and tries to color/format appropriately. 
        private static void ColorTextElement(UIElement f, System.Drawing.Color? fgCol, System.Drawing.Color? bgCol, double fontSize)
        {
            // create brushes
            Brush backgroundBrush = new SolidColorBrush();
            Brush foregroundBrush = new SolidColorBrush();

            // if a value exists, create a brush for it
            if (bgCol.HasValue)
            {
                backgroundBrush = new SolidColorBrush(HUI_Util.ToMediaColor(bgCol.Value));
            }
            if (fgCol.HasValue)
            {
                foregroundBrush = new SolidColorBrush(HUI_Util.ToMediaColor(fgCol.Value));
            }

            // Apply brushes where available
            //

            //Try graph
            if (f is ChartBase ChartB)
            {
                if (fgCol.HasValue) ChartB.Foreground = foregroundBrush;
                if (bgCol.HasValue) ChartB.Background = backgroundBrush;
                if (fontSize > 0) ChartB.FontSize = fontSize;
            }
            //Try Label
            if (f is Label l)
            {
                if (fgCol.HasValue) l.Foreground = foregroundBrush;
                if (bgCol.HasValue) l.Background = backgroundBrush;
                if (fontSize > 0) l.FontSize = fontSize;
                return;
            }
            // Try textbox
            if (f is TextBox tb)
            {
                if (fgCol.HasValue) tb.Foreground = foregroundBrush;
                if (bgCol.HasValue) tb.Background = backgroundBrush;
                if (fontSize > 0) tb.FontSize = fontSize;
                return;
            }
            //Try Textblock

            if (f is TextBlock textblock)
            {
                if (fgCol.HasValue) textblock.Foreground = foregroundBrush;
                if (bgCol.HasValue) textblock.Background = backgroundBrush;
                if (fontSize > 0) textblock.FontSize = fontSize;
                return;
            }
            //Try Button

            if (f is Button b)
            {
                if (fgCol.HasValue) b.Foreground = foregroundBrush;
                if (bgCol.HasValue) b.Background = backgroundBrush;
                if (fontSize > 0) b.FontSize = fontSize;
                return;
            }
            //Try Checkbox
            if (f is CheckBox cb)
            {
                if (fgCol.HasValue) cb.Foreground = foregroundBrush;
                if (bgCol.HasValue) cb.Background = backgroundBrush;
                if (fontSize > 0) cb.FontSize = fontSize;
                return;

            }
            //Try RadioButton
            if (f is RadioButton rb)
            {
                if (fgCol.HasValue) rb.Foreground = foregroundBrush;
                if (bgCol.HasValue) rb.Background = backgroundBrush;
                if (fontSize > 0) rb.FontSize = fontSize;
                return;
            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AdjustElementAppearance;
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("76eb5930-7b2b-4a11-839e-d3c00990af8b"); }
        }
    }
}