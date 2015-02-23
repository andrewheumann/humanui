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

namespace HumanUI
{
    class HUI_Util
    {
       public static void removeParent(UIElement child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return;
            var parentAsPanel = parent as Panel;
            if (parentAsPanel != null)
            {
                parentAsPanel.Children.Remove(child);
            }
        }

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


 

       public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
       {
           return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
       }


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


       static TextBox findTextBox(Panel p)
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
                       s.Value = (double)o;
                       return;
                   case "System.Windows.Controls.Button":
                       Button b = u as Button;
                       return;
                   case "System.Windows.Controls.Label":
                       Label l = u as Label;
                       l.Content = (string)o;
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
                      
                       //return cbi.Content;
                       return;
                   case "System.Windows.Controls.ListView":
                       ListView v = u as ListView;
                       var cbs = from cbx in v.Items.OfType<CheckBox>() select cbx;

                       string namesJoined = (string) o;
                       string[] checkBoxNames = namesJoined.Split(',');

                       
                       foreach (CheckBox chex in cbs)
                       {
                           bool isItChecked = false;
                           foreach (string cbn in checkBoxNames)
                           {
                              
                               if (chex.Content.ToString() == cbn)
                               {
                                   isItChecked = true;
                                   break;
                               }
                           }
                           chex.IsChecked = isItChecked;
                       }

                      // return String.Join(",", checkedVals);
                       return;
                   case "System.Windows.Controls.CheckBox":
                       CheckBox chb = u as CheckBox;
                       chb.IsChecked = (bool)o;
                       return;
                   case "System.Windows.Controls.RadioButton":
                       RadioButton rb = u as RadioButton;
                       rb.IsChecked = (bool)o;
                       return;
                   case "System.Windows.Controls.Image":
                       Image I = u as Image;
                       //set image source;
                       SetImageSource((string)o, I);
                       return;
                   default:
                       return;
               }
           }
           catch
           {
              
           }
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
               case "System.Windows.Controls.Label":
                   Label l = u as Label;
                   return l.Content;
               case "System.Windows.Controls.ListBox":
                   ListBox lb = u as ListBox;
                   Label lab = lb.SelectedItem as Label;
                   return lab.Content;
               case "System.Windows.Controls.TextBox":
                   TextBox tb = u as TextBox;
                   return tb.Text;
               case "System.Windows.Controls.ComboBox":
                   ComboBox cb = u as ComboBox;
                   Label cbi = cb.SelectedItem as Label;
                   return cbi.Content;
               case "System.Windows.Controls.ListView":
                   ListView v = u as ListView;
                   var cbs = from cbx in v.Items.OfType<CheckBox>() select cbx;
                   List<string> checkedVals = new List<string>();
                   foreach (CheckBox chex in cbs)
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


    }
}
