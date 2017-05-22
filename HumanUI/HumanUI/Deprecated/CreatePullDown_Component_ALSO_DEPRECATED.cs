using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grasshopper.Kernel.Special;
using System.Linq;
using Grasshopper.Kernel.Types;

namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a Combobox ("Pulldown menu")
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreatePullDown_Component_ALSO_DEPRECATED : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreatePullDown_Component_ALSO_DEPRECATED()
            : base("Create Pulldown Menu", "Pulldown",
                "Creates a pulldown menu from which items can be selected.",
                "Human UI", "UI Elements")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List Items", "L", "The initial list of options to display in the list.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Selected Index", "I", "The initially selected index. Defaults to the first item.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pulldown", "PD", "The pulldown object", GH_ParamAccess.list);
        }

        internal int Iterator = 0;


        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0) Iterator = 0;
            List<GH_ValueList> GHValLists = new List<GH_ValueList>();


            List<object> listItems = new List<object>();
            int selectedIndex = 0;

            if (!DA.GetDataList<object>("List Items", listItems)) return;
            bool selectedIndexSupplied = DA.GetData<int>("Selected Index", ref selectedIndex);

            //try to retrieve any attached GHValueLists
            GHValLists.AddRange(Params.Input[0].Sources.Where(s => s is GH_ValueList).Cast<GH_ValueList>());

            //if GHValLists is empty, either user has supplied direct text or direct value list objects, e.g. from metahopper output

            if (GHValLists.Count == 0)
            { // see if we got any vallists as objects directly
                foreach (object o in listItems)
                {
                    GH_ObjectWrapper wrapper = o as GH_ObjectWrapper;
                    if (wrapper != null)
                    {
                        GHValLists.Add(wrapper.Value as GH_ValueList);
                    }
                }
            }

            //if GHValLists is STILL empty, we just process straight up text, once. otherwise, we iterate over all the lists

            if (GHValLists.Count == 0)
            {
                //initialize combobox
                ComboBox pd = new ComboBox();
                //for each string add a textbox object to the combobox
                foreach (object item in listItems)
                {
                    TextBlock textbox = new TextBlock();
                    textbox.Text = item.ToString();
                    pd.Items.Add(textbox);
                }
                pd.Margin = new Thickness(4);
                pd.SelectedIndex = selectedIndex;

                //pass out the combobox
                DA.SetDataList("Pulldown", new List<UIElement_Goo>() { new UIElement_Goo(pd, "Pulldown", InstanceGuid, Iterator) });
                Iterator++;
            }
            else
            {
                List<UIElement_Goo> goosOut = new List<UIElement_Goo>();
                foreach (GH_ValueList valList in GHValLists)
                {
                    //initialize combobox
                    ComboBox pd = new ComboBox();
                    //for each string add a textbox object to the combobox

                    List<string> values = valList.ListItems.Select(li => li.Name).ToList();

                    foreach (string value in values)
                    {
                        TextBlock textbox = new TextBlock();
                        textbox.Text = value;
                        pd.Items.Add(textbox);
                    }
                    pd.Margin = new Thickness(4);
                    if (selectedIndexSupplied)
                    {
                        pd.SelectedIndex = selectedIndex;
                    }
                    else
                    {
                        pd.SelectedIndex = valList.ListItems.IndexOf(valList.FirstSelectedItem);
                    }

                    //pass out the combobox
                    goosOut.Add(new UIElement_Goo(pd, "Pulldown", InstanceGuid, Iterator));
                    Iterator++;
                }
                DA.SetDataList("Pulldown", goosOut);
            }


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreatePullDown;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{8F5B1D66-DE73-47A2-9678-9E59CEA106C0}");
    }
}