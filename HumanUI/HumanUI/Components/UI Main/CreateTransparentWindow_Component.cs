using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace HumanUI.Components.UI_Main
{

    /// <summary>
    /// This extends the main LaunchWindow_Component class to alter some of the behavior - adding the ability to allow transparent background colors. 
    /// </summary>
    /// <seealso cref="HumanUI.LaunchWindow_Component" />
    public class CreateTransparentWindow_Component : LaunchWindow_Component
    {
        public CreateTransparentWindow_Component()
            : base("Launch Transparent Window", "LaunchXPWin", "This component launches a new blank, transparent control window.", "Human UI", "UI Main")
        {

        }


        /// <summary>
        /// Override the guid so it is recognized as a separate component
        /// </summary>
        public override Guid ComponentGuid => new Guid("{106D7436-7223-454A-A2DA-57EE118E6815}");

        protected override System.Drawing.Bitmap Icon => Properties.Resources.LaunchWindow_Transparent;

        /// <summary>
        /// Enable transparency before calling on the main window implementation
        /// </summary>
        protected override void SolveInstance(Grasshopper.Kernel.IGH_DataAccess DA)
        {
            if (!mw.AllowsTransparency) mw.AllowsTransparency = true;
            mw.Background = new SolidColorBrush(Color.FromArgb(100,255,255,255));
            base.SolveInstance(DA);
        }
    }
}
