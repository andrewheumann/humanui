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
    /// <summary>
    /// A class representing a 3D view model for use in a 3d HUI viewport
    /// </summary>
    public class _3DViewModel
    {
         public Model3D Model { get; set; }
         public List<System.Drawing.Color> cols { get; set; }


         /// <summary>
         /// Initializes a new instance of the <see cref="_3DViewModel"/> class with materials for each mesh
         /// </summary>
         /// <param name="meshes">The meshes.</param>
         /// <param name="mats">The mats.</param>
         public _3DViewModel(List<Mesh> meshes, List<Material> mats)
         {
             setupModel(meshes, mats,false);
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="_3DViewModel"/> class with colors for each mesh
         /// </summary>
         /// <param name="meshes">The meshes.</param>
         /// <param name="colors">The colors.</param>
        public _3DViewModel(List<Mesh> meshes, List<System.Drawing.Color> colors)
        {
            List<Material> mats = new List<Material>();
            for (int i = 0; i < meshes.Count; i++)
            {
                mats.Add(MaterialHelper.CreateMaterial(HUI_Util.ToMediaColor(colors[i % colors.Count])));
            }
             setupModel(meshes, mats,false);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="_3DViewModel"/> class with textures for each mesh
        /// </summary>
        /// <param name="meshes">The meshes.</param>
        /// <param name="bitmaps">The bitmaps.</param>
        public _3DViewModel(List<Mesh> meshes, List<string> bitmaps)
        {
            List<Material> mats = new List<Material>();
            //for each mesh, create an image material based on the matching bitmaps
            for (int i = 0; i < meshes.Count; i++)
            {
           
                mats.Add(MaterialHelper.CreateImageMaterial(bitmaps[i % bitmaps.Count],1,UriKind.Absolute,false));
            }
            //set up the model
            setupModel(meshes, mats,true);
        }

        /// <summary>
        /// Sets up the Model3DGroup object
        /// </summary>
        /// <param name="meshes">The meshes.</param>
        /// <param name="mats">The materials.</param>
        /// <param name="useTextures">if set to <c>true</c> [use textures].</param>
        void setupModel(List<Mesh> meshes, List<Material> mats,bool useTextures)
        {

            var modelGroup = new Model3DGroup();

            int i = 0;
            foreach (Mesh m in meshes)
            {
                //set up the MeshBuilder
                var meshBuilder = new MeshBuilder(false, useTextures);
                //triangulate the mesh for consistency
                m.Faces.ConvertQuadsToTriangles();

                List<Point3D> ptsToAppend = new List<Point3D>();
                List<int> indicesToAppend = new List<int>();
                
                //add all mesh coordinates
                foreach (Point3f p in m.Vertices)
                {
                    ptsToAppend.Add(new Point3D(p.X, p.Y, p.Z));
                }

                if (useTextures)
                {
                    //special points to keep from autonormalizing texcoords
                    for (int j = 0; j < 4; j++)
                    {
                        ptsToAppend.Add(new Point3D(0, 0, 0));
                    }
                }
                //make a list of all face indices
                foreach (MeshFace mf in m.Faces)
                {

                    int[] inds = new int[] { mf.A, mf.B, mf.C };
                    indicesToAppend.AddRange(inds);


                }

              
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;
                List<System.Windows.Point> texCoords = new List<System.Windows.Point>();
                //Establish the max and min texture coordinates and add the appropriate texture coords
               foreach(Point2f texPt in m.TextureCoordinates.ToList()){
                   if (texPt.X > maxX) maxX = texPt.X;
                   if (texPt.X < minX) minX = texPt.X;
                   if (texPt.Y > maxY) maxY = texPt.Y;
                   if (texPt.Y < minY) minY = texPt.Y;
                   texCoords.Add(new System.Windows.Point(texPt.X, 1.0-texPt.Y));
               }
                //Add in special texture coords to prevent auto-normalization
               if (useTextures)
               {
                   texCoords.Add(new System.Windows.Point(0, 0));
                   texCoords.Add(new System.Windows.Point(0, 1));
                   texCoords.Add(new System.Windows.Point(1, 0));
                   texCoords.Add(new System.Windows.Point(1, 1));
               }
           
               if (texCoords.Count == ptsToAppend.Count)
               {
                   meshBuilder.Append(ptsToAppend, indicesToAppend, textureCoordinatesToAppend: texCoords);
               }
               else //assume we're not using textures or something went wrong with the TexCoords
               {
                   meshBuilder.Append(ptsToAppend, indicesToAppend);
               }
                
              
                //Get the MeshGeometry3d from the MeshBuilder
                var mesh = meshBuilder.ToMesh(true);

                //add the specific mesh to the model group

                //if materials have been specified
                if (mats.Count > 0)
                {
                    var material = mats[i % mats.Count];
                    modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material });
                }
                else // add model without materials
                {
                    modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh});
                }
                i++;


            }


            this.Model = modelGroup;

        }


      
    }
}
