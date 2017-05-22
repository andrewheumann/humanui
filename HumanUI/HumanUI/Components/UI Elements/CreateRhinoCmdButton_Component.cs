using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_IO.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HumanUIBaseApp;

namespace HumanUI.Components.UI_Elements
{




    /// <summary>
    /// This class extends the CreateButton component to add an extra event handler and an input for a Command Line script to be executed on button press. 
    /// </summary>
    /// <seealso cref="HumanUI.Components.UI_Elements.CreateButton_Component" />
    public class CreateRhinoCmdButton_Component : CreateButton_Component
    {

       


        /// <summary>
        /// Initializes a new instance of the CreateRhinoCmdButton_Component class.
        /// </summary>
        public CreateRhinoCmdButton_Component()
            : base("Create Rhino Command Button", "CmdButton",
                "Create a Special Button object to trigger a Rhino command.")
        {
        
        }

        
        
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Button Name", "N", "The text to display on the button", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Image Path", "I", "The image to display on the button.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Rhino Script", "S", "The command line script to execute on button press", GH_ParamAccess.item);
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Button";
            string imagePath = "";
            string localCmd = "";
            if (DA.GetData<string>("Rhino Script", ref localCmd))
            {
                commandString = localCmd;
            }

            bool hasText = DA.GetData<string>("Button Name", ref name);
            bool hasIcon = DA.GetData<string>("Image Path", ref imagePath);
            if (!hasText && !hasIcon) return;
            Button btn = new Button();
            SetupButton(name, imagePath, hasText, hasIcon, btn, bs);
            //Here's where we directly attach the ExecuteCommand event to the button. 
            btn.Click += ExecuteCommand;
            DA.SetData("Button", new UIElement_Goo(btn, name, InstanceGuid, DA.Iteration));

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateRhinoButton;

        string commandString = "";

        /// <summary>
        /// Executes the command when the button is pressed.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ExecuteCommand(object Sender, EventArgs e)
        {
            Rhino.RhinoApp.RunScript(commandString, false);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{58AEE14D-8214-4E76-8D51-3432CD30B3AC}");
    }
}