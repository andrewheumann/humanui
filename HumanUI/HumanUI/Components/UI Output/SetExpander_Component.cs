using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Grasshopper.Kernel;

namespace HumanUI.Components.UI_Output
{
    public class SetExpander_Component : GH_Component
    {

        public SetExpander_Component() : base("Set Expander","SetExp","Sets the properties of an expander container","Human UI","UI Output")
        {
            
        }

        public override Guid ComponentGuid => new Guid("{8A270D94-FBD4-49F0-8CFE-5FB43CE85BEE}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Expander", "E", "The expander to modify", GH_ParamAccess.item);
            pManager[pManager.AddTextParameter("Name", "N", "The new name of the expander", GH_ParamAccess.item)].Optional = true;
            pManager[pManager.AddBooleanParameter("Expanded Open", "O", "Set to true to expand or false to collapse",
                GH_ParamAccess.item)].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
      
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object expanderElement = null;
            string newName = "";
            bool expanded = false;

            if (!DA.GetData("Expander", ref expanderElement)) return;
            bool hasName = DA.GetData("Name", ref newName);
            bool hasExpanded = DA.GetData("Expanded Open", ref expanded);

            Expander e = HUI_Util.GetUIElement<Expander>(expanderElement);
            if (hasName) e.Header = newName;
            if (hasExpanded) e.IsExpanded = expanded;


        }

        protected override Bitmap Icon => Properties.Resources.setExpander;
    }
}
