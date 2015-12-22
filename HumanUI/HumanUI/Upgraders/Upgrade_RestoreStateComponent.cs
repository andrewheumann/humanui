using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace HumanUI
{
   public class Upgrade_RestoreStateComponent : IGH_UpgradeObject
    {
        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null)
            {
                return null;
            }
            return GH_UpgradeUtil.SwapComponents(component, this.UpgradeTo);
        }

        public Guid UpgradeFrom
        {
            get { return new Guid("{d106b262-7a20-4151-b59a-872300f7ee9c}"); }
        }

        public Guid UpgradeTo
        {
            get { return new Guid("{A6567BB1-37D1-46CB-AD10-594FF726299B}"); }
        }

        public DateTime Version
        {
            get { return new DateTime(2015, 8, 31); }
        }
    }
}
