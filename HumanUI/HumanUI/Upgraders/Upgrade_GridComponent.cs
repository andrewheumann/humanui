using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

using Grasshopper.Kernel.Parameters;

namespace HumanUI.Upgraders
{
    public class Upgrade_GridComponent : IGH_UpgradeObject
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
            HumanUI.Components.UI_Containers.CreateGrid_Component testComp = new Components.UI_Containers.CreateGrid_Component();

            //identify the indices of the newly added params (relative to the old version of the component)
            int[] paramsToCopy = new int[] { 3, 4, 5, 6, 7, 8 };
            //grab the input params at those indices and register them to the component
            paramsToCopy.Select(i => testComp.Params.Input[i]).ToList().ForEach(p => newComponent.Params.RegisterInputParam(p));
            //make sure display and such gets cleaned up
            newComponent.Params.OnParametersChanged();
            return newComponent;
        }

        public Guid UpgradeFrom => new Guid("{1E68A9A8-C28D-4799-854C-337DC4018917}");

        public Guid UpgradeTo => new Guid("{B618569A-868D-4A88-A035-FAA1416A841F}");

        public DateTime Version => new DateTime(2016, 1, 11, 13, 0, 0);
    }
}
