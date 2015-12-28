using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// This file is designed as a template for components that handle the modification/updating of existing HUI Elements.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class Set_TEMPLATE_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Set_TEMPLATE_Component class.
        /// </summary>
        public Set_TEMPLATE_Component()
            : base("Set SOME_UI_ELEMENT", "Set SOME_UI_ELEMENT",
                "Description",
                "Human", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //The first input should be the UI element itself -as direct output from
            // the corresponding "Create" Component
            pManager.AddGenericParameter("SOME_UI_ELEMENT to modify", "U", "The SOME_UI_ELEMENT object to modify", GH_ParamAccess.item);

            //Add any additional variables necessary to modify the content of the element.
            //These usually look an awful lot like the Input Params of the "Create" component.
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //"Set" components usually do not have any outputs.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Create a basic object to hold the UI Element prior to type conversion
            object myUIElement = null;
            //Create any other placeholder variables for the various properties you're setting.

            //if you're not getting the objects, return.

            if (!DA.GetData<object>(0, ref myUIElement)) return;

            // use DA.GetData to populate any other placeholder variables. You may 
            // want to store a boolean for any optional variables. 
            // (bool didUserSpecifyHeight = DA.GetData<int>("Height",ref height); for example.)

            // Once you've got your generic object, you'll need to cast it to the type
            // of the UI Element you're passing around. that will look like this:
            // SOME_UI_ELEMENT element = HUI_Util.GetUIElement<SOME_UI_ELEMENT>(myUIElement);
            // with "SOME_UI_ELEMENT" as the type of UI element you're modifying.

            // Then modify the element itself with all of your variables. As with the Create component, it's
            // likely that you'll want to refer to the stored booleans from optional inputs to decide whether or
            // not to make a change.


            //That's all there is to it!

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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            //Remember to update this guid with one of your own if you're working from the template. 
            get { return new Guid("{e5d09dcf-6ef3-40e1-b7de-371b40fdcc20}"); }
        }
    }
}