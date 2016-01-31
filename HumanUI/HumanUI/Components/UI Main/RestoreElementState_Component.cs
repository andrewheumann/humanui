using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;

namespace HumanUI.Components.UI_Main 
{

    /// <summary>
    /// This component handles restoring saved states. It is a variable parameter component allowing the optional addition of an output to indicate when 
    /// restoration is complete.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    /// <seealso cref="Grasshopper.Kernel.IGH_VariableParameterComponent" />
    public class RestoreElementState_Component : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the RestoreElementState_Component class.
        /// </summary>
        public RestoreElementState_Component()
            : base("Restore Element States", "Restore",
                "Restore the saved states of UI elements",
                "Human UI", "UI Main")
        {
          //set up listener for changed parameters so that appropriate maintenance can be performed.
            Params.ParameterSourcesChanged += new GH_ComponentParamServer.ParameterSourcesChangedEventHandler(ParamSourcesChanged);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Restore", "R", "Set to true to restore the state. \nProbably not a good idea to leave this set to true while you're adding new states.", GH_ParamAccess.item,false);
            pManager.AddGenericParameter("Saved States", "SS", "The collection of saved states", GH_ParamAccess.item);
            pManager.AddTextParameter("State Name to restore", "N", "The name of the state to restore", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool restore = false;
            string setName = "";
            StateSet_Goo states = null;
            if (!DA.GetData<StateSet_Goo>("Saved States", ref states)) return;
            if (!DA.GetData<string>("State Name to restore", ref setName)) return;
            DA.GetData<bool>("Restore", ref restore);

            //if there's an output param, pass out the value of restore. 
            if (Params.Output.Count > 0)
            {
                
                DA.SetData(0, restore);
            }

            if (!restore)
            {
                return;
            }

            //retrieve the appropriate state from the state set dictionary
            State stateToRestore = states.states[setName];
            //restore state
            foreach (KeyValuePair<UIElement_Goo, object> elementState in stateToRestore.stateDict)
            {
           
                UIElement element = elementState.Key.element;
                
                //if it has a parent, it's a real element that exists - if the parent is null we are in a new document session and the original element
                //may not really exist any longer. 
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(element);
                
                //This is the situation when opening a definition fresh - we have to retrtieve the element based on the saved state's
                //component instance guid and output data index. Because there's no way to serialize a  UIElement we have to retrieve it
                // from the component actively creating it (or more precisely a new but identical instance of it)
                
             if (parent == null)
             {
                 Guid id = elementState.Key.instanceGuid;
                 int index = elementState.Key.index;

                 UIElement_Goo newGoo = SaveElementState_Component.getElementGoo(this.OnPingDocument(),id,index);
                 element = newGoo.element;
             }
                //Once we've got the element, try to set its value. 
                HUI_Util.TrySetElementValue(HUI_Util.extractBaseElement(element), elementState.Value);
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
                return Properties.Resources.RestoreState;
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{A6567BB1-37D1-46CB-AD10-594FF726299B}"); }
        }

        //if the user adds/removes a parameter, this is the event handler
        private void ParamSourcesChanged(object sender, GH_ParamServerEventArgs e)
        {
            // if where the user clicked was the output side, and it's the last of the output parameters
            if (e.ParameterSide == GH_ParameterSide.Output && e.ParameterIndex == Params.Output.Count - 1)
            {
                //create a new parameter
                IGH_Param new_Param = CreateParameter(GH_ParameterSide.Output, Params.Output.Count);
                Params.RegisterOutputParam(new_Param);
                VariableParameterMaintenance();
                Params.OnParametersChanged();

            }
        }

        //These overrides help handle the variable parameter behavior
        
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            //if it's the output side and it doesn't already have an output, you can insert a parameter.
            return side == GH_ParameterSide.Output && Params.Output.Count<1;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            //If it's the output side, you can remove a parameter.
            return side == GH_ParameterSide.Output;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            //the type of the parameter to create is boolean
            return new Grasshopper.Kernel.Parameters.Param_Boolean();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            //should always be ok to destroy it
            return true;
        }

        public void VariableParameterMaintenance()
        {
            // set the name/nickname info of the param. This is a loop to follow normal behavior, 
            // but there shouldn't ever be more than 1 output for this component.
            for (int i = 0; i < Params.Output.Count; i++)
            {
                Params.Output[i].Name = "Complete";
                Params.Output[i].NickName = "C";
                Params.Output[i].Description = "Optional output parameter to indicate restore execution";
                Params.Output[i].Access = GH_ParamAccess.item;
            }
        }
    }
}