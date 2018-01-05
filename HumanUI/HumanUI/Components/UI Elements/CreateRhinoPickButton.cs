using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_IO.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HumanUIBaseApp;
using Grasshopper.Kernel.Parameters;
using Rhino.DocObjects;

namespace HumanUI.Components.UI_Elements
{




    /// <summary>
    /// This class extends the CreateButton component to add an extra event handler and an input for a Command Line script to be executed on button press. 
    /// </summary>
    /// <seealso cref="HumanUI.Components.UI_Elements.CreateButton_Component" />
    public class CreateRhinoPickButton_Component : CreateButton_Component
    {




        /// <summary>
        /// Initializes a new instance of the CreateRhinoPickButton_Component class.
        /// </summary>
        public CreateRhinoPickButton_Component()
            : base("Create Rhino Pick Button", "PickBtn",
                "Create a special Button object to pick geometry from Rhino.")
        {

        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Button Name", "N", "The text to display on the button", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Image Path", "I", "The image to display on the button.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Prompt", "P", "The prompt to display to the user", GH_ParamAccess.item,
                "Please select geometry...");
            pManager.AddBooleanParameter("Allow Multiple", "AM", "Set to true to allow selecting multiple items",
                GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Allow None", "AN", "Set to true to allow the selection of nothing",
                GH_ParamAccess.item, true);
            var tp = pManager.AddIntegerParameter("Type", "T", "The allowed geometry types", GH_ParamAccess.item, 0);
            var tparam = pManager[tp] as Param_Integer;
            tparam.AddNamedValue("Any", 0);
            tparam.AddNamedValue("Brep", 1);
            tparam.AddNamedValue("Curve", 2);
            tparam.AddNamedValue("Mesh", 3);
            tparam.AddNamedValue("Point", 4);
        }


        private static ObjectType[] otMap = new ObjectType[]
  {
            ObjectType.AnyObject,
            ObjectType.Brep | ObjectType.Surface | ObjectType.Extrusion,
            ObjectType.Curve,
            ObjectType.Mesh,
            ObjectType.Point

  };

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Button";
            string imagePath = "";
            string prompt = "";
            bool allowMultiple = true;
            bool allowNone = true;
            //bool clear = false;
            int filterInt = 0;

            bool hasText = DA.GetData<string>("Button Name", ref name);
            bool hasIcon = DA.GetData<string>("Image Path", ref imagePath);
            if (!DA.GetData("Prompt", ref prompt)) return;
            if (!DA.GetData("Allow Multiple", ref allowMultiple)) return;
            if (!DA.GetData("Allow None", ref allowNone)) return;
           // if (!DA.GetData("Clear", ref clear)) return;
            if (!DA.GetData("Type", ref filterInt)) return;
            var filter = otMap[filterInt];



            if (!hasText && !hasIcon) return;
            HUI_RhPickButton btn = new HUI_RhPickButton(prompt, allowMultiple, allowNone, filter);
            SetupButton(name, imagePath, hasText, hasIcon, btn, bs);

            DA.SetData("Button", new UIElement_Goo(btn, name, InstanceGuid, DA.Iteration));

        }



        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateRhinoButton;



        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{81A69431-0AEE-4B30-BB1B-131A8EEB2B7F}");
    }
}