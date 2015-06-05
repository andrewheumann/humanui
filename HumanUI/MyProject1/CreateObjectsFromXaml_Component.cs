using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Xaml;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;

namespace HumanUI
{
    public class CreateObjectsFromXaml_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateObjectsFromXaml_Component class.
        /// </summary>
        public CreateObjectsFromXaml_Component()
            : base("Create Objects from XAML", "XAML",
                "Creates UI elements from typed XAML syntax",
                "Human", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("XAML", "X", "The XAML text", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "The XAML object tree", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string xaml= "";
            if (!DA.GetData<string>("XAML", ref xaml)) return;

            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            object xamlObj = System.Windows.Markup.XamlReader.Parse(xaml,parserContext); //XamlServices.Parse(xaml);
            UIElement uie = xamlObj as UIElement;
            if (uie == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "unable to convert xaml into a UI element");
            }
            DA.SetData("Object", new UIElement_Goo(uie, "Generic XAML", InstanceGuid, DA.Iteration));
      
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
            get { return new Guid("{fd2eb7a5-9db0-4688-ad19-3736eb4fb182}"); }
        }
    }
}