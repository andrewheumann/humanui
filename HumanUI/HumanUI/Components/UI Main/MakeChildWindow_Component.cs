using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using HumanUIBaseApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace HumanUI.Components.UI_Main
{
    public class MakeChildWindow_Component : GH_Component
    {
        public MakeChildWindow_Component() : base("Make Child Window","ChildWin","Make one window a child of another","Human UI","UI Main")
        {

        }

        public override Guid ComponentGuid => new Guid("{A067CEBE-045C-4F4A-8D77-CD4FB0075A5A}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Parent Window", "P", "The parent window", GH_ParamAccess.item);
            pManager.AddGenericParameter("Child Window", "C", "The child window", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Default Option", "D", "Use this if you want to reset the child window to one of the default modes and break its relationship with the parent.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            var defaultOptionParam = pManager[2] as Param_Integer;
            defaultOptionParam.AddNamedValue("Child of GH", 0);
            defaultOptionParam.AddNamedValue("Child of Rhino", 1);
            defaultOptionParam.AddNamedValue("Always on Top", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MainWindow parent = null;
            MainWindow child = null;
            int defaultOption = -1;

            if (!DA.GetData("Parent Window", ref parent)) return;
            if (!DA.GetData("Child Window", ref child)) return;
            bool hasDefaultOption = DA.GetData("Default Option", ref defaultOption);

            if (hasDefaultOption)
            {
                if(!Enum.IsDefined(typeof(childStatus),defaultOption))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid value for default option. Nothing will happen!");
                    return;
                }
                LaunchWindow_Component.SetChildStatus(child, (childStatus)defaultOption);
            }

            WindowInteropHelper childHelper = new WindowInteropHelper(child);
            WindowInteropHelper parentHelper = new WindowInteropHelper(parent);
            childHelper.Owner = parentHelper.Handle;


        }
    }
}
