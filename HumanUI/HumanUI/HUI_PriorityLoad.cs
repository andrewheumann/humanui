using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI
{
    public class HUI_PriorityLoad : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Human UI", Properties.Resources.Icon_16);

            return GH_LoadingInstruction.Proceed;
        }
    }
}
