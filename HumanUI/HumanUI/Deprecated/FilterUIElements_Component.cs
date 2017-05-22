using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI
{
    /// <exclude />
    public class FilterUIElements_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FilterUIElements_Component class.
        /// </summary>
        public FilterUIElements_Component()
            : base("Filter UI Elements", "Filter",
                "This component allows you to select UI elements from a window by name, to let you listen for their values or set their properties dynamically.",
                "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "All Controls and other elements belonging to the window", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The filter(s) for the elements you want to listen for.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Filtered Elements", "FE", "The selected UI elements.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> filters = new List<string>();

            if (!DA.GetDataList<KeyValuePair<string, UIElement_Goo>>("Elements", allElements)) return;
            if (!DA.GetDataList<string>("Name Filter(s)", filters)) return;

            //create a dictionary
            Dictionary<string, UIElement_Goo> elementDict = allElements.ToDictionary(pair => pair.Key, pair => pair.Value);
            Dictionary<string, UIElement_Goo> results = new Dictionary<string, UIElement_Goo>();
            foreach (string fil in filters)
            {
                results.Add(fil,elementDict[fil]);
            }
            DA.SetDataList("Filtered Elements", results);


        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.FilterElements;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{fe29a68f-5c93-48b0-9558-c0c75c53172f}");
    }
}