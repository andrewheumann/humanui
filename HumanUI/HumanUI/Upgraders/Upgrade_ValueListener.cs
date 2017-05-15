using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI.Upgraders
{
    public class Upgrade_ValueListener : IGH_UpgradeObject
    {

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            ValueListener_Component_DEPRECATED component = target as ValueListener_Component_DEPRECATED;
            if (component == null)
            {
                return null;
            }
            IGH_Component swappedComp = GH_UpgradeUtil.SwapComponents(component, this.UpgradeTo);

            ValueListener_Component swappedValListener = swappedComp as ValueListener_Component;

            swappedValListener.AddEventsEnabled = component.AddEventsEnabled;
            swappedValListener.updateMessage();

            return swappedComp;
        }

        public Guid UpgradeFrom => new Guid("{78fb7e0c-ae2a-45ad-b09c-83df32d0b3bc}");

        public Guid UpgradeTo => new Guid("{D6BA0398-70A7-46E7-A068-274486EB0ACB}");

        public DateTime Version => new DateTime(2016, 2, 17);
    }
}
