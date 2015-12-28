using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using HelixToolkit.Wpf;

using System.Windows.Media.Media3D;

namespace HumanUI.Components.UI_Output
{
    /// <summary>
    /// A special version of the set3DView component that accepts textures. 
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.GH_Component" />
    public class Set3DViewTex_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Set3DViewTex_Component class.
        /// </summary>
        public Set3DViewTex_Component()
            : base("Set 3D View Textured", "Set3DViewTex",
                "Allows you to modify the contents of an existing 3D view.",
                "Human", "UI Output")
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
            pManager.AddTextParameter("Mesh Texture", "T", "The textures to display each mesh", GH_ParamAccess.list);
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
            List<string> texBitmap = new List<string>();
            List<System.Drawing.Color> cols = new List<System.Drawing.Color>();
            if (!DA.GetData<object>("3D View", ref o)) return;
            DA.GetDataList<Mesh>("Mesh to display", m);

            bool hasTexture = DA.GetDataList<string>("Mesh Texture", texBitmap);
            bool hasColor = DA.GetDataList<System.Drawing.Color>("Mesh Colors", cols);


            HelixViewport3D vp3 = HUI_Util.GetUIElement<HelixViewport3D>(o);
            ModelVisual3D mv3 = GetModelVisual3D(vp3);
            List<ModelVisual3D> mv3s = GetModels(vp3);
            List<Material> mats = new List<Material>();

   
            //empty out the helixviewport
            vp3.Children.Clear();
            vp3.Children.Add(new SunLight());

            if (!hasColor && !hasTexture) //if user has not specified either color or texture
            {
                //for all the models
                foreach (ModelVisual3D mv30 in mv3s)
                {
                    Model3DGroup model = mv30.Content as Model3DGroup;
                    foreach (Model3D mod in model.Children)
                    {
                        if (mod is GeometryModel3D)
                        {
                            GeometryModel3D geom = mod as GeometryModel3D;
                            //extract the current material settings
                            mats.Add(geom.Material);
                        }
                    }
                }
                //pass in the new mesh with the existing materials
                mv3.Content = new _3DViewModel(m, mats).Model;
            }
            else if(!hasTexture)
            {
                //pass in the new mesh with new colors
                mv3.Content = new _3DViewModel(m, cols).Model;
            }
            else
            {
                //pass in the new mesh with new textures
                mv3.Content = new _3DViewModel(m, texBitmap).Model;
            }



            //add the model back into the viewport
            vp3.Children.Add(mv3);


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
                return Properties.Resources.Set3dView;
            }
        }

        /// <summary>
        /// Extracts the ModelVisual3D from the HelixViewport
        /// </summary>
        /// <param name="vp3">The VP3.</param>
        /// <returns>A ModelVisual3D</returns>
        ModelVisual3D GetModelVisual3D(HelixViewport3D vp3)
        {
            foreach (Visual3D v in vp3.Children)
            {
                if (v is ModelVisual3D)
                {
                    return v as ModelVisual3D;
                }
            }
            return null;

        }

        /// <summary>
        /// Extracts a list of ModelVisual3D from a given HelixViewport
        /// </summary>
        /// <param name="vp3">The Helix Viewport 3d.</param>
        /// <returns>a list of ModelVisual3D </returns>
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
        public override Guid ComponentGuid
        {
            get { return new Guid("{47D12D28-2711-435A-A445-DD4016EBB363}"); }
        }
    }
}