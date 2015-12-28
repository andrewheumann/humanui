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

namespace HumanUI
{

    /// <summary>
    /// A utility class containing shared methods utilized by several components.
    /// </summary>
   public static class HUI_Util
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

       /// <summary>
       /// Attempts to gets a UI element of type T from a given object.
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="o">The object.</param>
       /// <returns>The UI element</returns>
       public static T GetUIElement<T>(object o) where T: UIElement
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




       /// <summary>
       /// Utility method to convert from System.Drawing.Color to System.Windows.Media.Color
       /// </summary>
       /// <param name="color">The color as System.Drawing.Color.</param>
       /// <returns>Color as System.Windows.Media.Color</returns>
       public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
       {
           return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
       }

       /// <summary>
       /// Utility method to convert from System.Windows.Media.Color to System.Drawing.Color
       /// </summary>
       /// <param name="color">The color as System.Windows.Media.Color.</param>
       /// <returns>Color as System.Drawing.Color</returns>
       public static System.Drawing.Color ToSysColor(System.Windows.Media.Color color)
       {
           return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
       }



       /// <summary>
       /// Extracts the base element from a parent element. HUI elements are sometimes composites - a "Slider" in HUI is actually a panel containing a slider 
       /// and several text boxes, for instance. This method retrieves just the main object (slider, textbox, etc) itself.
       /// </summary>
       /// <param name="element">The element.</param>
       /// <returns>The base UI Element</returns>
       public static UIElement extractBaseElement(UIElement element)
       {
           if (element is Panel)
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

       /// <summary>
       /// Extracts the base elements from a list of parent elements. HUI elements are sometimes composites - a "Slider" in HUI is actually a panel 
       /// containing a slider and several text boxes, for instance. 
       /// </summary>
       /// <param name="element">The element.</param>
       /// <returns>The base UI Element</returns>
       public static void extractBaseElements(IEnumerable<UIElement> elements, List<UIElement> extractedElements)
       {
           foreach (UIElement elem in elements)
           {
               if (elem is Panel)
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
                       default:
                           // This recursive operation is in case we've wound up with a panel inside a panel or some such thing. 
                           // Not sure this ever actually occurs?
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


       /// <summary>
       /// Finds the text box in a panel.
       /// </summary>
       /// <param name="p">The p.</param>
       /// <returns>the text box</returns>
       public static TextBox findTextBox(Panel p)
       {
           foreach (UIElement u in p.Children)
           {
               if (u is TextBox)
               {
                   return u as TextBox;
               }
           }
           return null;
       }

       /// <summary>
       /// Finds the slider in a panel.
       /// </summary>
       /// <param name="p">The p.</param>
       /// <returns>The slider</returns>
       static Slider findSlider(Panel p)
       {
           foreach (UIElement u in p.Children)
           {
               if (u is Slider)
               {
                   return u as Slider;
               }
           }
           return null;
       }


       /// <summary>
       /// Adds a UI Element Goo to a result dictionary, generating an appropriate string for the key to avoid conflicts.
       /// </summary>
       /// <param name="e">The element.</param>
       /// <param name="resultDict">The result dictionary.</param>
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


       /// <summary>
       /// Tries to set the element value based on the data type of the UI Element
       /// </summary>
       /// <param name="u">The ui element.</param>
       /// <param name="o">The object.</param>
       static public void TrySetElementValue(UIElement u, object o)
       {
           try
           {
               switch (u.GetType().ToString())
               {
                   case "System.Windows.Controls.Slider":
                       Slider s = u as Slider;
                       
                       s.Value = (double)o;
                       return;
                   case "System.Windows.Controls.ListBox":
                       ListBox lb = u as ListBox;
                       lb.SelectedIndex = getSelectedItemIndex(lb,(string) o); 
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
                   default:
                       return;
               }
           }
           catch (Exception e)
           {
               System.Windows.Forms.MessageBox.Show(e.ToString());
           }
       }

       /// <summary>
       /// Casts an arbitrary object to an IGH_Goo GH data type.
       /// </summary>
       /// <param name="o">The object.</param>
       /// <returns></returns>
       public static IGH_Goo GetRightType(object o)
       {
           if (o == null) return new GH_ObjectWrapper(null);
           switch (o.GetType().ToString())
           {
               case "System.Boolean": 
                   return new GH_Boolean((bool)o);
               case "System.Int32": 
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



       /// <summary>
       /// Utility method to de-concatenate a list of bools from a string for deserialization purposes
       /// </summary>
       /// <param name="str">The string</param>
       /// <returns></returns>
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

       /// <summary>
       /// Utility method to concatenate a list of bools into a string for serialization purposes
       /// </summary>
       /// <param name="bs">The boolean values.</param>
       /// <returns></returns>
       public static string stringFromBools(List<bool> bs)
       {
           string str = "";
           foreach (bool b in bs)
           {
               str += b.ToString()+",";
           }
           return str;
       }

       /// <summary>
       /// Retrieves the custom string name for a UIelement of a particular type.
       /// </summary>
       /// <param name="elem">The elem.</param>
       /// <returns></returns>
       public static string elemType(UIElement elem)
       {
           if (elem is Panel)
           {

               Panel p = elem as Panel;
               //special elements that consist of a panel that wraps other stuff should make sure to
               //set the panel name to something distinctive so it can be differentiated here and elsewhere.
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
           //at this point the element is not a panel so we just get its string name
           string baseType = elem.GetType().ToString();
           //and clear out the "System.Windows.Controls." cruft.
           return baseType.Replace("System.Windows.Controls.", "");
          
       }


       /// <summary>
       /// Utility method to set the source of an Image object
       /// </summary>
       /// <param name="newImagePath">The new image path.</param>
       /// <param name="l">The image.</param>
       public static void SetImageSource(string newImagePath, Image l)
       {
           Uri filePath = new Uri(newImagePath);
           BitmapImage bi = new BitmapImage(filePath);
           l.Source = bi;
       }

       /// <summary>
       /// Gets the index of the selected item in a selector object.
       /// </summary>
       /// <param name="selector">The selector.</param>
       /// <param name="labelContent">Content of the label.</param>
       /// <returns>the index of the item</returns>
       static int getSelectedItemIndex(Selector selector, string labelContent)
       {
           foreach (object o in selector.Items)
           {
               if (o is Label)
               {
                   Label l = o as Label;
                   if (l.Content.ToString() == labelContent)
                   {
                       return selector.Items.IndexOf(o);
                   }
               }
           }
           return -1;
       }


       /// <summary>
       /// Tries to get the element value from a UIElement, based on its type name. 
       /// </summary>
       /// <param name="u">The ui element.</param>
       /// <returns></returns>
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
                   if (lab != null) { 
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
                    
                           checkeds.Add(chex.IsChecked==true);
                     
                   }
                  

                   return checkeds;
               case "System.Windows.Controls.TextBox":
                   TextBox tb = u as TextBox;
                   return tb.Text;
               case "System.Windows.Controls.ComboBox":
                   ComboBox cb = u as ComboBox;
                   Label cbi = cb.SelectedItem as Label;
                   if (cbi != null)
                   {
                       return cbi.Content;
                   }
                   else
                   {
                       return null;
                   }
               case "Xceed.Wpf.Toolkit.ColorPicker":
                   ColorPicker colP = u as ColorPicker;

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
               case "System.Windows.Controls.TabControl":
                   TabControl tc = u as TabControl;
                   TabItem ti = tc.SelectedItem as TabItem;
                   if (ti == null)
                   {
                       ti = tc.Items[0] as TabItem;
                   }
                   return ti.Header.ToString();
               default:
                   return null;
           }
       }


       /// <summary>
       /// Gets the selected index of a non-selector element.
       /// </summary>
       /// <param name="u">The u.</param>
       /// <returns></returns>
       static public object GetElementIndex(UIElement u)
       {
           switch (u.GetType().ToString())
           {
               
              case "System.Windows.Controls.ListBox":
                   ListBox lb = u as ListBox;
                   if(lb != null) {
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
                  
               case "System.Windows.Controls.TabControl":
                   TabControl tc = u as TabControl;
                   return tc.SelectedIndex;
               default:
                   return -1;
           }
       }

    }
}
