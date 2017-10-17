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
    public class SetElementProperty_Component : GH_Component
    {
        public SetElementProperty_Component() : base("Set Element Property", "SetProp", "Tries to set any property of an element. This is experimental!", "Human UI", "UI Main")
        {

        }

        public override Guid ComponentGuid => new Guid("{CF439FD5-6CC5-4DB6-A265-9B0E5AED2989}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UI Element", "E", "The element to set", GH_ParamAccess.item);
            pManager.AddTextParameter("Property Name", "P", "The name of the property to set", GH_ParamAccess.item);
            pManager.AddGenericParameter("Value to set", "V", "The value or object to set the property to", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object elem = null;
            string propName = "";
            object val = null;



            if (!DA.GetData("UI Element", ref elem)) return;
            if (!DA.GetData("Property Name", ref propName)) return;
            if (!DA.GetData("Value to set", ref val)) return;

            object unwrappedElem = HUI_Util.GetUIElement<UIElement>(elem);

            if (val is GH_ObjectWrapper)
            {
                val = (val as GH_ObjectWrapper).Value;
            }
            else if (val is IGH_Goo)
            {
                try
                {
                    val = val.GetType().GetProperty("Value").GetValue(val);

                }
                catch
                {

                }
            }

            var type = unwrappedElem.GetType();
            var prop = type.GetProperty(propName);
            if (prop == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Object type {type} does not contain a property called {propName}");
                return;
            }
            try
            {
                prop.SetValue(unwrappedElem, val);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Something went wrong setting the property:\n{e.Message}");
            }
        }

        protected override Bitmap Icon => Properties.Resources.SetProp;
    }
}
