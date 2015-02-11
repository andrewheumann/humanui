using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Windows;

using System.Windows.Controls;

namespace HumanUI
{
    public class UIElement_Goo : GH_Goo<UIElement>
    {
        public UIElement element { get; set; }
        public string name { get; set; }
        public UIElement_Goo(UIElement _element, string _name)
        {
            element = _element;
            name = _name;

        }
        public override IGH_Goo Duplicate()
        {
            return new UIElement_Goo(element,name);
        }

        public override bool IsValid
        {
            get { return (element != null); }
        }

        public override string ToString()
        {
            if (element is Panel)
            {
                Panel p = element as Panel;
                return String.Format("Container UI Element {2}, Type {0}, {1:0} Children Elements", element.GetType().ToString(), p.Children.Count,name);

            }
            else
            {
                return String.Format("UI Element {1}, Type: {0}", element.GetType().ToString(),name);
            }
        }

        public override string TypeDescription
        {
            get { return "A UI element"; }
        }

        public override string TypeName
        {
            get { return "UI Element"; }
        }
    }
}
