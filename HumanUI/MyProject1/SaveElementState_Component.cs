using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Linq;
using GH_IO.Serialization;

namespace HumanUI
{
    public class SaveElementState_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SaveElementState_Component class.
        /// </summary>
        public SaveElementState_Component()
            : base("Save Element States", "SaveStates",
                "This component lets you save the states of seleted elements for later retrieval",
                "Human", "UI Main")
        {
            savedStates = new StateSet_Goo();
            serializedStates = new Dictionary<string, Dictionary<int, object>>();
        //    baseElems = new List<UIElement>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "All Controls and other elements belonging to the window", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The filter(s) for the elements you want to save.", GH_ParamAccess.list);
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

        private static StateSet_Goo savedStates;
        private static Dictionary<string, Dictionary<int, object>> serializedStates;
        private List<UIElement> baseElems;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool saveState = false;
            bool clearState = false;
            string stateName = "Unnamed State";
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> filters = new List<string>();

            if (!DA.GetDataList<KeyValuePair<string, UIElement_Goo>>("Elements", allElements)) return;
            DA.GetDataList<string>("Name Filter(s)", filters);
            if (!DA.GetData<string>("State Name", ref stateName)) return;
            DA.GetData<bool>("Save State", ref saveState);
            DA.GetData<bool>("Clear Saved States", ref clearState);

            List<UIElement> elems = new List<UIElement>();

            //populate elems based on filtering
            Dictionary<string, UIElement_Goo> elementDict = allElements.ToDictionary(pair => pair.Key, pair => pair.Value);
            
            foreach (string fil in filters)
            {
                elems.Add(elementDict[fil].element);
            }

            if (clearState)
            {
                savedStates.Clear();
            }
           

            //restore from serializedStates

            if (saveState)
            {
              //  baseElems.Clear();
                baseElems = new List<UIElement>();
                HUI_Util.extractBaseElements(elems, baseElems);
              
                State namedState = new State();
                //for each element
                foreach (UIElement u in baseElems)
                {

                    //get its state 
                    object state = HUI_Util.GetElementValue(u);
                    //add the element and its state to an element state dictionary
                    namedState.AddMember(u, state);
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
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8b6f72d3-6eff-4d25-8faa-62065ab7663e}"); }
        }
        /*
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Dictionary<string, State> states = savedStates.states;
            writer.SetInt32("StateCount", states.Count);
            int i = 0;
            report("StateCount="+states.Count.ToString());
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, states.Count.ToString());
            foreach(KeyValuePair<string,State> state in states){
                 GH_IWriter chunk = writer.CreateChunk("State", i);
                 chunk.SetString("StateName", state.Key);
                 State theState = state.Value;
                 int j = 0;
                 chunk.SetInt32("elementCount", theState.Count);
                 foreach (KeyValuePair<UIElement, object> elementState in theState)
                 {
                     GH_IWriter elementChunk = chunk.CreateChunk("ElementState", j);
                   //find index of uiElement
                     int index = baseElems.FindIndex(u => u == elementState.Key);
                     elementChunk.SetInt32("elementIndex", index);
                     elementChunk.SetString("elementType", elementState.Value.GetType().ToString());
                     elementChunk.SetString("elementValue", elementState.Value.ToString());

                     j++;
                 }
                 //chunk.

                 i++;
             }
            return base.Write(writer);
        }

        private void report(string str)
        {
            List<string> lines = new List<string>();
            lines.Add(str);
            System.IO.File.AppendAllLines(@"C:\users\aheumann\desktop\errorReadout.txt", lines);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            Dictionary<string, Dictionary<int, object>> stateSetDict = new Dictionary<string, Dictionary<int, object>>();
            int stateCount = 0;
            reader.TryGetInt32("StateCount", ref stateCount);
           
      
            for (int i = 0; i < stateCount; i++)
            {
                Dictionary<int, object> stateDict = new Dictionary<int, object>();
                GH_IReader chunk = reader.FindChunk("State",i);
                string stateName = chunk.GetString("StateName");
                int elementCount = chunk.GetInt32("elementCount");
                report(elementCount.ToString());
                for (int j = 0; j < elementCount; j++)
                {
                    GH_IReader elementChunk = chunk.FindChunk("ElementState",j);
                    int index = elementChunk.GetInt32("elementIndex");
                    string elementType = elementChunk.GetString("elementType");
                    string elementValueRaw = elementChunk.GetString("elementValue");
                    object actualValue;
                    report(elementType);
                    switch (elementType)
                    {
                        case "string":
                            actualValue = elementValueRaw;
                            break;
                        case "bool":
                            bool boolVal = false;
                            Boolean.TryParse(elementValueRaw,out boolVal);
                            actualValue = boolVal;
                            break;
                        case "int":
                            int intVal = -1;
                            Int32.TryParse(elementValueRaw, out intVal);
                            actualValue = intVal;
                            break;
                        default:
                            actualValue = null;
                            break;
                    }
                    stateDict.Add(index, actualValue);

                }

                stateSetDict.Add(stateName, stateDict);
            }

                return base.Read(reader);
        }
        */
    }
}