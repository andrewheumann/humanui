using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel.Types;
using System.Windows;

namespace HumanUI
{


    /// <summary>
    /// GH_Goo compatible data type containing a collection of States in a dictionary
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.Types.GH_Goo{System.Collections.Generic.Dictionary{System.String,HumanUI.State}}" />
    public class StateSet_Goo : GH_Goo<Dictionary<string, State>>
    {
        public Dictionary<string, State> states { get; set; }
        public int Count => states.Count;

        public StateSet_Goo()
        {
             states = new Dictionary<string, State>(); 
        }

        public StateSet_Goo(Dictionary<string, State> _states)
        {
            states = _states;
        }

        public void Add(string _name, State _state)
        {
            if (states.ContainsKey(_name))
            {
                states.Remove(_name);
            }
            states.Add(_name, _state);
        }


        public override IGH_Goo Duplicate()
        {
            return new StateSet_Goo(states);
        }

        public override bool IsValid => states!=null;

        public override string ToString()
        {
            if (states.Count < 1)
            {
                return "Empty State Set";
            }
            string[] stateNames = states.Keys.ToArray<string>();
            string stateNameString = String.Join("\n", stateNames);
            return String.Format("State Set: \n{0}",stateNameString);
        }

        public void Clear()
        {
            states.Clear();
        }

        public string[] Names => states.Keys.ToArray<string>();

        public override string TypeDescription => "A collection of saved interface states";

        public override string TypeName => "UI Element State Collection";
    }


    /// <summary>
    /// Represents a collection of elements and their state objects (number for slider, text for label, etc.)
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{HumanUI.UIElement_Goo,System.Object}" />
    public class State : Dictionary<UIElement_Goo, object>
    {
        public  Dictionary<UIElement_Goo, object> stateDict {get; set;}
        public State()
        {
            stateDict = new Dictionary<UIElement_Goo, object>();
        }

        public void AddMember(UIElement_Goo u,object o)
        {
            if (stateDict.ContainsKey(u))
            {
                stateDict.Remove(u);
            }
            stateDict.Add(u, o);
        }

        public override string ToString()
        {
            return base.ToString()+stateDict.Count.ToString();
        }


    }
}
