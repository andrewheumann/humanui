using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;

namespace HumanUI.Components
{
    public class GetElementProperties_Component : GH_Component
    {
        public GetElementProperties_Component() : base("Get Element Properties", "GetProps", "Tries to get all properties of any element. This is experimental!", "Human UI", "UI Main")
        {

        }

        public override Guid ComponentGuid => new Guid("{097D774D-A647-4DEA-8333-63E8F662A7EA}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Element", "E", "The element to get properties for", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Property Names", "N", "The names of the properties", GH_ParamAccess.list);
            pManager.AddTextParameter("Property Types", "T", "The types of the properties", GH_ParamAccess.list);
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object elem = null;



            if (!DA.GetData("UI Element", ref elem)) return;

            object unwrappedElem = HUI_Util.GetUIElement<UIElement>(elem);
            

            var type = unwrappedElem.GetType();
            var props = type.GetProperties();
            DA.SetDataList("Property Names", props.Select(p => p.Name));
            DA.SetDataList("Property Types", props.Select(p => p.PropertyType.ToString()));

        }

        protected override Bitmap Icon => Properties.Resources.GetProps;
    }
}
