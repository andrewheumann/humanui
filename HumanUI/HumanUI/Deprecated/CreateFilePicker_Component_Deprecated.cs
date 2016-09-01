using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;

namespace HumanUI.Components
{
    public class CreateFilePicker_Component_Deprecated : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateFilePicker_Component class.
        /// </summary>
        public CreateFilePicker_Component_Deprecated()
          : base("Create File Picker", "FilePicker",
              "Create a dialog box that lets you choose a path for a file, folder, or save path.",
              "Human UI", "UI Elements")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        public override bool Obsolete => true;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Button Label Text", "BT", "The text to display on the button to the right of the text box", GH_ParamAccess.item, "Browse...");
            pManager.AddIntegerParameter("Dialog Type", "DT", "The type of dialog to display - Open, Save, or Folder", GH_ParamAccess.item, 0);
            Param_Integer typeParam = pManager[1] as Param_Integer;
            typeParam.AddNamedValue("Open File", 0);
            typeParam.AddNamedValue("Save File", 1);
            typeParam.AddNamedValue("Browse Folder", 2);
            pManager.AddBooleanParameter("Must Exist", "ME", "Set to true if the file (in an open file dialog) must exist", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Filter", "F", "The file filter(s) to use. Specify type names, separated from path filters with a | character, like \"Grasshopper Files|*.gh\" and join multiples with | characters as well.", GH_ParamAccess.item, "All Files|*.*");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("File Picker", "P", "The File Picker element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string buttonLabelText = "";
            int dialogType = -1;
            bool mustExist = true;
            string filter = "";
            DA.GetData("Button Label Text", ref buttonLabelText);
            DA.GetData("Dialog Type", ref dialogType);
            DA.GetData("Must Exist", ref mustExist);
            DA.GetData("Filter", ref filter);

            FilePicker picker = new FilePicker(buttonLabelText, (fileDialogType)dialogType, mustExist, filter);

            DA.SetData("File Picker", new UIElement_Goo(picker, "FIle Picker", InstanceGuid, DA.Iteration));

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
                return Properties.Resources.FilePicker;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ea5d59c3-bb17-49e5-9967-1f46b21e3e51}"); }
        }
    }
}