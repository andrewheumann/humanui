using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Controls;
using System.Windows;


namespace HumanUI.Components.UI_Elements
{
    /// <summary>
    /// Component to create a stackpanel containing a textbox and an optional button for user entry. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class CreateTextBox_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateTextBox_Component class.
        /// </summary>
        /// 

        // Set show-label boolean for custom right-click menu
        private bool showLabel;
        private bool enterEvent;
        private bool enterMenuEnabled;


        public CreateTextBox_Component()
            : base("Create Text Box", "TextBox",
                "Create a box for text entry, with a button to pass its value.",
                "Human UI", "UI Elements")
        {
            showLabel = true;
        }
                
        // Create right-click menu item for show-label
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem ShowLabelMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Show Label", new EventHandler(this.Menu_ShowLabelClicked), true, showLabel);
            ShowLabelMenuItem.ToolTipText = "When checked, the UI Element will include the supplied label.";

            System.Windows.Forms.ToolStripMenuItem EnterListenerMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Use Enter to submit", new EventHandler(this.Menu_EnterEventClicked), true, enterEvent);
            ShowLabelMenuItem.ToolTipText = "If checked, the text will be submitted when Enter key is pressed.";
            if (!enterMenuEnabled)
            {
                EnterListenerMenuItem.Enabled = false;
                EnterListenerMenuItem.Checked = true;
            }
        }

        private void Menu_EnterEventClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Enter Event Toggle");
            enterEvent = !enterEvent;
            ExpireSolution(true);
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
            writer.SetBoolean("enterEvent", enterEvent);

            return base.Write(writer);
        }


        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            reader.TryGetBoolean("showLabel", ref showLabel);
            reader.TryGetBoolean("enterEvent", ref enterEvent);
            //updateMessage();
            return base.Read(reader);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Label", "L", "Optional label for the Text Box", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Default Text", "D", "The starting text in the text box", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Update Button", "U", "Set to true to associate text box \nwith a button for updates. Otherwise event listening will \nassociate with every change in text box content.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Text Box", "TB", "The created text box.", GH_ParamAccess.item);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool includeButton = true;
            string label = "";
            string defaultValue = "";
            if (!DA.GetData<string>("Default Text", ref defaultValue)) return;
            DA.GetData<string>("Label", ref label);
            DA.GetData<bool>("Update Button", ref includeButton);
            //set up the stackpanel to contain the text box
            DockPanel sp = new DockPanel();
            //  sp.Orientation = Orientation.Horizontal;
            //create the text box
            TextBox tb = new TextBox();
            tb.Text = defaultValue;

            //set up the button
            Button b = new Button();
            b.Width = 50;
            sp.Margin = new Thickness(4);
            Label l = new Label();
            l.Content = label;
            //add the label to the stackpanel if showLabel is true
            if (!string.IsNullOrWhiteSpace(label) & showLabel) sp.Children.Add(l);

            

            if (includeButton) // if the component is set to use a button for updating, add the button to the stack panel
            {
                sp.Children.Add(b);
                DockPanel.SetDock(b, Dock.Right);
                //this key is used by other methods (like AddEvents) to figure out whether or not to listen to all changes or just button presses.
                sp.Name = "GH_TextBox";

                // disable menu item (enter will still work)
                enterMenuEnabled = false;
            }

            else
            {
                //this key is used by other methods (like AddEvents) to figure out whether or not to listen to all changes or just button presses.
                sp.Name = "GH_TextBox_NoButton";

                enterMenuEnabled = true;
            }

            tb.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(tb);

            // save enter event in a textbox tag
            if (enterEvent || !enterMenuEnabled)
                tb.Tag = "enterEvent";

            //pass out the stackpanel
            DA.SetData("Text Box", new UIElement_Goo(sp, String.Format("TextBox: {0}", label), InstanceGuid, DA.Iteration));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateTextBox;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{41A3A0D8-E0F4-4B48-88B3-BF87D79A3CFD}");
    }
}