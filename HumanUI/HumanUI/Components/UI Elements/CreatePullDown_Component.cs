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
    public class CreatePullDown_Component : GH_Component
    {

        // Set show-label boolean for custom right-click menu
        private bool showLabel;

        /// <summary>
        /// Initializes a new instance of the CreateListBox_Component class.
        /// </summary>
        public CreatePullDown_Component()
            : base("Create Pulldown Menu", "Pulldown",
                "Creates a pulldown menu from which items can be selected.",
                "Human UI", "UI Elements")
        {
            showLabel = true;
        }

        // Create right-click menu item for show-label
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem ShowLabelMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Show Label", new EventHandler(this.Menu_ShowLabelClicked), true, showLabel);
            ShowLabelMenuItem.ToolTipText = "When checked, the UI Element will include the supplied label.";
        }

        // Method called on click event of Menu Item
        public void Menu_ShowLabelClicked(object sender, System.EventArgs e)
        {
            RecordUndoEvent("Show Label Toggle");
            showLabel = !showLabel;
            //updateMessage();
            ExpireSolution(true);
        }

        // Methods to save the boolean state of the component between file opens

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("showLabel", showLabel);

            return base.Write(writer);
        }


        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            showLabel = reader.GetBoolean("showLabel");
            //updateMessage();
            return base.Read(reader);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Label", "L", "Optional label for the Text Box", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("List Items", "L", "The initial list of options to display in the list.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Selected Index", "I", "The initially selected index. Defaults to the first item.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pulldown", "PD", "The pulldown object", GH_ParamAccess.list);
        }

        internal int Iterator = 0;


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string label = "";
            DA.GetData<string>("Label", ref label);

            if (DA.Iteration == 0) Iterator = 0;
            List<GH_ValueList> GHValLists = new List<GH_ValueList>();


            List<object> listItems = new List<object>();
            int selectedIndex = 0;

            if (!DA.GetDataList<object>("List Items", listItems)) return;
            bool selectedIndexSupplied = DA.GetData<int>("Selected Index", ref selectedIndex);

            //try to retrieve any attached GHValueLists
            GHValLists.AddRange(Params.Input[1].Sources.Where(s => s is GH_ValueList).Cast<GH_ValueList>());

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

                DockPanel sp = new DockPanel();
                //  sp.Orientation = Orientation.Horizontal;
                
                //set up the button
                sp.Margin = new Thickness(4);
                Label l = new Label();
                l.Content = label;

                //add the label to the stackpanel if showLabel is true
                if (!string.IsNullOrWhiteSpace(label) & showLabel)
                {
                    sp.Name = "GH_PullDown_Label";
                    sp.Children.Add(l);
                } else
                {
                    sp.Name = "GH_PullDown_NoLabel";
                }

                //List<UIElement_Goo> combobox = new List<UIElement_Goo>() { new UIElement_Goo(pd, "Pulldown", InstanceGuid, Iterator) };

                sp.Children.Add(pd);

                //pass out the stackpanel
                DA.SetData("Pulldown", new UIElement_Goo(sp, String.Format("Pulldown: {0}", label), InstanceGuid, DA.Iteration));

                ////pass out the combobox
                //DA.SetDataList("Pulldown", new List<UIElement_Goo>() { new UIElement_Goo(pd, "Pulldown", InstanceGuid, Iterator) });
                //Iterator++;
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

                    DockPanel sp = new DockPanel();
                    //  sp.Orientation = Orientation.Horizontal;

                    //set up the button
                    sp.Margin = new Thickness(4);
                    Label l = new Label();
                    l.Content = label;

                    //add the label to the stackpanel if showLabel is true
                    if (!string.IsNullOrWhiteSpace(label) & showLabel)
                    {
                        sp.Name = "GH_PullDown_Label";
                        sp.Children.Add(l);
                    }
                    else
                    {
                        sp.Name = "GH_PullDown_NoLabel";
                    }

                    sp.Children.Add(pd);

                    //pass out the stackpanel
                    DA.SetData("Pulldown", new UIElement_Goo(sp, String.Format("Pulldown: {0}", label), InstanceGuid, DA.Iteration));

                    ////pass out the combobox
                    //goosOut.Add(new UIElement_Goo(pd, "Pulldown", InstanceGuid, Iterator));
                    //Iterator++;
                }
                //DA.SetDataList("Pulldown", goosOut);
            }


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.CreatePullDown;
            }
        }


/// <summary>
/// Gets the unique ID for this component. Do not change this ID after release.
/// </summary>
public override Guid ComponentGuid
        {
            get { return new Guid("{fc6ae741-ecd1-432f-abb4-36b3f439c6f5}"); }
        }
    }
}