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
             setupModel(meshes, mats,false);
         }

        public _3DViewModel(List<Mesh> meshes, List<System.Drawing.Color> colors)
        {
            List<Material> mats = new List<Material>();
            for (int i = 0; i < meshes.Count; i++)
            {
                mats.Add(MaterialHelper.CreateMaterial(HUI_Util.ToMediaColor(colors[i % colors.Count])));
            }
             setupModel(meshes, mats,false);
        }

        public _3DViewModel(List<Mesh> meshes, List<string> bitmaps)
        {
            List<Material> mats = new List<Material>();
            for (int i = 0; i < meshes.Count; i++)
            {
           
                mats.Add(MaterialHelper.CreateImageMaterial(bitmaps[i % bitmaps.Count],1,UriKind.Absolute,false));
            }
            setupModel(meshes, mats,true);
        }

        void setupModel(List<Mesh> meshes, List<Material> mats,bool useTextures)
        {

            var modelGroup = new Model3DGroup();

            int i = 0;
            foreach (Mesh m in meshes)
            {
                var meshBuilder = new MeshBuilder(false, useTextures);
                m.Faces.ConvertQuadsToTriangles();
                List<Point3D> ptsToAppend = new List<Point3D>();
                List<int> indicesToAppend = new List<int>();
                //  List<Vector3D> normsToAppend = new List<Vector3D>();

                foreach (Point3f p in m.Vertices)
                {
                    ptsToAppend.Add(new Point3D(p.X, p.Y, p.Z));
                }
                if (useTextures)
                {
                    //special coordinates to keep from autonormalizing texcoords
                    for (int j = 0; j < 4; j++)
                    {
                        ptsToAppend.Add(new Point3D(0, 0, 0));
                    }
                }
                foreach (MeshFace mf in m.Faces)
                {

                    int[] inds = new int[] { mf.A, mf.B, mf.C };
                    indicesToAppend.AddRange(inds);


                }

              //  m.TextureCoordinates.NormalizeTextureCoordinates();
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;
                List<System.Windows.Point> texCoords = new List<System.Windows.Point>();
               foreach(Point2f texPt in m.TextureCoordinates.ToList()){
                   if (texPt.X > maxX) maxX = texPt.X;
                   if (texPt.X < minX) minX = texPt.X;
                   if (texPt.Y > maxY) maxY = texPt.Y;
                   if (texPt.Y < minY) minY = texPt.Y;
                   texCoords.Add(new System.Windows.Point(texPt.X, 1.0-texPt.Y));
               }
               if (useTextures)
               {
                   texCoords.Add(new System.Windows.Point(0, 0));
                   texCoords.Add(new System.Windows.Point(0, 1));
                   texCoords.Add(new System.Windows.Point(1, 0));
                   texCoords.Add(new System.Windows.Point(1, 1));
               }
            //   System.Windows.Forms.MessageBox.Show(minX.ToString() + ", " + maxX.ToString() + ", " + minY.ToString() + ", " + maxY.ToString());
               if (texCoords.Count == ptsToAppend.Count)
               {
                   meshBuilder.Append(ptsToAppend, indicesToAppend, textureCoordinatesToAppend: texCoords);
               }
               else
               {
                   meshBuilder.Append(ptsToAppend, indicesToAppend);
               }
                
              

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
