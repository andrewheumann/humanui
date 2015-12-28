using System;
using System.Collections.Generic;
using System.Collections;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using Grasshopper.Kernel.Types;
using System.Linq;
using GH_IO.Serialization;

namespace HumanUI.Components.UI_Main 
{
    /// <summary>
    /// This component lets you save the states of selected elements for later retrieval.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class SaveElementState_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SaveElementState_Component class.
        /// </summary>
        public SaveElementState_Component()
            : base("Save Element States", "SaveStates",
                "This component lets you save the states of selected elements for later retrieval",
                "Human", "UI Main")
        {
            savedStates = new StateSet_Goo();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "All Controls and other elements belonging to the window", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The optional filter(s) for the elements you want to save.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Save State", "S", "Set to true to save the current state of all selected elements", GH_ParamAccess.item, false);
            pManager.AddTextParameter("State Name", "N", "The name under which to save the state. If the name already exists, saved state will be overwritten", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Clear Saved States", "C", "Set to true to clear all saved states.", GH_ParamAccess.item, false);
      
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Saved States", "SS", @"The saved element states. Connect to a ""Restore State"" component to reinstate.", GH_ParamAccess.item);
            pManager.AddTextParameter("Saved State Names", "N", "The names of all currently saved states", GH_ParamAccess.list);
        }

        //We have to maintain a parallel set of "Shadow states" which are serializable with the document, so that we can have saved states persist on save. 
        private Dictionary<string, Dictionary<Tuple<Guid, int>, object>> savedShadowStates = new Dictionary<string, Dictionary<Tuple<Guid, int>, object>>();
       
        private StateSet_Goo savedStates;
        private List<UIElement> baseElems;
        private bool hasProperlyGrabbedShadowElements = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            try
            {
                if (!hasProperlyGrabbedShadowElements) //if we have not yet succeeded in retoring states from shadows
                {
                    hasProperlyGrabbedShadowElements = shadowToState(); //try restoring states from shadows
                }
            }
            catch (Exception e)
            {
                //hopefully this never happens.
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, String.Format("It threw an {0} error when trying to restore shadow states",e.ToString()));
            }

            bool allElementsHaveParents = true;
            //hacky hack fix for weird deserialization problem. It occasionally happens that elements are deserialized before they belong to a window - 
            //if this happens we have to get them from the shadow state a second time. 
            if (savedStates != null)
            {
                foreach (State state in savedStates.states.Values)
                {
                    foreach (UIElement_Goo goo in state.stateDict.Keys)
                    {
                        if(FindTopmostParent<HumanUIBaseApp.MainWindow>(goo.element) == null) allElementsHaveParents = false;
                       
                    }
                }
            }

            if (!allElementsHaveParents)
            {
                shadowToState();
            }

            bool saveState = false;
            bool clearState = false;
            string stateName = "Unnamed State";

            List<object> elementObjects = new List<object>();
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> elementFilters = new List<string>();

            if (!DA.GetDataList<object>("Elements", elementObjects)) return;
            if (!DA.GetData<string>("State Name", ref stateName)) return;
            DA.GetData<bool>("Save State", ref saveState);
            DA.GetData<bool>("Clear Saved States", ref clearState);

            bool hasFilters = DA.GetDataList<string>("Name Filter(s)", elementFilters);



     
            
            List<UIElement_Goo> filteredElements = new List<UIElement_Goo>();

            // This is necessary because sometimes elements will come in as UIElement_Goo,
            // and sometimes they will be wrapped Key-Value pairs from a dictionary. I think this 
            // latter case is now fairly rare - I think only the "Filter UI Element" component was outputting a dictionary
            // but I'm leaving it in for backwards compatibility.
            foreach (object o in elementObjects)
            {
                
                switch (o.GetType().ToString())
                {
                    case "HumanUI.UIElement_Goo":
                        UIElement_Goo goo = o as UIElement_Goo;
                        filteredElements.Add(goo);
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

                    filteredElements.Add(elementDict[fil]);
                }
                //if there are no filters, include all values. 
                if (elementFilters.Count == 0)
                {
                    foreach (UIElement_Goo u in elementDict.Values)
                    {
                        filteredElements.Add(u);
                    }
                }
            }





            //clear the saved state info
            if (clearState)
            {
                savedStates.Clear();
                savedShadowStates.Clear();
            }
           

        
            //if the user has specified that the state should be saved
            if (saveState)
            {
          
                baseElems = new List<UIElement>();

                //setting up a 1:1 correspondence between base elements and parent elements (UI_ElementGoo).
                foreach (UIElement_Goo u in filteredElements)
                {
                    baseElems.Add(HUI_Util.extractBaseElement(u.element));
                }

              
                State namedState = new State();
                //for each element
                for (int i = 0; i < baseElems.Count;i++ )
                {
                    UIElement u = baseElems[i];
                    UIElement_Goo parent = filteredElements[i];
                    //get its state 
                    object state = HUI_Util.GetElementValue(u);
                    //add the element and its state to an element state dictionary

                    
                    namedState.AddMember(parent, state);
                }
                savedStates.Add(stateName, namedState);

            }
            DA.SetData("Saved States", savedStates);
            DA.SetDataList("Saved State Names", savedStates.Names);

        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
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
                return Properties.Resources.SaveState;
            }
        }


        /// <summary>
        /// Write all required data for deserialization to an IO archive.
        /// </summary>
        /// <param name="writer">Object to write with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        public override bool Write(GH_IWriter writer)
        {
           
            //serialize the states
            
            GH_IWriter stateSetChunk = writer.CreateChunk("stateSetChunk");
            stateSetChunk.SetInt32("StateCount", savedStates.states.Count);
            int i = 0; //the state

            //for each state in saved states
            foreach (KeyValuePair<string,State> statePair in savedStates.states)
            {
                string stateName = statePair.Key;
                GH_IWriter StateChunk = stateSetChunk.CreateChunk("State", i);
                StateChunk.SetString("stateName", stateName);
               
                State state = statePair.Value;
                StateChunk.SetInt32("itemCount", state.stateDict.Count);
                int j=0;
                //Custom serialization logic 
                //for each element in the state
                foreach (KeyValuePair<UIElement_Goo, object> stateItem in state.stateDict)
                {
                    //get element and value
                    UIElement_Goo element = stateItem.Key;
                    object value = stateItem.Value;
                   
                    GH_IWriter stateItemChunk = StateChunk.CreateChunk("stateItem",j);
                    //this info is used to retrieve the dynamic UI element on reserialization, by grabbing the component with matching instance guid
                    //and getting the output item at the matching index.
                    stateItemChunk.SetString("ElementID", element.instanceGuid.ToString());
                    stateItemChunk.SetInt32("ElementIndex", element.index);

                    string stringValue = value.ToString();
                    string typeString = value.GetType().ToString();
                    //special case for lists of bool - all other element "states" are single items. This only applies to a checklist object.
                    if (value is List<bool>)
                    {

                        typeString = "LIST OF BOOL";
                        stringValue = HUI_Util.stringFromBools((List<bool>)value);
                    }
                    
                    //store the value and a hint as to its type
                    stateItemChunk.SetString("ElementValue", stringValue);
                    stateItemChunk.SetString("ElementValueType", typeString);
                 
                        j++; //counting up elements in state
                }
               

                i++; //counting up states
            }
            return base.Write(writer);
        }



        /// <summary>
        /// Read all required data for deserialization from an IO archive.
        /// </summary>
        /// <param name="reader">Object to read with.</param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        public override bool Read(GH_IReader reader)
        {
            // set up a "shadow" state set of only basic data types
            // check out that wackadoodle data type - it's a Dictionary<string, Dictionary<Tuple<Guid, int>, object>>  :-p
            Dictionary<string, Dictionary<Tuple<Guid, int>, object>> stateSet = new Dictionary<string, Dictionary<Tuple<Guid, int>, object>>();
            GH_IReader stateSetChunk = reader.FindChunk("stateSetChunk");
            int stateCount = stateSetChunk.GetInt32("StateCount");
            //for all the states in the archive
            for (int i = 0; i < stateCount; i++)
            {
                Dictionary<Tuple<Guid, int>, object> state = new Dictionary<Tuple<Guid, int>, object>();
                GH_IReader stateChunk = stateSetChunk.FindChunk("State", i);
                string stateName = stateChunk.GetString("stateName");
                int itemCount = stateChunk.GetInt32("itemCount");

                //for all the elements in the state
                for (int j = 0; j < itemCount; j++)
                {
                    
                        GH_IReader stateItemChunk = stateChunk.FindChunk("stateItem", j);
                        Guid elementID = new Guid(stateItemChunk.GetString("ElementID"));
                        int index = stateItemChunk.GetInt32("ElementIndex");
                        string elementValue = stateItemChunk.GetString("ElementValue");
                        string elementValueType = stateItemChunk.GetString("ElementValueType");
                        
                            Tuple<Guid, int> goo = new Tuple<Guid, int>(elementID, index);
                        object value = getValue(elementValue, elementValueType);
                        state.Add(goo, value);
                    
                }
                if (stateSet.ContainsKey(stateName))
                {
                    stateSet[stateName] = state;
                }
                else
                {
                    stateSet.Add(stateName, state);
                }
            }
            savedShadowStates = stateSet;



                return base.Read(reader);
        }

        /// <summary>
        /// Converts a live state set to a shadow state set, made up of only simple, serializable data types.
        /// </summary>
        void stateToShadow(){
            Dictionary<string, Dictionary<Tuple<Guid, int>, object>> stateSet = new Dictionary<string, Dictionary<Tuple<Guid, int>, object>>();

            //foreach state in savedStates
            foreach(KeyValuePair<string,State> s in savedStates.states)
            {
                Dictionary<Tuple<Guid, int>, object> shadowState = new Dictionary<Tuple<Guid, int>, object>();

                string stateName = s.Key;
                State state = s.Value;
                //foreach stateitem in s
                foreach(KeyValuePair<UIElement_Goo,object> stateItem in state.stateDict)
                {


                    Guid elementID = stateItem.Key.instanceGuid;
                    int index = stateItem.Key.index;
                    string elementValue = stateItem.Value.ToString();
                    string elementValueType = stateItem.Value.GetType().ToString();

                    Tuple<Guid, int> goo = new Tuple<Guid, int>(elementID, index);
                    object value = getValue(elementValue, elementValueType);
                    shadowState.Add(goo, value);

                }
                if (stateSet.ContainsKey(stateName))
                {
                    stateSet[stateName] = shadowState;
                }
                else
                {
                    stateSet.Add(stateName, shadowState);
                }
            }
            savedShadowStates = stateSet;
        }


        /// <summary>
        /// Converts a shadow state set, made up of only simple, serializable data types, to a live state set. 
        /// </summary>
        /// <returns>True on success</returns>
        bool shadowToState()
        {
            bool allElementsHaveParents = true;
            //for each saved shadow state
            foreach (KeyValuePair<string, Dictionary<Tuple<Guid, int>, object>> ShadowState in savedShadowStates)
            {
                //get the name and the shadow state item
                string stateName = ShadowState.Key;
                Dictionary<Tuple<Guid, int>, object> stateItem = ShadowState.Value;

                //create a new real state
                State state = new State();

                //for each shadow state uielement proxy
                foreach (Tuple<Guid, int> uiElemProxy in stateItem.Keys)
                {
                    //get the value and a reference to the object
                    object valueSet = stateItem[uiElemProxy];
                    UIElement_Goo goo = getElementGoo(this.OnPingDocument(),uiElemProxy.Item1, uiElemProxy.Item2);
                    //add the value and the elementGoo into the new real state
                    state.stateDict.Add(goo, valueSet);
                    //assumes that all elements, if properly deserialized and referenced back to the doc, have a parent element.
                    if (FindTopmostParent<HumanUIBaseApp.MainWindow>(goo.element) == null) allElementsHaveParents = false;
                }
                //add the re-constituted state into the global state dictionary
                savedStates.Add(stateName, state);

            }
            return allElementsHaveParents;
        }

        /// <summary>
        /// Gets the value of a serialized object from a string and the saved type.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="valueType">Data type of the value.</param>
        /// <returns></returns>
        object getValue(string value, string valueType)
        {
            switch (valueType)
            {
                  
                case "System.Double":
                    double dbl;
                    Double.TryParse(value,out dbl);
                    return dbl;
                case "System.Boolean":
                    bool bl;
                    Boolean.TryParse(value, out bl);
                    return bl;
                case "LIST OF BOOL":
                    return HUI_Util.boolsFromString(value);
                case "System.Drawing.Color":
                    string[] res = value.Split("=,]".ToCharArray());
                    int A, R, G, B;
                    Int32.TryParse(res[1],out A);
                    Int32.TryParse(res[3], out R);
                    Int32.TryParse(res[5], out G);
                    Int32.TryParse(res[7], out B);
                    return System.Drawing.Color.FromArgb(A, R, G, B);
                default:
                    return value;
            }
        }

        //Grab the elementgoo based on its guid and index
        public static UIElement_Goo getElementGoo(GH_Document doc, Guid id, int index)
        {
           
            IGH_Component comp = doc.FindComponent(id);
          
                return comp.Params.Output[0].VolatileData.AllData(true).ToArray()[index] as UIElement_Goo;
         
        }



        /// <summary>
        /// Recursive function to find the topmost parent of a dependencyobject (UI Element) matching a given type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The child element.</param>
        /// <returns></returns>
        public static T FindTopmostParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindTopmostParent<T>(parentObject);
        }




        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8b6f72d3-6eff-4d25-8faa-62065ab7663e}"); }
        }
       
    }
}