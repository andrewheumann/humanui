using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Grasshopper.Kernel;
using HelixToolkit.Wpf;
using Rhino.Geometry;

namespace HumanUI.Components.UI_Output
{
    public class Set3DViewPropertiesComponent : GH_Component
    {

        public Set3DViewPropertiesComponent() : base("Set 3D View Properties", "Set3DViewProps", "Additional controls for modifying a 3D view", "Human UI", "UI Output")
        {

        }


        public override Guid ComponentGuid => new Guid("{B3B5DD54-90C3-4FB1-B026-866E960AB942}");
        protected override Bitmap Icon => Properties.Resources.set3dViewProps;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("3D View", "V", "The 3D view to modify", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show View Cube", "VC", "Show the view cube", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Allow Pan", "AP", "Allow user to pan 3d view", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Allow Zoom", "AZ", "Allow user to zoom 3d view", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Allow Move", "AM", "Allow user to move 3d view", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Allow Rotation", "AR", "Allow user to rotate 3d view", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Perspective/Parallel", "Par", "Set to true for Parallel, false for Perspective", GH_ParamAccess.item);
            pManager.AddPointParameter("Camera Location", "CL", "The camera location", GH_ParamAccess.item);
            pManager.AddPointParameter("Camera Target", "CT", "The target for the camera", GH_ParamAccess.item);
            pManager.AddNumberParameter("Camera Transition Time", "CTT", "The amount of time it takes the camera position to update", GH_ParamAccess.item);
            Enumerable.Range(1, 9).ToList().ForEach(i => pManager[i].Optional = true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object o = null;
            bool showViewCube = true;
            bool allowPan = true;
            bool allowZoom = true;
            bool allowMove = true;
            bool allowRotation = true;
            bool isParallel = true;
            Point3d camLoc = Point3d.Unset;
            Point3d camTarget = Point3d.Unset;
            double camTransitionTime = 0;


            if (!DA.GetData<object>("3D View", ref o)) return;
            var hasShowViewCube = DA.GetData("Show View Cube", ref showViewCube);
            var hasAllowPan = DA.GetData("Allow Pan", ref allowPan);
            var hasAllowZoom = DA.GetData("Allow Zoom", ref allowZoom);
            var hasAllowMove = DA.GetData("Allow Move", ref allowMove);
            var hasAllowRotation = DA.GetData("Allow Rotation", ref allowRotation);
            var hasIsParallel = DA.GetData("Perspective/Parallel", ref isParallel);
            var hasCamLoc = DA.GetData("Camera Location", ref camLoc);
            var hasCamTarget = DA.GetData("Camera Target", ref camTarget);
            var hasCamTransitionTime = DA.GetData("Camera Transition Time", ref camTransitionTime);


            HelixViewport3D vp3 = HUI_Util.GetUIElement<HelixViewport3D>(o);

            if (hasShowViewCube) vp3.ShowViewCube = showViewCube;
            if (hasAllowPan) vp3.IsPanEnabled = allowPan;
            if (hasAllowZoom) vp3.IsZoomEnabled = allowZoom;
            if (hasAllowMove) vp3.IsMoveEnabled = allowMove;
            if (hasAllowRotation) vp3.IsRotationEnabled = allowRotation;
            if (hasIsParallel) vp3.Orthographic = isParallel;
            if (hasCamLoc && hasCamTarget)
            {
                var lookDir = camTarget - camLoc;
                vp3.SetView(new Point3D(camLoc.X, camLoc.Y, camLoc.Z), new Vector3D(lookDir.X, lookDir.Y, lookDir.Z), new Vector3D(0, 0, 1), hasCamTransitionTime ? camTransitionTime : 0);
            }
            
        

        }

    }
}
