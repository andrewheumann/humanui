using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI.Upgraders
{
    public class Upgrade_TextBoxComponent : IGH_UpgradeObject
    {
        public Guid UpgradeFrom => new Guid("{c8d203fe-7e84-416a-b93e-d1bd746f3f66}");

        public Guid UpgradeTo => new Guid("{41A3A0D8-E0F4-4B48-88B3-BF87D79A3CFD}");

        public DateTime Version => new DateTime(2016, 5, 3);

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null)
            {
                return null;
            }

            IGH_Component swappedComp = GH_UpgradeUtil.SwapComponents(component, UpgradeTo);
            return swappedComp;
        }
    }
}
