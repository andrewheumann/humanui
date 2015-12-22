using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanUI.Components.UI_Main
{
    public class CreateTransparentWindow_Component : LaunchWindow_Component
    {
        public CreateTransparentWindow_Component()
            : base("Launch Transparent Window", "LaunchXPWin", "This component launches a new blank, transparent control window.", "Human", "UI Main")
        {

        }

        public override Guid ComponentGuid
        {
            get { 
                return new Guid("{106D7436-7223-454A-A2DA-57EE118E6815}"); 
            }
        }

        protected override void SolveInstance(Grasshopper.Kernel.IGH_DataAccess DA)
        {
            if (!mw.AllowsTransparency) mw.AllowsTransparency = true;
            base.SolveInstance(DA);
        }
    }
}
