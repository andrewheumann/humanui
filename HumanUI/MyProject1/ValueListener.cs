using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GH_IO.Serialization;

namespace HumanUI
{
    public class ValueListener : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ValueListener class.
        /// </summary>
        public ValueListener()
            : base("Value Listener", "Values",
                "This component is used to retrieve the values of UI elements from the window. By default it will automatically refresh when those values change.",
                "Human", "UI Main")
        {
            eventedElements = new List<UIElement>();
            AddEventsEnabled = true;
            updateMessage();
        }

     
        private static List<UIElement> eventedElements;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "UI Elements to listen to.", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The filter(s) for the elements you want to listen for.", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Values", "V", "The values of the listened elements", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> elementFilters = new List<string>();

            if (!DA.GetDataList<KeyValuePair<string, UIElement_Goo>>("Elements", allElements)) return;
            if (!DA.GetDataList<string>("Name Filter(s)", elementFilters)) return;

            //create a dictionary
            Dictionary<string, UIElement_Goo> elementDict = allElements.ToDictionary(pair => pair.Key, pair => pair.Value);
            List<UIElement> filteredElements = new List<UIElement>();
            //filter the dictionary
            foreach (string fil in elementFilters)
            {
                
                filteredElements.Add(elementDict[fil].element);
            }
            //if there are no filters, include all values. 
            if (elementFilters.Count == 0)
            {
                foreach(UIElement_Goo u in elementDict.Values){
                    filteredElements.Add(u.element);
                }
            }

            //remove all events from previously "evented" elements
            foreach (UIElement u in eventedElements)
            {
                RemoveEvents(u);
            }
            eventedElements.Clear();

            //extract base elements
            List<UIElement> elementsToListen = new List<UIElement>();
            HUI_Util.extractBaseElements(filteredElements,elementsToListen);

            //retrieve element values
            List<object> values = new List<object>();
            foreach (UIElement u in elementsToListen)
            {
                values.Add(HUI_Util.GetElementValue(u));
                //add listener events to elements 
               if(AddEventsEnabled) AddEvents(u);
            }

           


            DA.SetDataList("Values", values);

        }

       

       

        /*
        void extractBaseElements(Dictionary<string, UIElement_Goo> elements, List<UIElement> extractedElements)
        {
             foreach (KeyValuePair<string, UIElement_Goo> element in elements)
             {
                 if (element.Value.element is Panel)
                 {
                     Panel p = element.Value.element as Panel;
                     switch (p.Name)
                     {
                         case "GH Slider":
                             extractedElements.Add(p.Children[1]);
                             return;
                         default:
                             extractBaseElements(p.Children, extractedElements);
                             return;
                     }
                 }
                 else
                 {
                     extractedElements.Add(element.Value.element);
                 }
             }

       
       

        } */


        void AddEvents(UIElement u)
        {
            eventedElements.Add(u);
            switch (u.GetType().ToString())
            {
                case "System.Windows.Controls.Slider":
                    Slider s = u as Slider;
                    s.ValueChanged -= ExpireThis;
                    s.ValueChanged += ExpireThis;

                    return;
                case "System.Windows.Controls.Button":
                    Button b = u as Button;
                    b.PreviewMouseDown -= ExpireThis;
                    b.PreviewMouseDown += ExpireThis;
                    b.PreviewMouseUp -= ExpireThis;
                    b.PreviewMouseUp += ExpireThis;
                    return;
                case "System.Windows.Controls.Label":
                    Label l = u as Label;
                    return;
                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    lb.SelectionChanged -= ExpireThis;
                    lb.SelectionChanged += ExpireThis;
                    return;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    cb.SelectionChanged -= ExpireThis;
                    cb.SelectionChanged += ExpireThis;
                    return;
                case "System.Windows.Controls.TextBox":
                    TextBox tb = u as TextBox;
                    StackPanel p = tb.Parent as StackPanel;
                    List<Button> btns = p.Children.OfType<Button>().ToList<Button>();
                    if (btns.Count > 0)
                    {
                        foreach (Button btn0 in btns)
                        {
                            btn0.Click -= ExpireThis;
                            btn0.Click += ExpireThis;
                        }
                    }
                    else
                    {
                        tb.TextChanged -= ExpireThis;
                        tb.TextChanged += ExpireThis;
                    }
                    return;
                case "System.Windows.Controls.ListView":
                    ListView v = u as ListView;
                     var cbs = from cbx in v.Items.OfType<CheckBox>() select cbx;
                    foreach (CheckBox chex in cbs)
                    {
                        chex.Checked -= ExpireThis;
                        chex.Unchecked -= ExpireThis;
                        chex.Checked += ExpireThis;
                        chex.Unchecked += ExpireThis;
                    }
                    return;
                case "System.Windows.Controls.CheckBox":
                    CheckBox chb = u as CheckBox;
                    chb.Checked -= ExpireThis;
                    chb.Unchecked -= ExpireThis;
                    chb.Checked += ExpireThis;
                    chb.Unchecked += ExpireThis;
                    return;
                case "System.Windows.Controls.RadioButton":
                    RadioButton rb = u as RadioButton;
                    rb.Checked -= ExpireThis;
                    rb.Checked += ExpireThis;
                    rb.Unchecked -= ExpireThis;
                    rb.Unchecked += ExpireThis;
                    return;
                case "System.Windows.Controls.Image":
                    return;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    tc.SelectionChanged -= ExpireThis;
                    tc.SelectionChanged += ExpireThis;
                    return;
                default:
                    return;
            }
        }

        void RemoveEvents(UIElement u)
        {
            switch (u.GetType().ToString())
            {
                case "System.Windows.Controls.Slider":
                    Slider s = u as Slider;
                    s.ValueChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.Button":
                    Button b = u as Button;
                    b.PreviewMouseDown -= ExpireThis;
                    b.PreviewMouseUp -= ExpireThis;
                    return;
                case "System.Windows.Controls.Label":
                    Label l = u as Label;
                    return;
                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    lb.SelectionChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    cb.SelectionChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.TextBox":
                    TextBox tb = u as TextBox;
                    StackPanel p = tb.Parent as StackPanel;
                    List<Button> btns = p.Children.OfType<Button>().ToList<Button>();
                 
                        foreach (Button btn0 in btns)
                        {
                            btn0.Click -= ExpireThis;
                        }
                        tb.TextChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.ListView":
                    ListView v = u as ListView;
                    var cbs = from cbx in v.Items.OfType<CheckBox>() select cbx;
                    foreach (CheckBox chex in cbs)
                    {
                        chex.Checked -= ExpireThis;
                        chex.Unchecked -= ExpireThis;
                    }
                    return;
                case "System.Windows.Controls.CheckBox":
                    CheckBox chb = u as CheckBox;
                    chb.Checked -= ExpireThis;
                    chb.Unchecked -= ExpireThis;
                    return;
                case "System.Windows.Controls.RadioButton":
                    RadioButton rb = u as RadioButton;
                    rb.Checked -= ExpireThis;
                    rb.Unchecked -= ExpireThis;
                    return;
                case "System.Windows.Controls.Image":
                    return;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    tc.SelectionChanged -= ExpireThis;
                    return;
                default:
                    return;
            }
        }

        

        void ExpireThis(object sender, EventArgs e)
        {
           // System.Windows.Forms.MessageBox.Show("Event Trigger");
            ExpireSolution(true);
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
                return Properties.Resources.ValueListener;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{78fb7e0c-ae2a-45ad-b09c-83df32d0b3bc}"); }
        }


        private void updateMessage()
        {
            Message = AddEventsEnabled ? "Live" : "Manual Update";
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("SomeProperty", AddEventsEnabled);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            AddEventsEnabled = false;
            reader.TryGetBoolean("SomeProperty", ref AddEventsEnabled);
            updateMessage();
            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            System.Windows.Forms.ToolStripMenuItem AddEventsMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Live Update", new EventHandler(this.Menu_AddEventsClicked), true, AddEventsEnabled);
            AddEventsMenuItem.ToolTipText = "When checked, the component will automatically update when UI element values change in the window.";
        }
        public void Menu_AddEventsClicked(object sender, System.EventArgs e)
        {
            this.RecordUndoEvent("Add Events");
            this.AddEventsEnabled = !this.AddEventsEnabled;
            updateMessage();
            this.ExpireSolution(true);
        }


        private bool AddEventsEnabled = true;


    }
}