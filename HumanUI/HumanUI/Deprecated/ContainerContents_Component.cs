using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace HumanUI
{
    /// <exclude />
    public class ContainerContents_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ContainerContents_Component class.
        /// </summary>
        public ContainerContents_Component()
            : base("Container Contents", "Contents",
                "Gets the child elements of a container element like a Stack or a Tab",
                "Human UI", "UI Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "All Controls and other elements belonging to the window", GH_ParamAccess.list);
            pManager.AddTextParameter("Name Filter(s)", "F", "The filter(s) for the containers you want to listen for.", GH_ParamAccess.list);
     
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Child Elements", "C", "The child elements inside this container", GH_ParamAccess.list);
            pManager.AddTextParameter("Element Names", "N", "The names of the child elements.", GH_ParamAccess.list);
     
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<KeyValuePair<string, UIElement_Goo>> allElements = new List<KeyValuePair<string, UIElement_Goo>>();
            List<string> filters = new List<string>();

            Dictionary<string, UIElement_Goo> resultDict = new Dictionary<string, UIElement_Goo>();

            if (!DA.GetDataList<KeyValuePair<string, UIElement_Goo>>("Elements", allElements)) return;
            if (!DA.GetDataList<string>("Name Filter(s)", filters)) return;

            //filter out objects in the dictionary
            Dictionary<string, UIElement_Goo> elementDict = allElements.ToDictionary(pair => pair.Key, pair => pair.Value);
            Dictionary<string, UIElement_Goo> results = new Dictionary<string, UIElement_Goo>();
            foreach (string fil in filters)
            {
                results.Add(fil, elementDict[fil]);
            }

            List<UIElement_Goo> childElements = new List<UIElement_Goo>();
            int i =0; //the parent element index
            
            foreach (UIElement_Goo ListObject in results.Values.ToList<UIElement_Goo>()) {
                int j = 0; //
            //get the container object
            UIElement container = HUI_Util.GetUIElement<UIElement>(ListObject);

            switch (container.GetType().ToString())
            {
                case "System.Windows.Controls.StackPanel":
                    StackPanel sp = container as StackPanel;
                    foreach (UIElement elem in sp.Children)
                    {

                        childElements.Add(new UIElement_Goo(elem, String.Format("StackPanel {0}/{2} {1}", i, j, HUI_Util.elemType(elem)), InstanceGuid, DA.Iteration));
                        j++;
                    }
                    break;
                case "System.Windows.Controls.TabControl":
                    TabControl tc = container as TabControl;
                   
                    foreach (object o in tc.Items) //foreach tab
                    {
                        if (o is TabItem)
                        {
                            TabItem ti = o as TabItem;
                            StackPanel spInside = ti.Content as StackPanel;
                            int k = 0; //the item it is in the tab
                            foreach (UIElement elem in spInside.Children)
                            {

                                childElements.Add(new UIElement_Goo(elem, String.Format("Tab Control {0}/{1}/{2} {3}", i, ti.Header.ToString(), HUI_Util.elemType(elem), k), InstanceGuid, DA.Iteration));
                                k++;
                            }

                            j++;
                        }
                        else if(o is UIElement)
                        {
                            UIElement elem = o as UIElement;
                            childElements.Add(new UIElement_Goo(elem, String.Format("Tab Control {0}/{1}", i, HUI_Util.elemType(elem)), InstanceGuid, DA.Iteration));
                        }
                    }
                    
                    break;
                default:
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more of the elements is not a valid container.");
                    break;
            }

            
            i++;
            }

            //get its children elements
            
            //output filtered child elements
            foreach (UIElement_Goo u in childElements)
            {

                HUI_Util.AddToDict(u, resultDict);
            }

            DA.SetDataList("Child Elements", resultDict);
            DA.SetDataList("Element Names", resultDict.Keys);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ContainerContents;

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{5215a6c0-8e04-4987-a87d-fef05f1a4a6b}");
    }
}