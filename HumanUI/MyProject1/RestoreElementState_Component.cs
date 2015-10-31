using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;

namespace HumanUI
{
    public class RestoreElementState_Component : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the RestoreElementState_Component class.
        /// </summary>
        public RestoreElementState_Component()
            : base("Restore Element States", "Restore",
                "Restore the saved states of UI elements",
                "Human", "UI Main")
        {
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


            if (Params.Output.Count > 0)
            {
                
                DA.SetData(0, restore);
            }

            if (!restore)
            {
                return;
            }
            State stateToRestore = states.states[setName];
            //restore state
            foreach (KeyValuePair<UIElement_Goo, object> elementState in stateToRestore.stateDict)
            {
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, string.Format("Setting element {0} to {1}", HUI_Util.extractBaseElement(elementState.Key.element).ToString(), elementState.Value));
                UIElement element = elementState.Key.element;
                
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(element);
         //    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Parent is: "+parent.ToString());
                
             if (parent == null)
             {
                 Guid id = elementState.Key.instanceGuid;
                 int index = elementState.Key.index;

                 UIElement_Goo newGoo = SaveElementState_Component.getElementGoo(this.OnPingDocument(),id,index);
                 element = newGoo.element;
             }

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

        private void ParamSourcesChanged(object sender, GH_ParamServerEventArgs e)
        {
            if (e.ParameterSide == GH_ParameterSide.Output && e.ParameterIndex == Params.Output.Count - 1)
            {
                IGH_Param new_Param = CreateParameter(GH_ParameterSide.Output, Params.Output.Count);
                Params.RegisterOutputParam(new_Param);
                VariableParameterMaintenance();
                Params.OnParametersChanged();

            }
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Output && Params.Output.Count<1;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Output;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Grasshopper.Kernel.Parameters.Param_Boolean();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
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