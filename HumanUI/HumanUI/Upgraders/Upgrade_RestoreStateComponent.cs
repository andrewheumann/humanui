using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace HumanUI
{
    /// <summary>
    /// A class implementing IGH_UpgradeObject to enable the "Upgrade component" menu to recognize out of date "Restore State" components.
    /// I haven't been consistent about doing this when I deprecate old components, but I'd like to be for future modifications. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.IGH_UpgradeObject" />
   public class Upgrade_RestoreStateComponent : IGH_UpgradeObject
    {
        /// <summary>
        /// Upgrade an existing object.
        /// </summary>
        /// <param name="target">Object to upgrade.</param>
        /// <param name="document">Document that contains the object.</param>
        /// <returns>
        /// The newly created object on success, null on failure.
        /// </returns>
        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            IGH_Component component = target as IGH_Component;
            if (component == null)
            {
                return null;
            }
            return GH_UpgradeUtil.SwapComponents(component, this.UpgradeTo);
        }

        /// <summary>
        /// Gets the ComponentGuid of the old object (the object to be updated).
        /// </summary>
        public Guid UpgradeFrom => new Guid("{d106b262-7a20-4151-b59a-872300f7ee9c}");

        /// <summary>
        /// Gets the ComponentGuid of the new object (the object that will be inserted).
        /// </summary>
        public Guid UpgradeTo => new Guid("{A6567BB1-37D1-46CB-AD10-594FF726299B}");

        /// <summary>
        /// Return a DateTime object that indicates when this upgrade mechanism was written,
        /// so that it becomes possible to distinguish competing upgrade mechanisms.
        /// </summary>
        public DateTime Version => new DateTime(2015, 8, 31);
    }
}
