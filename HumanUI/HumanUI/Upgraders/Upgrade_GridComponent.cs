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

            HumanUI.Components.UI_Containers.CreateGrid_Component testComp = new Components.UI_Containers.CreateGrid_Component();

            IGH_Param RowDescription = testComp.Params.Input[3];
            IGH_Param ColDescription = testComp.Params.Input[4];
            IGH_Param ElementRows = testComp.Params.Input[5];
            IGH_Param ElementCols = testComp.Params.Input[6];
            IGH_Param ElementRowSpans = testComp.Params.Input[7];
            IGH_Param ElementColSpans = testComp.Params.Input[8];
            newComponent.Params.RegisterInputParam(RowDescription);
            newComponent.Params.RegisterInputParam(ColDescription);
            newComponent.Params.RegisterInputParam(ElementRows);
            newComponent.Params.RegisterInputParam(ElementCols);
            newComponent.Params.RegisterInputParam(ElementRowSpans);
            newComponent.Params.RegisterInputParam(ElementColSpans);
            newComponent.Params.OnParametersChanged();
            return newComponent;
        }

        public Guid UpgradeFrom
        {
            get { return new Guid("{1E68A9A8-C28D-4799-854C-337DC4018917}"); }
        }

        public Guid UpgradeTo
        {
            get { return new Guid("{B618569A-868D-4A88-A035-FAA1416A841F}"); }
        }

        public DateTime Version
        {
            get { return new DateTime(2016, 1, 11, 13, 0, 0); }
        }
    }
}
