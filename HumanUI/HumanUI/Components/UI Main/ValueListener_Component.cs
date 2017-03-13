using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Xceed.Wpf.Toolkit;

using System.Collections;
using De.TorstenMandelkow.MetroChart;
using Grasshopper.Kernel.Parameters;
using MahApps.Metro.Controls;
using RangeSlider = MahApps.Metro.Controls.RangeSlider;

namespace HumanUI
{
    public class ValueListener_Component : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the ValueListener class.
        /// </summary>
        public ValueListener_Component()
            : base("Value Listener", "Values",
                "This component is used to retrieve the values of UI elements from the window. By default it will automatically refresh when those values change.",
                "Human UI", "UI Main")
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
            pManager.AddGenericParameter("Elements", "E", "UI Element(s) to listen to. This can be retrieved either directly from the component \n that generated the element, or from the output of the \"Add Elements\" component.", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The optional filter(s) for the elements you want to listen for.", GH_ParamAccess.list);
            pManager[1].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Values", "V", "The values of the listened elements", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Indices", "I", "For list-based objects (checklist, pulldown menu, etc) returns the selected index - otherwise returns -1.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> elementObjects = new List<object>();
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> elementFilters = new List<string>();




            if (!DA.GetDataList<object>("Elements", elementObjects)) return;
            DA.GetDataList<string>("Name Filter(s)", elementFilters);



            //the elements to listen to
            List<UIElement> filteredElements = new List<UIElement>();



            //decide whether to populate the dictionary, or just populate filteredElements directly. 
            foreach (object o in elementObjects)
            {
                UIElement elem = null;
                switch (o.GetType().ToString())
                {
                    case "HumanUI.UIElement_Goo":
                        UIElement_Goo goo = o as UIElement_Goo;
                        elem = goo.element as UIElement;
                        filteredElements.Add(elem);
                        break;
                    case "Grasshopper.Kernel.Types.GH_ObjectWrapper":
                        GH_ObjectWrapper wrapper = o as GH_ObjectWrapper;
                        KeyValuePair<string, UIElement_Goo> kvp = (KeyValuePair<string, UIElement_Goo>)wrapper.Value;
                        allElements.Add(kvp);
                        break;
                    default:
                        break;
                }
            }

            if (allElements.Count > 0) //if we've been getting keyvaluepairs and need to filter
            {

                //create a dictionary for filtering
                Dictionary<string, UIElement_Goo> elementDict = allElements.ToDictionary(pair => pair.Key, pair => pair.Value);



                //filter the dictionary
                foreach (string fil in elementFilters)
                {

                    filteredElements.Add(elementDict[fil].element);
                }
                //if there are no filters, include all values. 
                if (elementFilters.Count == 0)
                {
                    foreach (UIElement_Goo u in elementDict.Values)
                    {
                        filteredElements.Add(u.element);
                    }
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
            HUI_Util.extractBaseElements(filteredElements, elementsToListen);




            //retrieve element values
            GH_Structure<IGH_Goo> values = new GH_Structure<IGH_Goo>();
            GH_Structure<GH_Integer> indsOut = new GH_Structure<GH_Integer>();
            int i = 0;
            foreach (UIElement u in elementsToListen)
            {
                object value = HUI_Util.GetElementValue(u);
                object indices = HUI_Util.GetElementIndex(u);
                IEnumerable list = null;
                if ((value as string) == null) list = value as IEnumerable;
                IEnumerable indList = indices as IEnumerable;
                if (list != null)
                {
                    foreach (object thing in list)
                    {
                        values.Append(HUI_Util.GetRightType(thing), new GH_Path(i));
                    }
                }
                else
                {
                    values.Append(HUI_Util.GetRightType(value), new GH_Path(i));
                }

                if (indList != null)
                {
                    foreach (int index in indList)
                    {
                        indsOut.Append(new GH_Integer(index), new GH_Path(i));
                    }
                }
                else
                {
                    indsOut.Append(new GH_Integer((int)indices), new GH_Path(i));
                }






                //add listener events to elements 
                if (AddEventsEnabled)
                {
                    eventedElements.Add(u);
                    AddEvents(u);
                }
                i++;
            }




            DA.SetDataTree(0, values);
            DA.SetDataTree(1, indsOut);

        }








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
                case "System.Windows.Controls.DataGrid":
                    DataGrid datagrid = u as DataGrid;
                    datagrid.SelectedCellsChanged -= ExpireThis;
                    datagrid.SelectedCellsChanged += ExpireThis;
                    return;
                case "HumanUI.TrueOnlyButton":
                    TrueOnlyButton tob = u as TrueOnlyButton;
                    tob.PreviewMouseDown -= ExpireThis;
                    tob.PreviewMouseDown += ExpireThis;
                    return;
                case "HumanUI.MDSliderElement":
                    MDSliderElement mds = u as MDSliderElement;
                    mds.PropertyChanged -= ExpireThis;
                    mds.PropertyChanged += ExpireThis;
                    return;
                case "HumanUI.GraphMapperElement":
                    GraphMapperElement gme = u as GraphMapperElement;
                    gme.PropertyChanged -= ExpireThis;
                    gme.PropertyChanged += ExpireThis;
                    return;
                case "HumanUI.HUI_GradientEditor":
                    HUI_GradientEditor hge = u as HUI_GradientEditor;
                    hge.PropertyChanged -= ExpireThis;
                    hge.PropertyChanged += ExpireThis;
                    return;
                case "HumanUI.FilePicker":
                    FilePicker fp = u as FilePicker;
                    fp.PropertyChanged -= ExpireThis;
                    fp.PropertyChanged += ExpireThis;
                    return;
                case "HumanUI.ClickableShapeGrid":
                   ClickableShapeGrid csg = u as ClickableShapeGrid;
                   switch (csg.clickMode)
                   {
                       case ClickableShapeGrid.ClickMode.ButtonMode:
                           csg.MouseUp -= ExpireThis;
                           csg.MouseUp += ExpireThis;
                           csg.MouseDown -= ExpireThis;
                           csg.MouseDown += ExpireThis;
                           return;
                       case ClickableShapeGrid.ClickMode.PickerMode:
                       case ClickableShapeGrid.ClickMode.ToggleMode:
                           csg.MouseUp -= ExpireThis;
                           csg.MouseUp += ExpireThis;
                           return;
                       case ClickableShapeGrid.ClickMode.None:
                       default:
                           return;
                   }
                case "System.Windows.Controls.Label":
                    Label l = u as Label;
                    return;
                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    lb.SelectionChanged -= ExpireThis;
                    lb.SelectionChanged += ExpireThis;
                    return;
                case "System.Windows.Controls.ScrollViewer":
                    ScrollViewer sv = u as ScrollViewer;
                    ItemsControl ic = sv.Content as ItemsControl;
                    ((INotifyCollectionChanged)ic.Items).CollectionChanged -= ExpireThis;
                    ((INotifyCollectionChanged)ic.Items).CollectionChanged += ExpireThis;
                    List<bool> checkeds = new List<bool>();
                    var cbs = from cbx in ic.Items.OfType<CheckBox>() select cbx;
                    foreach (CheckBox chex in cbs)
                    {
                        chex.Checked -= ExpireThis;
                        chex.Unchecked -= ExpireThis;
                        chex.Checked += ExpireThis;
                        chex.Unchecked += ExpireThis;
                    }
                    return;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    cb.SelectionChanged -= ExpireThis;
                    cb.SelectionChanged += ExpireThis;
                    return;
                case "System.Windows.Controls.TextBox":
                    TextBox tb = u as TextBox;
                    Panel p = tb.Parent as Panel;
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
                case "Xceed.Wpf.Toolkit.ColorPicker":
                    ColorPicker cp = u as ColorPicker;
                    cp.SelectedColorChanged -= ExpireThis;
                    cp.SelectedColorChanged += ExpireThis;
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
                case "System.Windows.Controls.Expander":
                    Expander exp = u as Expander;
                    exp.Expanded -= ExpireThis;
                    exp.Collapsed -= ExpireThis;
                    exp.Expanded += ExpireThis;
                    exp.Collapsed += ExpireThis;
                    return;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    tc.SelectionChanged -= ExpireThis;
                    tc.SelectionChanged += ExpireThis;
                    return;
                case "MahApps.Metro.Controls.ToggleSwitch":
                    ToggleSwitch ts = u as ToggleSwitch;
                    ts.IsCheckedChanged -= ExpireThis;
                    ts.IsCheckedChanged += ExpireThis;
                    return;
                case "MahApps.Metro.Controls.RangeSlider":
                    RangeSlider rs = u as RangeSlider;
                    rs.RangeSelectionChanged -= ExpireThis;
                    rs.RangeSelectionChanged += ExpireThis;
                    return;
                case "De.TorstenMandelkow.MetroChart.ChartBase":
                case "De.TorstenMandelkow.MetroChart.PieChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredBarChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredColumnChart":
                case "De.TorstenMandelkow.MetroChart.DoughnutChart":
                case "De.TorstenMandelkow.MetroChart.RadialGaugeChart":
                case "De.TorstenMandelkow.MetroChart.StackedBarChart":
                case "De.TorstenMandelkow.MetroChart.StackedColumnChart":
                    ChartBase chart = u as ChartBase;
                    chart.MouseUp -= ExpireThis;
                    chart.MouseUp += ExpireThis;
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
                case "System.Windows.Controls.DataGrid":
                    DataGrid datagrid = u as DataGrid;
                    datagrid.SelectedCellsChanged -= ExpireThis;
                    return;
                case "HumanUI.TrueOnlyButton":
                    TrueOnlyButton tob = u as TrueOnlyButton;
                    tob.PreviewMouseDown -= ExpireThis;
                    return;
                case "HumanUI.MDSliderElement":
                    MDSliderElement mds = u as MDSliderElement;
                    mds.PropertyChanged -= ExpireThis;
                    return;
                case "HumanUI.GraphMapperElement":
                    GraphMapperElement gme = u as GraphMapperElement;
                    gme.PropertyChanged -= ExpireThis;
                    return;
                case "HumanUI.HUI_GradientEditor":
                    HUI_GradientEditor hge = u as HUI_GradientEditor;
                    hge.PropertyChanged -= ExpireThis;
                    return;
                case "HumanUI.FilePicker":
                    FilePicker fp = u as FilePicker;
                    fp.PropertyChanged -= ExpireThis;
                    return;
                case "HumanUI.ClickableShapeGrid":
                    ClickableShapeGrid csg = u as ClickableShapeGrid;
                    switch (csg.clickMode)
                    {
                        case ClickableShapeGrid.ClickMode.ButtonMode:
                            csg.MouseUp -= ExpireThis;
                            csg.MouseDown -= ExpireThis;
                            return;
                        case ClickableShapeGrid.ClickMode.PickerMode:
                        case ClickableShapeGrid.ClickMode.ToggleMode:
                            csg.MouseUp -= ExpireThis;
                            return;
                     case ClickableShapeGrid.ClickMode.None:
                        default:
                            return;
                    }
                    
                case "System.Windows.Controls.Label":
                    Label l = u as Label;
                    return;
                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    lb.SelectionChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.ScrollViewer":
                    ScrollViewer sv = u as ScrollViewer;
                    ItemsControl ic = sv.Content as ItemsControl;
                    ((INotifyCollectionChanged)ic.Items).CollectionChanged -= ExpireThis;
                    List<bool> checkeds = new List<bool>();
                    var cbs = from cbx in ic.Items.OfType<CheckBox>() select cbx;
                    foreach (CheckBox chex in cbs)
                    {
                        chex.Checked -= ExpireThis;
                        chex.Unchecked -= ExpireThis;
                    }
                    return;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    cb.SelectionChanged -= ExpireThis;
                    return;
                case "System.Windows.Controls.TextBox":
                    TextBox tb = u as TextBox;
                    Panel p = tb.Parent as Panel;
                    List<Button> btns = p.Children.OfType<Button>().ToList<Button>();

                    foreach (Button btn0 in btns)
                    {
                        btn0.Click -= ExpireThis;
                    }
                    tb.TextChanged -= ExpireThis;
                    return;
                case "Xceed.Wpf.Toolkit.ColorPicker":
                    ColorPicker cp = u as ColorPicker;
                    cp.SelectedColorChanged -= ExpireThis;
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
                case "System.Windows.Controls.Expander":
                    Expander exp = u as Expander;
                    exp.Expanded -= ExpireThis;
                    exp.Collapsed -= ExpireThis;
                    return;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    tc.SelectionChanged -= ExpireThis;
                    return;
                case "MahApps.Metro.Controls.ToggleSwitch":
                    ToggleSwitch ts = u as ToggleSwitch;
                    ts.IsCheckedChanged -= ExpireThis;
                    return;
                case "MahApps.Metro.Controls.RangeSlider":
                    RangeSlider rs = u as RangeSlider;
                    rs.RangeSelectionChanged -= ExpireThis;
                    return;
                case "De.TorstenMandelkow.MetroChart.ChartBase":
                case "De.TorstenMandelkow.MetroChart.PieChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredBarChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredColumnChart":
                case "De.TorstenMandelkow.MetroChart.DoughnutChart":
                case "De.TorstenMandelkow.MetroChart.RadialGaugeChart":
                case "De.TorstenMandelkow.MetroChart.StackedBarChart":
                case "De.TorstenMandelkow.MetroChart.StackedColumnChart":
                    ChartBase chart = u as ChartBase;
                    chart.MouseUp -= ExpireThis;
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
            get { return new Guid("{D6BA0398-70A7-46E7-A068-274486EB0ACB}"); }
        }


        internal void updateMessage()
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


        internal bool AddEventsEnabled = true;

        //All the variable parameter stuff so that we can have an input for manually triggering an update w/o dispatch gymnastics.

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && index == 2 && Params.Input.Count==2) return true;
            return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && index == 2 && Params.Input.Count == 3) return true;
            return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_Boolean manualTrigger = new Param_Boolean();
            manualTrigger.NickName = "T";
            manualTrigger.Name = "Trigger";
            manualTrigger.Description = "An optional input parameter to force trigger an update (useful when the component is in manual mode)";
            manualTrigger.Optional = true;
            Params.RegisterInputParam(manualTrigger, index);
            return manualTrigger;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Input && index == 2;


        }

        public void VariableParameterMaintenance()
        {
            return;
        }
    }
}