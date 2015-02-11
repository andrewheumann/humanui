using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Rhino.Geometry;


namespace HumanUI
{
    public class _3DViewModel
    {
         public Model3D Model { get; set; }
         public List<System.Drawing.Color> cols { get; set; }


         public _3DViewModel(List<Mesh> meshes, List<Material> mats)
         {
             setupModel(meshes, mats);
         }

        public _3DViewModel(List<Mesh> meshes, List<System.Drawing.Color> colors)
        {
            List<Material> mats = new List<Material>();
            for (int i = 0; i < meshes.Count; i++)
            {
                mats.Add(MaterialHelper.CreateMaterial(HUI_Util.ToMediaColor(colors[i % colors.Count])));
            }
             setupModel(meshes, mats);
        }


        void setupModel(List<Mesh> meshes, List<Material> mats)
        {

            var modelGroup = new Model3DGroup();

            int i = 0;
            foreach (Mesh m in meshes)
            {
                var meshBuilder = new MeshBuilder(false, false);
                m.Faces.ConvertQuadsToTriangles();
                List<Point3D> ptsToAppend = new List<Point3D>();
                List<int> indicesToAppend = new List<int>();
                //  List<Vector3D> normsToAppend = new List<Vector3D>();

                foreach (Point3f p in m.Vertices)
                {
                    ptsToAppend.Add(new Point3D(p.X, p.Y, p.Z));
                }

                foreach (MeshFace mf in m.Faces)
                {

                    int[] inds = new int[] { mf.A, mf.B, mf.C };
                    indicesToAppend.AddRange(inds);


                }

                meshBuilder.Append(ptsToAppend, indicesToAppend);

                var mesh = meshBuilder.ToMesh(true);
                if (mats.Count > 0)
                {
                    var material = mats[i % mats.Count];
                    modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material });
                }
                else
                {
                    modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh});
                }
                i++;



               

            }




            this.Model = modelGroup;

        }

    }
}
