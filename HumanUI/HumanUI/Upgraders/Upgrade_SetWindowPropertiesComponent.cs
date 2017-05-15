using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI.Upgraders
{

    public class Upgrade_SetWindowPropertiesComponent2to3 : IGH_UpgradeObject
    {
        public Guid UpgradeFrom => new Guid("{B2CA4D57-1F81-4CE5-AED6-2A39FB285814}");

        public Guid UpgradeTo => new Guid("{14A1EE78-6536-43B2-B6D8-4B26A736F0A9}");

        public DateTime Version => new DateTime(2016, 05, 04);

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null) return null;

            IGH_Component newComponent = GH_UpgradeUtil.SwapComponents(component, UpgradeTo);

            Components.SetWindowProperties_Component tempComp = new Components.SetWindowProperties_Component();

            IGH_Param inputParam = tempComp.Params.Input[6];

            newComponent.Params.RegisterInputParam(inputParam);


            return newComponent;
        }
    }
}
