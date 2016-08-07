using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Xceed.Wpf.Toolkit;
using Grasshopper.Kernel;
using De.TorstenMandelkow.MetroChart;
using HumanUI.Components;
using System.Data;

namespace HumanUI
{

    /// <summary>
    /// A utility class containing shared methods utilized by several components.
    /// </summary>
    static class HUI_Util
    {
        /// <summary>
        /// Removes the parent from a child UI Element. Since an element cannot have multiple parents, it is necessary to
        /// remove it from the parent stackpanel in order to place it into a tab within that grid, for instance.
        /// </summary>
        /// <param name="child">The child.</param>
        public static void removeParent(UIElement child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return; // if it has no parent

            var parentAsPanel = parent as Panel;
            if (parentAsPanel != null)
            {
                parentAsPanel.Children.Remove(child);
            }
        }

        public static T GetUIElement<T>(object o) where T : UIElement
        {
            T elem = null;
            switch (o.GetType().ToString())
            {
                case "HumanUI.UIElement_Goo":
                    UIElement_Goo goo = o as UIElement_Goo;
                    elem = goo.element as T;
                    break;
                case "Grasshopper.Kernel.Types.GH_ObjectWrapper":
                    GH_ObjectWrapper wrapper = o as GH_ObjectWrapper;
                    KeyValuePair<string, UIElement_Goo> kvp = (KeyValuePair<string, UIElement_Goo>)wrapper.Value;
                    elem = kvp.Value.element as T;
                    break;
                default:
                    break;
            }
            return elem;
        }




        public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }


        public static System.Drawing.Color ToSysColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }



        public static UIElement extractBaseElement(UIElement element)
        {
            if (element is Panel && !(element is HumanUI.ClickableShapeGrid) && !(element is HumanUI.FilePicker))
            {

                Panel p = element as Panel;
                switch (p.Name)
                {
                    case "GH_Slider":
                        return findSlider(p);
                    case "GH_TextBox":
                    case "GH_TextBox_NoButton":
                        return findTextBox(p);
                    default:
                        return null;
                }

            }
            else
            {
                return element;
            }
        }


        public static void extractBaseElements(IEnumerable<UIElement> elements, List<UIElement> extractedElements)
        {
            foreach (UIElement elem in elements)
            {
                if (elem is Panel && !(elem is ClickableShapeGrid) && !(elem is FilePicker) && !(elem is HUI_GradientEditor))
                {

                    Panel p = elem as Panel;
                    switch (p.Name)
                    {
                        case "GH_Slider":
                            extractedElements.Add(findSlider(p));
                            break;
                        case "GH_TextBox":
                        case "GH_TextBox_NoButton":
                            extractedElements.Add(findTextBox(p));
                            break;
                        default: // WE MAY HAVE TO FORGET ABOUT GETTING STUFF OUT OF CONTAINERS (TABS ETC)
                            extractBaseElements(p.Children.Cast<UIElement>(), extractedElements);
                            break;
                    }

                }
                else
                {
                    extractedElements.Add(elem);
                }
            }
        }


        public static TextBox findTextBox(Panel p)
        {
            foreach (UIElement u in p.Children)
            {
                if (u is Panel)
                {
                    return findTextBox(u as Panel);
                }
                if (u is TextBox)
                {
                    return u as TextBox;
                }
            }
            return null;
        }

        static Slider findSlider(Panel p)
        {
            foreach (UIElement u in p.Children)
            {

                if (u is Slider)
                {
                    return u as Slider;
                }
                else if (u is Grid)
                {
                    foreach (UIElement gu in (u as Grid).Children)
                    {
                        if (gu is Slider)
                        {
                            return gu as Slider;
                        }
                    }
                }
            }
            return null;
        }


        static public void AddToDict(UIElement_Goo e, Dictionary<string, UIElement_Goo> resultDict)
        {
            int tryCount = 0;
            string keyName = e.name;
            while (resultDict.ContainsKey(keyName))
            {
                tryCount++;
                keyName = String.Format("{0} {1:0}", e.name, tryCount);

            }
            e.name = keyName;
            resultDict.Add(keyName, e);

        }


        static public void TrySetElementValue(UIElement u, object o)
        {
            try
            {
                switch (u.GetType().ToString())
                {
                    case "System.Windows.Controls.Slider":
                        Slider s = u as Slider;
                        //System.Windows.Forms.MessageBox.Show(o.GetType().ToString());

                        s.Value = (double)o;
                        return;
                    case "System.Windows.Controls.ListBox":
                        ListBox lb = u as ListBox;
                        lb.SelectedIndex = getSelectedItemIndex(lb, (string)o);
                        return;
                    case "System.Windows.Controls.TextBox":
                        TextBox tb = u as TextBox;
                        tb.Text = (string)o;
                        return;
                    case "System.Windows.Controls.ComboBox":
                        ComboBox cb = u as ComboBox;
                        cb.SelectedIndex = getSelectedItemIndex(cb, (string)o);
                        return;

                    case "Xceed.Wpf.Toolkit.ColorPicker":
                        ColorPicker colP = u as ColorPicker;
                        System.Drawing.Color sysCol = (System.Drawing.Color)o;
                        colP.SelectedColor = HUI_Util.ToMediaColor(sysCol);
                        return;
                    case "System.Windows.Controls.ScrollViewer":
                        //it's a checklist
                        ScrollViewer sv = u as ScrollViewer;
                        List<bool> valueList = (List<bool>)o;
                        ItemsControl ic = sv.Content as ItemsControl;
                        var cbs = from cbx in ic.Items.OfType<CheckBox>() select cbx;
                        int i = 0;
                        foreach (CheckBox chex in cbs)
                        {

                            chex.IsChecked = valueList[i];
                            i++;
                        }

                        return;
                    case "System.Windows.Controls.CheckBox":
                        CheckBox chb = u as CheckBox;
                        chb.IsChecked = (bool)o;
                        return;
                    case "System.Windows.Controls.RadioButton":
                        RadioButton rb = u as RadioButton;
                        rb.IsChecked = (bool)o;
                        return;
                    case "System.Windows.Controls.DataGrid":
                        DataGrid datagrid = u as DataGrid;
                        List<string> selectedRowContents = (List<string>)o;
                        try
                        {
                            DataView dv = datagrid.ItemsSource as DataView;
                            foreach(DataRowView drv in dv)
                            {
                                var items = drv.Row.ItemArray.Cast<string>().ToList();
                                items.RemoveAt(0); //get rid of hidden index column
                                bool selectRow = true;
                                for(int counter = 0; counter < items.Count; counter++)
                                {
                                    if (selectedRowContents[counter] != items[counter])
                                    {
                                        selectRow = false;
                                        break;
                                    }
                                }
                                if (selectRow) datagrid.SelectedItem = drv;
                            }
                        }
                        catch
                        {

                        }
                        return;
                    case "HumanUI.MDSliderElement":
                        MDSliderElement mds = u as MDSliderElement;
                        mds.SliderPoint = (Rhino.Geometry.Point3d)o;
                        return;
                    case "HumanUI.GraphMapperElement":
                        GraphMapperElement gme = u as GraphMapperElement;

                        gme.SetByCurve((Rhino.Geometry.NurbsCurve)o);
                        return;
                    case "HumanUI.HUI_GradientEditor":
                        HUI_GradientEditor hge = u as HUI_GradientEditor;
                        HUI_Gradient gradient = HUI_Gradient.FromString((string)o);
                        hge.Gradient = gradient;
                        return;
                    case "HumanUI.FilePicker":
                        FilePicker fp = u as FilePicker;
                        fp.Path = (string)o;
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public static IGH_Goo GetRightType(object o)
        {
            if (o == null) return new GH_ObjectWrapper(null);
            switch (o.GetType().ToString())
            {
                case "System.Boolean":
                    return new GH_Boolean((bool)o);
                case "System.Int32":
                    return new GH_Integer((int)o);
                case "System.Double":
                case "System.Single":
                    return new GH_Number((double)o);
                case "System.String":
                    return new GH_String((string)o);
                case "System.Drawing.Color":
                    return new GH_Colour((System.Drawing.Color)o);

                default:
                    return new GH_ObjectWrapper(o);


            }
        }


        public static List<bool> boolsFromString(string str)
        {
            List<bool> bools = new List<bool>();

            string[] strs = str.Split(',');
            foreach (string s in strs)
            {
                bool bl;
                Boolean.TryParse(s, out bl);
                bools.Add(bl);
            }
            return bools;
        }

        public static string stringFromBools(List<bool> bs)
        {
            string str = "";
            foreach (bool b in bs)
            {
                str += b.ToString() + ",";
            }
            return str;
        }

        public static string elemType(UIElement elem)
        {
            if (elem is Panel)
            {

                Panel p = elem as Panel;
                switch (p.Name)
                {
                    case "GH_Slider":
                        foreach (UIElement u in p.Children)
                        {
                            if (u is Label)
                            {
                                Label name = u as Label;
                                return "Slider " + name.Content.ToString();
                            }
                        }
                        break;
                    case "GH_TextBox":
                    case "GH_TextBox_NoButton":
                        return "Text Box";
                    default:

                        break;
                }

            }
            string baseType = elem.GetType().ToString();
            return baseType.Replace("System.Windows.Controls.", "");

        }

        internal static string stringFromStrings(List<string> values)
        {
            return string.Join("|", values);
        }

        public static void SetImageSource(string newImagePath, Image l)
        {
            Uri filePath = new Uri(newImagePath);
            BitmapImage bi = new BitmapImage(filePath);
            l.Source = bi;
        }

        static int getSelectedItemIndex(Selector selector, string labelContent)
        {
            foreach (object o in selector.Items)
            {
                if (o is TextBlock)
                {
                    TextBlock l = o as TextBlock;
                    if (l.Text == labelContent)
                    {
                        return selector.Items.IndexOf(o);
                    }
                }
            }
            return -1;
        }




        static public object GetElementValue(UIElement u)
        {
            switch (u.GetType().ToString())
            {
                case "System.Windows.Controls.Slider":
                    Slider s = u as Slider;
                    return s.Value;
                case "System.Windows.Controls.Button":
                    Button b = u as Button;
                    return (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed) && b.IsMouseOver;
                case "HumanUI.TrueOnlyButton":
                    TrueOnlyButton tob = u as TrueOnlyButton;
                    return (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed) && tob.IsMouseOver;
                case "System.Windows.Controls.Label":
                    Label l = u as Label;
                    return l.Content;
                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    Label lab = lb.SelectedItem as Label;
                    if (lab != null)
                    {
                        return lab.Content;
                    }
                    else
                    {
                        return null;
                    }
                case "System.Windows.Controls.ScrollViewer":
                    ScrollViewer sv = u as ScrollViewer;
                    ItemsControl ic = sv.Content as ItemsControl;
                    List<bool> checkeds = new List<bool>();
                    var cbs = from cbx in ic.Items.OfType<CheckBox>() select cbx;
                    foreach (CheckBox chex in cbs)
                    {

                        checkeds.Add(chex.IsChecked == true);

                    }


                    return checkeds;
                case "System.Windows.Controls.TextBox":
                    TextBox tb = u as TextBox;
                    return tb.Text;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    TextBlock cbi = cb.SelectedItem as TextBlock;
                    if (cbi != null)
                    {
                        return cbi.Text;
                    }
                    else
                    {
                        return null;
                    }
                case "Xceed.Wpf.Toolkit.ColorPicker":
                    ColorPicker colP = u as ColorPicker;

                    //return cbi.Content;
                    return HUI_Util.ToSysColor(colP.SelectedColor);
                case "System.Windows.Controls.ListView":
                    ListView v = u as ListView;
                    var cbxs = from cbx in v.Items.OfType<CheckBox>() select cbx;
                    List<string> checkedVals = new List<string>();
                    foreach (CheckBox chex in cbxs)
                    {
                        if (chex.IsChecked == true)
                        {
                            checkedVals.Add(chex.Content.ToString());
                        }
                    }

                    return String.Join(",", checkedVals);
                case "System.Windows.Controls.CheckBox":
                    CheckBox chb = u as CheckBox;
                    return chb.IsChecked;
                case "System.Windows.Controls.RadioButton":
                    RadioButton rb = u as RadioButton;
                    return rb.IsChecked;
                case "System.Windows.Controls.Image":
                    Image img = u as Image;

                    return img.Source.ToString();
                case "System.Windows.Controls.Expander":
                    Expander exp = u as Expander;
                    return exp.IsExpanded;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    TabItem ti = tc.SelectedItem as TabItem;
                    if (ti == null)
                    {
                        ti = tc.Items[0] as TabItem;
                    }
                    return ti.Header.ToString();

                case "De.TorstenMandelkow.MetroChart.ChartBase":
                case "De.TorstenMandelkow.MetroChart.PieChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredBarChart":
                case "De.TorstenMandelkow.MetroChart.ClusteredColumnChart":
                case "De.TorstenMandelkow.MetroChart.DoughnutChart":
                case "De.TorstenMandelkow.MetroChart.RadialGaugeChart":
                case "De.TorstenMandelkow.MetroChart.StackedBarChart":
                case "De.TorstenMandelkow.MetroChart.StackedColumnChart":
                    ChartBase chart = u as ChartBase;
                    ChartItem selectedItem = chart.SelectedItem as ChartItem;

                    if (selectedItem != null)
                    {
                        string response = "";
                        if (!String.IsNullOrEmpty(selectedItem.ClusterCategory))
                        {
                            response += selectedItem.ClusterCategory + ": ";
                        }
                        response += selectedItem.Category;
                        return response;
                    }
                    else
                    {
                        return null;
                    }
                case "System.Windows.Controls.DataGrid":
                    DataGrid datagrid = u as DataGrid;
                    var SelectedItem = datagrid.SelectedItem;
                    //System.Data.DataView dv = datagrid.ItemsSource as System.Data.DataView;
                    List<string> result = new List<string>();
                    try
                    {
                        System.Data.DataRowView drv = SelectedItem as System.Data.DataRowView;
                        result = drv.Row.ItemArray.Cast<string>().ToList();
                        result.RemoveAt(0);
                    }
                    catch
                    {

                    }

                    return result;

                case "HumanUI.MDSliderElement":
                    MDSliderElement mds = u as MDSliderElement;

                    return mds.SliderPoint;
                case "HumanUI.GraphMapperElement":
                    GraphMapperElement gme = u as GraphMapperElement;
                    return gme.GetCurve().ToNurbsCurve();
                case "HumanUI.HUI_GradientEditor":
                    HUI_GradientEditor hge = u as HUI_GradientEditor;
                    return hge.Gradient.ToString();
                case "HumanUI.FilePicker":
                    FilePicker fp = u as FilePicker;
                    return fp.Path;
                case "HumanUI.ClickableShapeGrid":
                    ClickableShapeGrid csg = u as ClickableShapeGrid;
                    return csg.SelectedStates;
                default:
                    return null;
            }
        }

        internal static List<string> stringsFromString(string value)
        {
            return value.Split('|').ToList();
        }

        static public object GetElementIndex(UIElement u)
        {
            switch (u.GetType().ToString())
            {

                case "System.Windows.Controls.ListBox":
                    ListBox lb = u as ListBox;
                    if (lb != null)
                    {
                        return lb.SelectedIndex;
                    }
                    else
                    {
                        return -1;
                    }
                case "System.Windows.Controls.ScrollViewer":
                    ScrollViewer sv = u as ScrollViewer;
                    ItemsControl ic = sv.Content as ItemsControl;
                    List<int> checkeds = new List<int>();
                    var cbs = from cbx in ic.Items.OfType<CheckBox>() select cbx;
                    int i = 0;
                    foreach (CheckBox chex in cbs)
                    {

                        if (chex.IsChecked == true)
                        {
                            checkeds.Add(i);
                        }
                        i++;
                    }

                    return checkeds;
                case "HumanUI.ClickableShapeGrid":
                    ClickableShapeGrid csg = u as ClickableShapeGrid;
                    List<int> selectedInds = new List<int>();
                    int j = 0;
                    foreach (bool b in csg.SelectedStates)
                    {

                        if (b)
                        {
                            selectedInds.Add(j);
                        }
                        j++;
                    }

                    return selectedInds;
                case "System.Windows.Controls.ComboBox":
                    ComboBox cb = u as ComboBox;
                    if (cb != null)
                    {
                        return cb.SelectedIndex;
                    }
                    else
                    {
                        return -1;
                    }
                case "System.Windows.Controls.DataGrid":
                    DataGrid datagrid = u as DataGrid;
                    int selectedRow = -1;
                    try
                    {
                        DataRowView drv = datagrid.SelectedItem as DataRowView;
                        string indexName = (string)drv.Row.ItemArray[0];
                        selectedRow = Int32.Parse(indexName);
                    }
                    catch { }
                    return selectedRow;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = u as TabControl;
                    return tc.SelectedIndex;
                default:
                    return -1;

            }
        }


        public static List<IGH_ActiveObject> SourcesRecursive(IGH_Param p, List<IGH_ActiveObject> possibleObjects)
        {
            List<IGH_ActiveObject> results = new List<IGH_ActiveObject>();
            var sources = possibleObjects.Where(po => p.DependsOn(po));

            var ParamSources = sources.Where(s => s is IGH_Param).Cast<IGH_Param>().ToList();

            var ComponentSources = sources.Where(s => s is IGH_Component).Cast<IGH_Component>().ToList();

            foreach (IGH_Param param in ParamSources)
            {
                results.Add(param);
                results.AddRange(SourcesRecursive(param, possibleObjects));

            }

            foreach (IGH_Component component in ComponentSources)
            {
                results.Add(component);
                component.Params.Input.ForEach(input => results.AddRange(SourcesRecursive(input, possibleObjects)));
            }
            return results;

        }

    }
}
