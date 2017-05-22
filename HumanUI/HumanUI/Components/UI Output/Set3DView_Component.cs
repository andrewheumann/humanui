using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using HelixToolkit.Wpf;

using System.Windows.Media.Media3D;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// Set the contents of an existing 3D viewport
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class Set3DView_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Set3DView_Component class.
        /// </summary>
        public Set3DView_Component()
            : base("Set 3D View", "Set3DView",
                "Allows you to modify the contents of an existing 3D view.",
                "Human UI", "UI Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("3D View", "V", "The 3D view to modify", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh to display", "M", "The mesh(es) to display in the viewport", GH_ParamAccess.list);
            pManager.AddColourParameter("Mesh Colors", "C", "The color with which to display the mesh.", GH_ParamAccess.list);
            pManager[2].Optional = true;
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
            object o = null;
            List<Mesh> m = new List<Mesh>();
            List<System.Drawing.Color> cols = new List<System.Drawing.Color>();
            if(!DA.GetData<object>("3D View", ref o)) return;
            DA.GetDataList<Mesh>("Mesh to display", m);
           
            //Get the 3d viewport object out of the input object
            HelixViewport3D vp3 = HUI_Util.GetUIElement<HelixViewport3D>(o);
           
            ModelVisual3D mv3 = GetModelVisual3D(vp3);
            List<ModelVisual3D> mv3s = GetModels(vp3);
            List<Material> mats = new List<Material>();

                  vp3.Children.Clear();
            vp3.Children.Add(new SunLight());

            if (!DA.GetDataList<System.Drawing.Color>("Mesh Colors", cols))
            {

                foreach (ModelVisual3D mv30 in mv3s)
                {
                    Model3DGroup model = mv30.Content as Model3DGroup;
                    foreach (Model3D mod in model.Children)
                    {
                        if (mod is GeometryModel3D)
                        {
                            GeometryModel3D geom = mod as GeometryModel3D;
                            mats.Add(geom.Material);
                        }
                    }
                }
                mv3.Content = new _3DViewModel(m, mats).Model;
            } else {
                mv3.Content = new _3DViewModel(m, cols).Model;
            }

      

            
            vp3.Children.Add(mv3);

          
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Set3dView;

        ModelVisual3D GetModelVisual3D(HelixViewport3D vp3){
           foreach(Visual3D v in vp3.Children){
               if (v is ModelVisual3D)
               {
                   return v as ModelVisual3D;
               }
           }
           return null;

        }

        List<ModelVisual3D> GetModels(HelixViewport3D vp3)
        {
            List<ModelVisual3D> models = new List<ModelVisual3D>();
            foreach (Visual3D v in vp3.Children)
            {
                if (v is ModelVisual3D)
                {
                    models.Add(v as ModelVisual3D);
                }

            }
            return models;

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{3472130d-fc0e-409d-9295-f93ecaf1afb5}");
    }
}