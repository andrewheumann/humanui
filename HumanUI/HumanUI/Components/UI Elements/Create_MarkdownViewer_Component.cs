using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// This file is designed as a template for components that handle creation of HUI Elements.
    /// </summary>
    public class CreateMarkdownViewer_Component : GH_Component
    {
        /// <summary>
        /// This file is designed as a template for components that handle creation of HUI Elements.
        /// </summary>
        public CreateMarkdownViewer_Component()
            : base("Create Markdown Viewer", "MDV",
                "Creates a block of formatted text based on Markdown-formatted input",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "The markdown-syntax-formatted text you'd like to render",
                GH_ParamAccess.item);
            pManager.AddTextParameter("Styles File", "SF",
                "The optional path to a styles.xaml file. Defaults will be used if not supplied", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Asset Directory","AD",
                "The optional root directory for any assets like in-line images included in your markdown", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Markdown Viewer", "MDV", "The Markdown Viewer Element.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string text = "";
            string stylePath = "";
            string assetPath = "";

            bool hasText = DA.GetData("Text", ref text);
            bool hasStylePath = DA.GetData("Styles File", ref stylePath);
            bool hasAssetPath = DA.GetData("Asset Directory", ref assetPath);
            if (!hasText) return;

            var MarkdownViewer = new MarkdownViewer(text, stylePath, assetPath);

            DA.SetData("Markdown Viewer", new UIElement_Goo(MarkdownViewer, "Markdown Viewer", InstanceGuid, DA.Iteration));

            // BUT YOU'RE NOT DONE YET!!!

            // OK, you *might* be done, if all you're creating is a static object to display. But in most cases you'll want to do a few things:

            // 1. Make sure the element plays nice with the "Value Listener" so that you can get data from user interaction with the element

            // 2. Create a companion "Set" object so you can modify its contents without re-launching the entire window. This is optional -
            //    but generally any component that displays data or information should do this. (if it just TAKES user input, you're probably fine 
            //    without this.)

            // 3. Make sure the element plays nice with Save/Restore State.

            // 4. Set behaviors for "Adjust Element Appearance" where appropriate.

            //Here's how to do those things:

            // 1. - Value Listener Integration
            // There are two key methods in the value listener that you will have to augment in order for it to be able to
            // listen for the appropriate events
            // with your element. These are ValueListener_Component.RemoveEvents(), and ValueListener_Component.AddEvents().
            // For elements where you want to return the index of some user selection in the element, you'll need to modify
            // HUI_Util.GetElementIndex() as well.


            // These look pretty similar to each other - big-ass switch statements operating on the type name. For example (AddEvents):

            //case "System.Windows.Controls.Slider":
            //       Slider s = u as Slider;
            //       s.ValueChanged -= ExpireThis;
            //       s.ValueChanged += ExpireThis;
            //       return;

            // For RemoveEvents, you'll just get rid of the line that does +=. 
            // I'm not actually totally sure that the removing before adding is totally necessary in AddEvents - my thinking
            // was that it would prevent events from getting added twice, so I've left it in. 
            // You will want to add your own new case to both of these methods, and attach the "ExpireThis" handler 
            // to whatever method is appropriate depending on your element. You'll need to cast "u" (the arbitrary ui element passed to the methods)
            // into your specific type to get access to the right events. 


            // 2. - Handle "Setting" values.
            // See the "Set_TEMPLATE_Component.cs" file in the "UI_Output folder/namespace.


            // 3. Play nice w/ Save/Restore State
            // Similar to Value Listener integration, there are a couple more methods to modify. 
            // These live in HUI_Util.cs:
            //      * GetElementValue(UIElement u)
            //      * TrySetElementValue(UIElement u, object o)

            //As w/ Value Listener, you just have to add a case for your element to the switch statements in each of these.

            // simple examples w/ textbox:
            // GetElementValue:
            //      case "System.Windows.Controls.TextBox":
            //          TextBox tb = u as TextBox;
            //          return tb.Text;
            // TrySetElementValue:
            //      case "System.Windows.Controls.TextBox":
            //          TextBox tb = u as TextBox;
            //          tb.Text = (string)o;
            //          return;

            // 4. - Adjust Element Appearance compliance
            // There are default behaviors that work in many cases, but if you want to
            // override or add behavior, just look at the method ColorTextElement in the 
            // AdjustElementAppearance_Component class - you can add another test case like
            // the one below:

            //  //Try Textblock
            //  TextBlock textblock = f as TextBlock;
            //  if (textblock != null)
            //  {
            //     textblock.Foreground = foregroundBrush;
            //     if (bgCol != System.Drawing.Color.Transparent) textblock.Background = backgroundBrush;
            //     if (fontSize > 0) textblock.FontSize = fontSize;
            //     return;
            //  }


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.MarkdownViewer;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{127E3F49-5F69-46A1-96FE-2E531EEAD975}");
    }
}