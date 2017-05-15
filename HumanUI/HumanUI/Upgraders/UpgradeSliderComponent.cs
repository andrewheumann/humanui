using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using HumanUI.Components.UI_Elements;

namespace HumanUI.Upgraders
{
    public class UpgradeSliderComponent1to3 : IGH_UpgradeObject
    {

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null) return null;

            IGH_Component newComponent = GH_UpgradeUtil.SwapComponents(component, UpgradeTo);

            CreateSlider_Component tempComp = new CreateSlider_Component();

            IGH_Param inputParam = tempComp.Params.Input[1];

            newComponent.Params.RegisterInputParam(inputParam);


            return newComponent;
        }

        public Guid UpgradeFrom => new Guid("{16231ddc-6473-42d4-b81f-e0c5e90e8fbd}");

        public Guid UpgradeTo => new Guid("{C77ACC8A-FE64-43F0-9485-D23744F6152E}");

        public DateTime Version => new DateTime(2016, 1, 16);
    }


    public class UpgradeSliderComponent2to3 : IGH_UpgradeObject
    {

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null) return null;

            IGH_Component newComponent = GH_UpgradeUtil.SwapComponents(component, UpgradeTo);

            CreateSlider_Component tempComp = new CreateSlider_Component();

            IGH_Param inputParam = tempComp.Params.Input[1];

            newComponent.Params.RegisterInputParam(inputParam);


            return newComponent;
        }

        public Guid UpgradeFrom => new Guid("{E4F276AF-46FB-478E-B10A-B95E7B04DFF0}");

        public Guid UpgradeTo => new Guid("{C77ACC8A-FE64-43F0-9485-D23744F6152E}");

        public DateTime Version => new DateTime(2016, 1, 16);
    }
}
