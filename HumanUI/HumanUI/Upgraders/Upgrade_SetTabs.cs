using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

using Grasshopper.Kernel.Parameters;

namespace HumanUI.Upgraders
{
    public class Upgrade_SetTabs : IGH_UpgradeObject
    {

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null)
            {
                return null;
            }
            var newComponent = GH_UpgradeUtil.SwapComponents(component, this.UpgradeTo);

            //create a dummy instance of our new version of the component
            HumanUI.Components.UI_Output.SetTabs_Component testComp = new Components.UI_Output.SetTabs_Component();

            //identify the indices of the newly added params (relative to the old version of the component)
            int[] paramsToCopy = new int[] { 3 };
            //grab the input params at those indices and register them to the component
            paramsToCopy.Select(i => testComp.Params.Input[i]).ToList().ForEach(p => newComponent.Params.RegisterInputParam(p));
            //make sure display and such gets cleaned up
            newComponent.Params.OnParametersChanged();
            return newComponent;
        }

        public Guid UpgradeFrom => new Guid("{0a32b740-1539-4417-ba73-c4885329bd77}");

        public Guid UpgradeTo => new Guid("{1e63a1ca-e3e8-44ad-8ee6-0a660e01c84b}");

        public DateTime Version => new DateTime(2017, 9, 19);
    }
}
