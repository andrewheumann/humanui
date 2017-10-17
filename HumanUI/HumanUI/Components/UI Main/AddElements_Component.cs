using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using HumanUIBaseApp;
using System.Windows.Forms;
using GH_IO.Serialization;

namespace HumanUI.Components.UI_Main
{
    /// <summary>
    /// Component to add elements to a HUI window
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class AddElements_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddElements_Component class.
        /// </summary>
        public AddElements_Component()
            : base("Add Elements", "AddElems",
                "Add WPF Controls to a window",
                "Human UI", "UI Main")
        {
            DoVLChecking = true;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Window", "W", "The window to which to add the elements", GH_ParamAccess.item);
            pManager.AddGenericParameter("Elements", "E", "The Controls and other elements you want to add to the window", GH_ParamAccess.list);
            pManager[1].DataMapping = GH_DataMapping.Flatten; // doesn't make sense to pass trees of elements to one window

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Added Elements", "E", "The elements added.", GH_ParamAccess.list);
            pManager.AddTextParameter("Element Names", "N", "The names of the added elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //set up the variables for GetData
            MainWindow mw = null;
            List<UIElement_Goo> elementsToAdd = new List<UIElement_Goo>();

            Dictionary<string, UIElement_Goo> resultDict = new Dictionary<string, UIElement_Goo>();

            //Get data from component
            if (!DA.GetData<MainWindow>("Window", ref mw)) return;
            if (!DA.GetDataList<UIElement_Goo>("Elements", elementsToAdd)) return;

            //clear out any old elements
            mw.clearElements();

            if (DoVLChecking)
            {
                //check for any value listener connected to the same window
                var ActiveObjects = OnPingDocument().ActiveObjects();
                var AllSources = HUI_Util.SourcesRecursive(Params.Input[1], ActiveObjects);
                var ValueListenerSources = AllSources.OfType<ValueListener_Component>();
                if (ValueListenerSources.Count() != 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, VALUE_LISTENER_WARNING);

                }

            }
            //for all the elements, remove its parent, add it to the window, and add it to our tracking dictionary
            foreach (UIElement_Goo u in elementsToAdd)
            {
                if (u == null) continue;
                HUI_Util.removeParent(u.element);
                mw.AddElement(u.element);
                HUI_Util.AddToDict(u, resultDict);
            }

            //Pass out the added elements and their names. This is no longer so necessary now that listeners attach to elements directly. 
            DA.SetDataList("Added Elements", resultDict);
            DA.SetDataList("Element Names", resultDict.Keys);
        }





        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AddElements;


        bool DoVLChecking = true;

        void updateMessage()
        {
            Message = DoVLChecking ? "" : "Fast Mode";

        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            GH_DocumentObject.Menu_AppendItem(menu, "Disable Flow Loop Checking (Fast Mode)", FastModeClicked, true, !DoVLChecking);
        }

        private void FastModeClicked(object sender, EventArgs e)
        {
            DoVLChecking = !DoVLChecking;
            updateMessage();
            ExpireSolution(true);

        }


        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("DoVLChecking", DoVLChecking);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool check = true;
            reader.TryGetBoolean("DoVLChecking", ref check);
            DoVLChecking = check;
            updateMessage();
            ExpireSolution(true);
            return base.Read(reader);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{73b5e187-b35d-45bd-8495-9e06b429bc07}");

        public string VALUE_LISTENER_WARNING =
            "In general, it's not a good idea to drive the creation of \n" +
            "elements with the results of a value listener. Instead, use \n" +
            "the value listener to drive a \"Set\" operation, from the \n" +
            "UI Output tab, to update the contents of an existing element \n" +
            "in the window. Otherwise, every time something in the window \n" +
            "changes, triggering your value listener, the entire window needs \n" +
            "to be re-generated. This can create the appearance of a \"freezing\"\n" +
            "behavior. See the example files for more information on the appropriate\n" +
            "configuration of \"Create\" and \"Set\" components in Human UI.";
    }
}