using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI.Upgraders
{
    public class Upgrade_CreatePulldownComponent : IGH_UpgradeObject
    {

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null)
            {
                return null;
            }
            GH_Component swappedComp = GH_UpgradeUtil.SwapComponents(component, this.UpgradeTo) as GH_Component;
            swappedComp.Params.Output[0].DataMapping = GH_DataMapping.Flatten;

            return swappedComp;
        }

        public Guid UpgradeFrom => new Guid("{1CA8D537-EF52-487C-828D-034B1BCA7361}");

        public Guid UpgradeTo => new Guid("{8F5B1D66-DE73-47A2-9678-9E59CEA106C0}");

        public DateTime Version => new DateTime(2016, 2, 17);
    }
}
