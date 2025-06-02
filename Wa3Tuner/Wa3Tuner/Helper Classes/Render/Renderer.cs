using SharpGL;
using System.Drawing.Imaging;
using System.Drawing;
 
using System;
using MdxLib.Model;
using MdxLib.Primitives;
using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
 
using System.Linq;
using System.Numerics;
using Whim_GEometry_Editor.Misc;
namespace Wa3Tuner.Helper_Classes
{

  
    public static class Renderer
    {
        public static void EnableAntialiasing(OpenGL gl)
        {
            // Enable multisampling (if supported)
            gl.Enable(OpenGL.GL_MULTISAMPLE);

            // Optional: smoother lines
            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);

            // Optional: smoother polygons
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
            gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
        }
        public static void RendertestPoints(OpenGL gl) //test
        {
            float nodeSize = 10; // Point size in pixels

            gl.Enable(OpenGL.GL_POINT_SMOOTH); // Enable smoother points (optional)
            gl.PointSize(nodeSize);



            gl.Begin(OpenGL.GL_POINTS);

            foreach (Ray line in RayCaster.Lines)
            {
                gl.Color(1,1,1);

                gl.Vertex(line.From.X, line.From.Y, line.From.Z);
            }
              
            gl.End();

            gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }
        public static void RenderSelectedPath(OpenGL gl)
        {
            if (PathManager.Selected != null && PathManager.Selected.Count > 0) // At least 2 nodes needed to form a line
            {
                gl.Disable(OpenGL.GL_TEXTURE_2D); // Ensure no textures interfere

                gl.PointSize(RenderSettings.PathNodeSize);
                gl.Begin(OpenGL.GL_POINTS);

                if (PathManager.Selected != null && PathManager.Selected.Count > 0)
                {


                    // Draw points
                    int count = PathManager.Selected.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var node = PathManager.Selected.List[i];

                    
                        if (node.IsSelected)
                            gl.Color(RenderSettings.Path_Node_Selected[0], RenderSettings.Path_Node_Selected[1], RenderSettings.Path_Node_Selected[2]);
                        else
                            gl.Color(RenderSettings.Path_Node[0], RenderSettings.Path_Node[1], RenderSettings.Path_Node[2]);

                        gl.Vertex(node.Position.X, node.Position.Y, node.Position.Z);
                    }
                    gl.End();

                    // Draw a separate line between each consecutive pair of nodes
                    gl.Color(RenderSettings.Path_Line[0], RenderSettings.Path_Line[1], RenderSettings.Path_Line[2]);
                    gl.LineWidth(2.0f);

                    for (int i = 0; i < PathManager.Selected.Count - 1; i++) // Loop through every pair
                    {
                        CVector3 start = PathManager.Selected.List[i].Position;
                        CVector3 end = PathManager.Selected.List[i + 1].Position;

                        gl.Begin(OpenGL.GL_LINES); // Begin a new line for each pair
                        gl.Vertex(start.X, start.Y, start.Z);
                        gl.Vertex(end.X, end.Y, end.Z);
                        gl.End(); // End the line
                    }

                    gl.Enable(OpenGL.GL_TEXTURE_2D); // Restore textures if needed

                }
            }
        }

        public static void RenderTestLines(OpenGL gl) //test
        {
            gl.Begin(OpenGL.GL_LINES); // Start drawing lines

            foreach (Ray line in RayCaster.Lines)
            {
                // Draw the line between `one` and `two`
                gl.Vertex(line.From.X, line.From.Y, line.From.Z); // First point
                gl.Vertex(line.To.X, line.To.Y, line.To.Z); // Second point
            }

            gl.End(); // End the drawing
        }

        public static void RenderGrid(OpenGL gl, float gridSize, float lineSpacing)
        {
            if (gridSize == 0 || lineSpacing == 0) { return; }
            // Calculate half the size for easier symmetric drawing
            float halfGridSize = gridSize / 2f;
            // Set line color (grayish)
            gl.Color(RenderSettings.GridColor[0], RenderSettings.GridColor[1], RenderSettings.GridColor[2]);  // RGB gray color
            gl.Begin(OpenGL.GL_LINES);
            // Draw lines along the X axis (horizontal)
            for (float y = -halfGridSize; y <= halfGridSize; y += lineSpacing)
            {
                // Horizontal lines parallel to the X axis (constant Y, varying X)
                gl.Vertex(-halfGridSize, y, 0);
                gl.Vertex(halfGridSize, y, 0);
            }
            // Draw lines along the Y axis (vertical)
            for (float x = -halfGridSize; x <= halfGridSize; x += lineSpacing)
            {
                // Vertical lines parallel to the Y axis (constant X, varying Y)
                gl.Vertex(x, -halfGridSize, 0);
                gl.Vertex(x, halfGridSize, 0);
            }
            gl.End();
        }
        public static void RenderYZGrid(OpenGL gl, float gridSize, float lineSpacing)
        {
            if (gridSize == 0 || lineSpacing == 0) { return; }
            float halfGridSize = gridSize / 2f;
            // Set line color (grayish)
            gl.Color(RenderSettings.GridColor[0], RenderSettings.GridColor[1], RenderSettings.GridColor[2]);
            gl.Begin(OpenGL.GL_LINES);
            // Draw lines along the Y axis (horizontal in the YZ plane)
            for (float z = -halfGridSize; z <= halfGridSize; z += lineSpacing)
            {
                // Horizontal lines parallel to the Y axis (constant Z, varying Y)
                gl.Vertex(0, -halfGridSize, z);
                gl.Vertex(0, halfGridSize, z);
            }
            // Draw lines along the Z axis (vertical in the YZ plane)
            for (float y = -halfGridSize; y <= halfGridSize; y += lineSpacing)
            {
                // Vertical lines parallel to the Z axis (constant Y, varying Z)
                gl.Vertex(0, y, -halfGridSize);
                gl.Vertex(0, y, halfGridSize);
            }
            gl.End();
        }
        public static void RenderXZGrid(OpenGL gl, float gridSize, float lineSpacing)
        {
            if (gridSize == 0 || lineSpacing == 0) { return; }
            float halfGridSize = gridSize / 2f;
            // Set line color (grayish)
            gl.Color(RenderSettings.GridColor[0], RenderSettings.GridColor[1], RenderSettings.GridColor[2]);
            gl.Begin(OpenGL.GL_LINES);
            // Draw lines along the X axis (horizontal in the XZ plane)
            for (float z = -halfGridSize; z <= halfGridSize; z += lineSpacing)
            {
                // Horizontal lines parallel to the X axis (constant Z, varying X)
                gl.Vertex(-halfGridSize, 0, z);
                gl.Vertex(halfGridSize, 0, z);
            }
            // Draw lines along the Z axis (vertical in the XZ plane)
            for (float x = -halfGridSize; x <= halfGridSize; x += lineSpacing)
            {
                // Vertical lines parallel to the Z axis (constant X, varying Z)
                gl.Vertex(x, 0, -halfGridSize);
                gl.Vertex(x, 0, halfGridSize);
            }
            gl.End();
        }
        public static void RenderExtents(OpenGL gl, CModel model)
        {
            // Set the color to blue (R, G, B)
            gl.Color(RenderSettings.Color_Extent[0], RenderSettings.Color_Extent[1], RenderSettings.Color_Extent[1]);
            // Begin drawing lines
            gl.Begin(OpenGL.GL_LINES);
            int count = model.GeosetLines.Count;
            for (int i = 0; i < count; i++) {
                var line = model.GeosetLines[i];
            
                // Specify the start point of the line
                gl.Vertex(line.From.X, line.From.Y, line.From.Z);
                // Specify the end point of the line
                gl.Vertex(line.To.X, line.To.Y, line.To.Z);
            }
            // End drawing
            gl.End();
        }
        public static void RenderCollisionShapes(OpenGL gl, CModel model)
        {
            if (model.CollisionShapeLines.Count > 0)
            {
                // Set the color to blue (R, G, B)
                gl.Color(RenderSettings.Color_CollisionShape[0], 
                    RenderSettings.Color_CollisionShape[1], RenderSettings.Color_CollisionShape[2]);

                // Begin drawing lines
                gl.Begin(OpenGL.GL_LINES);
                int count = model.CollisionShapeLines.Count;
                for (int i = 0; i < count; i++)
                {
                    var line = model.CollisionShapeLines[i];
               
                
                    // Specify the start point of the line
                    gl.Vertex(line.From.X, line.From.Y, line.From.Z);
                    // Specify the end point of the line
                    gl.Vertex(line.To.X, line.To.Y, line.To.Z);
                }
                // End drawing
                gl.End();
            }
        }
        private static void RenderEdgesOf(OpenGL gl, CGeoset geo, bool Render)
        {
            if (!Render && !geo.isSelected) { return; }
            gl.Begin(OpenGL.GL_LINES); // Start drawing lines once for all edges
            int count = geo.Edges.Count;
            for (int i = 0; i < count; i++) {
                var edge = geo.Edges[i];
            
                if (edge.isSelected || geo.isSelected)
                {
                    gl.Color(RenderSettings.Color_Edge_Selected[0],
                        RenderSettings.Color_Edge_Selected[1], 
                        RenderSettings.Color_Edge_Selected[2]);
                }
                else
                {
                    gl.Color(RenderSettings.Color_Edge[0],
                        RenderSettings.Color_Edge[1], RenderSettings.Color_Edge[2]);
                }
                // Get the vertices by their IDs
                var v1 = edge.Vertex1;
                var v2 = edge.Vertex2;
                // Draw the edge
                gl.Vertex(v1.Position.X, v1.Position.Y, v1.Position.Z);
                gl.Vertex(v2.Position.X, v2.Position.Y, v2.Position.Z);
            }
            gl.End(); // End drawing lines after all edges have been drawn
        }
        public static void RenderEdges(OpenGL gl, CModel currentModel, bool DoRender)
        {
            int count = currentModel.Geosets.Count;
            for (int i = 0; i < count; i++) {
                var geo = currentModel.Geosets[i];
            
                if (!geo.isVisible) continue;
               
                RenderEdgesOf(gl, geo, DoRender);
            }
        }
        public static void RenderAxis(OpenGL gl, int gridsize)
        {
            float axisLength = gridsize; // Set the length of the axis lines
            gl.Begin(OpenGL.GL_LINES);
            // X-Axis (Red)
            gl.Color(1.0f, 0.0f, 0.0f);  // Red color
            gl.Vertex(0, 0, 0);          // Start at the origin
            gl.Vertex(axisLength, 0, 0);  // End at (axisLength, 0, 0)
            // Y-Axis (Green)
            gl.Color(0.0f, 1.0f, 0.0f);  // Green color
            gl.Vertex(0, 0, 0);          // Start at the origin
            gl.Vertex(0, axisLength, 0);  // End at (0, axisLength, 0)
            // Z-Axis (Blue)
            gl.Color(0.0f, 0.0f, 1.0f);  // Blue color
            gl.Vertex(0, 0, 0);          // Start at the origin
            gl.Vertex(0, 0, axisLength);  // End at (0, 0, axisLength)
            gl.End();
        }
        public static uint LoadTextureFromBitmap(OpenGL gl, Bitmap bitmap)
        {
            if (bitmap == null) { throw new Exception("Provided bitmap for handling texture was null"); }
            uint[] textureId = new uint[1]; // Create an array to hold the texture ID
            gl.GenTextures(1, textureId); // Generate a texture ID
            // Set texture parameters
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            // Convert the Bitmap to a byte array
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                               ImageLockMode.ReadOnly,
                                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Upload the texture data
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0,
                          OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
            bitmap.UnlockBits(data); // Unlock the bitmap
            return textureId[0]; // Return the generated texture ID
        }
        public static void RenderTriangles(OpenGL gl, CModel model, bool RenderTextures, List<Texture> textures, 
            bool animated , bool RenderShading)
        {
            List<CGeoset> Geosets = model.Geosets.ObjectList;

            int gCount = Geosets.Count;
            for (int i = 0; i < gCount; i++)
            {
                CGeoset geo = model.Geosets[i];

                int last = geo.Material.Object.Layers.Count - 1;
                bool Twosided = geo.Material.Object.Layers[last].TwoSided;

                if (!geo.isVisible && !animated) { continue; }
                if (animated && !geo.isVisibleAnimated) { continue; }

                var geoAnim = model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geo);
                Vector3 GeosetColor = new  (1, 1, 1);

                if (geoAnim != null && geoAnim.UseColor && geoAnim.Color.Static)
                {
                    var color = geoAnim.Color.GetValue();
                    GeosetColor = new Vector3(color.Z, color.X, color.Z);
                }

                for (int layerIndex = 0; layerIndex < geo.Material.Object.Layers.Count; layerIndex++)
                {
                    var layer = geo.Material.Object.Layers[layerIndex];

                    gl.Enable(OpenGL.GL_BLEND);

                    switch (layer.FilterMode)
                    {
                        case EMaterialLayerFilterMode.None:
                        case EMaterialLayerFilterMode.Transparent:
                            gl.Disable(OpenGL.GL_BLEND);
                            break;

                        case EMaterialLayerFilterMode.Additive:
                        case EMaterialLayerFilterMode.AdditiveAlpha:
                            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
                            break;

                        case EMaterialLayerFilterMode.Modulate:
                            gl.BlendFunc(OpenGL.GL_DST_COLOR, OpenGL.GL_ZERO);
                            break;

                        case EMaterialLayerFilterMode.Modulate2x:
                            gl.BlendFunc(OpenGL.GL_DST_COLOR, OpenGL.GL_SRC_COLOR);
                            break;

                        case EMaterialLayerFilterMode.Blend:
                            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                            break;
                    }

                    if (RenderTextures && layer.Texture != null)
                    {
                        gl.Enable(OpenGL.GL_TEXTURE_2D);
                        gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                        textures[i].Bind(gl);
                    }

                    if (Twosided)
                        gl.Disable(OpenGL.GL_CULL_FACE);
                    else
                        gl.Enable(OpenGL.GL_CULL_FACE);

                    gl.Begin(OpenGL.GL_TRIANGLES);
                    int  count = geo.Triangles.Count;
                    for (int j=0; j<count; j++) {
                        var triangle = geo.Triangles[j];
                   
                        var v1 = triangle.Vertex1;
                        var v2 = triangle.Vertex2;
                        var v3 = triangle.Vertex3;

                        CVector3 pos1 = animated ? v1.Object.AnimatedPosition : v1.Object.Position;
                        CVector3 pos2 = animated ? v2.Object.AnimatedPosition : v2.Object.Position;
                        CVector3 pos3 = animated ? v3.Object.AnimatedPosition : v3.Object.Position;

                        // Determine normals
                        CVector3 normal1, normal2, normal3;
                        if (RenderShading)
                        {
                            normal1 = v1.Object.Normal;
                            normal2 = v2.Object.Normal;
                            normal3 = v3.Object.Normal;
                        }
                        else
                        {
                            CVector3 edge1 = new  (pos2.X - pos1.X, pos2.Y - pos1.Y, pos2.Z - pos1.Z);
                            CVector3 edge2 = new  (pos3.X - pos1.X, pos3.Y - pos1.Y, pos3.Z - pos1.Z);
                            CVector3 normal = Cross(edge1, edge2);
                            normal1 = normal2 = normal3 = normal;
                        }

                        // Color
                        if (triangle.isSelected)
                        {
                            gl.Color(RenderSettings.Color_VertexSelected[0],
                                     RenderSettings.Color_VertexSelected[1],
                                     RenderSettings.Color_VertexSelected[2],
                                     1.0f);
                        }
                        else
                        {
                            gl.Color(GeosetColor.X, GeosetColor.Y, GeosetColor.Z, 1.0f);
                        }

                        // Vertices + normals + textures
                        if (RenderTextures)
                        {
                            gl.Normal(normal1.X, normal1.Y, normal1.Z);
                            gl.TexCoord(v1.Object.TexturePosition.X, v1.Object.TexturePosition.Y);
                            gl.Vertex(pos1.X, pos1.Y, pos1.Z);

                            gl.Normal(normal2.X, normal2.Y, normal2.Z);
                            gl.TexCoord(v2.Object.TexturePosition.X, v2.Object.TexturePosition.Y);
                            gl.Vertex(pos2.X, pos2.Y, pos2.Z);

                            gl.Normal(normal3.X, normal3.Y, normal3.Z);
                            gl.TexCoord(v3.Object.TexturePosition.X, v3.Object.TexturePosition.Y);
                            gl.Vertex(pos3.X, pos3.Y, pos3.Z);
                        }
                        else
                        {
                            gl.Normal(normal1.X, normal1.Y, normal1.Z);
                            gl.Vertex(pos1.X, pos1.Y, pos1.Z);

                            gl.Normal(normal2.X, normal2.Y, normal2.Z);
                            gl.Vertex(pos2.X, pos2.Y, pos2.Z);

                            gl.Normal(normal3.X, normal3.Y, normal3.Z);
                            gl.Vertex(pos3.X, pos3.Y, pos3.Z);
                        }
                    }

                    gl.End();

                    if (RenderTextures)
                        gl.Disable(OpenGL.GL_TEXTURE_2D);
                }

                gl.Disable(OpenGL.GL_BLEND);
            }
        }

        private static CVector3 Cross(CVector3 original, CVector3 other)
        {
            return new CVector3(
                original.Y * other.Z - original.Z * other.Y,
                original.Z * other.X - original.X * other.Z,
                original.X * other.Y - original.Y * other.X
            );
        }



       
        public static void RenderNormals(OpenGL gl, CModel model)
        {
            float normalLength = 0.5f; // Adjust length of the normal visualization
            foreach (CGeoset geo in model.Geosets)
            {
                foreach (CGeosetVertex vertex in geo.Vertices)
                {
                    CVector3 pos = vertex.Position;
                    CVector3 normal = vertex.Normal;
                    // Set color to green
                    gl.Color(RenderSettings.Color_Normals[0], RenderSettings.Color_Normals[1], RenderSettings.Color_Normals[2]); // Green
                    // Begin drawing a line
                    gl.Begin(OpenGL.GL_LINES);
                    // Start point of the line is the vertex position
                    gl.Vertex(pos.X, pos.Y, pos.Z);
                    // End point of the line is the vertex position plus the scaled normal vector
                    gl.Vertex(
                        pos.X + normal.X * normalLength,
                        pos.Y + normal.Y * normalLength,
                        pos.Z + normal.Z * normalLength
                    );
                    gl.End();
                }
            }
        }
        public static void RenderGroundTexture(OpenGL gl, Texture? t, int size)
        {
            if (t == null) return;
          //  MessageBox.Show("grd");
            gl.Enable(OpenGL.GL_TEXTURE_2D);



            gl.Color(1.0f, 1.0f, 1.0f);

            t.Bind(gl);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_REPLACE);
            gl.Begin(OpenGL.GL_QUADS); // Draw the quad
                                       // Normal for both sides
            gl.Normal(0.0f, 0.0f, 1.0f);

            // Bottom-left
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-size / 2.0f, -size / 2.0f, 0);

            // Bottom-right
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(size / 2.0f, -size / 2.0f, 0);

            // Top-right
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(size / 2.0f, size / 2.0f, 0);

            // Top-left
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-size / 2.0f, size / 2.0f, 0);

            gl.End();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
        }


        public static void RenderRigging(OpenGL gl, CModel model)
        {
            int gcount = model.Geosets.Count;
            for (int i = 0; i < gcount; i++)
            {
                var geo = model.Geosets[i];

            int vcount = geo.Vertices.Count;
                for (int j = 0; j < vcount; j++)
                {
                    var v = geo.Vertices[j];
                
                    CVector3 From = v.Position;
                    foreach (var gnode in v.Group.Object.Nodes)
                    {
                        CVector3 To = gnode.Node.Node.PivotPoint;
                        DrawLineBetweenVertexAndBone(gl, From, To);
                    }
                }
            }
        }
        private static void DrawLineBetweenVertexAndBone(OpenGL gl, CVector3 from, CVector3 to)
        {
            // Set color to orange
            gl.Color(RenderSettings.Color_Skinning[0], RenderSettings.Color_Skinning[1], RenderSettings.Color_Skinning[2]); // RGB for orange
            // Begin drawing lines
            gl.Begin(OpenGL.GL_LINES);
            // Specify the start point of the line (vertex position)
            gl.Vertex(from.X, from.Y, from.Z);
            // Specify the end point of the line (bone pivot point)
            gl.Vertex(to.X, to.Y, to.Z);
            // End drawing lines
            gl.End();
        }
        private static void DrawLineBetweenNodes(OpenGL gl, CVector3 from, CVector3 to)
        {
            gl.Color(RenderSettings.Color_Skeleton[0], RenderSettings.Color_Skeleton[0], RenderSettings.Color_Skeleton[2]); // RGB for purple
            // Begin drawing lines
            gl.Begin(OpenGL.GL_LINES);
            // Specify the start point of the line (vertex position)
            gl.Vertex(from.X, from.Y, from.Z);
            // Specify the end point of the line (bone pivot point)
            gl.Vertex(to.X, to.Y, to.Z);
            // End drawing lines
            gl.End();
        }
      
        public static void RenderSkeleton(OpenGL gl, CModel model)
        {
            int count = model.Nodes.Count; ;
            for (int i = 0; i < count; i++)
            {
                var node = model.Nodes[i];
                if (node.Parent != null && node.Parent.Node != null)
                {
                    CVector3 From = node.PivotPoint;
                    CVector3 To = node.Parent.Node.PivotPoint;
                    DrawLineBetweenNodes(gl, From, To);
                }
            }
            
        }
        internal static void RenderNodes(OpenGL gl, CModel currentModel, bool animated = false)
        {
              float nodeSize = RenderSettings.NodeSize; // Point size in pixels

            gl.Enable(OpenGL.GL_POINT_SMOOTH); // Enable smoother points (optional)
            gl.PointSize(nodeSize);



            gl.Begin(OpenGL.GL_POINTS);

            int count = currentModel.Nodes.Count;
            for (int i = 0; i < count; i++)
            {
                var node = currentModel.Nodes[i];
                if (animated && node.IsVisibleAnimated == false) { continue; }
                var position = node.PivotPoint;

                // Fixed colors (Red = Selected, Blue = Normal)
                if (node.IsSelected)
                    gl.Color(RenderSettings.Color_NodeSelected[0], RenderSettings.Color_NodeSelected[1], RenderSettings.Color_NodeSelected[1]);
                else
                    gl.Color(RenderSettings.Color_Node[0], RenderSettings.Color_Node[1], RenderSettings.Color_Node[1]);

                gl.Vertex(position.X, position.Y, position.Z);
            }
            
            gl.End();

            gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }

        internal static void RenderVertices(OpenGL gl, CModel model, bool UseAnimatedPositions)
        {
             float vertexSize = RenderSettings.VertexSize; // Point size in pixels

            gl.Enable(OpenGL.GL_POINT_SMOOTH); // Enable smoother points (optional)
            gl.PointSize(vertexSize);

            List<CGeoset> geosets = model.Geosets.ObjectList;

            gl.Begin(OpenGL.GL_POINTS);
            int gCount = geosets.Count;
            for (int i = 0; i < gCount; i++)
            {
                var geo = geosets[i];
            
                if (!geo.isVisible) continue;

                int count = geo.Vertices.Count;
                for (int j = 0; j < count; j++)
                {
                    var vertex = geo.Vertices[j];
               
                    var position = UseAnimatedPositions ? vertex.AnimatedPosition : vertex.Position;

                    if (vertex.isSelected)
                    {
                        if (vertex.isRigged)
                        {
                            gl.Color(RenderSettings.Color_VertexRiggedSelected[0], RenderSettings.Color_VertexRiggedSelected[1], RenderSettings.Color_VertexRiggedSelected[2]); // Red

                        }
                        else
                        {
                            gl.Color(RenderSettings.Color_VertexSelected[0], RenderSettings.Color_VertexSelected[1], RenderSettings.Color_VertexSelected[2]); // Red

                        }
                    }
                    else
                    {
                        if (vertex.isRigged)
                        {
                            gl.Color(RenderSettings.Color_VertexRigged[0], RenderSettings.Color_VertexRigged[1], RenderSettings.Color_VertexRigged[2]);

                        }
                        else
                        {
                            gl.Color(RenderSettings.Color_Vertex[0], RenderSettings.Color_Vertex[1], RenderSettings.Color_Vertex[2]);
                        }
                    }

                    gl.Vertex(position.X, position.Y, position.Z);
                }
            }
            gl.End();

            gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }

        
        internal static void NormalizeVector(float[] vector)
        {
            float length = (float)Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
            // Prevent division by zero
            if (length == 0.0f)
                return;
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }
        internal static float[] CrossProduct(float[] vectorA, float[] vectorB)
        {
            return new float[]
            {
        vectorA[1] * vectorB[2] - vectorA[2] * vectorB[1],
        vectorA[2] * vectorB[0] - vectorA[0] * vectorB[2],
        vectorA[0] * vectorB[1] - vectorA[1] * vectorB[0]
            };
        }
       
        internal static void ApplySSAA(OpenGL gl)
        {
            return;
        }
        internal static void ApplySMAA(OpenGL gl)
        {
            return;
        }
        internal static void ApplyFXAA(OpenGL gl)
        {
            return;
        }
        internal static void HandleLighting(OpenGL gl, bool enable)
        {
             
            if (enable)
            {
                // Enable lighting and set up light 0
                gl.Enable(OpenGL.GL_LIGHTING);
                gl.Enable(OpenGL.GL_LIGHT0);
                // Set the ambient, diffuse, and specular components of the light
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, RenderSettings.AmbientColor);
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, RenderSettings.DiffuseColor);
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, RenderSettings.SpecularColor);
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, RenderSettings.LightPostion);
                // Set material properties for the object being drawn
                gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, RenderSettings.Shininess);
            }
            else
            {
                // Disable lighting
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_LIGHT0);
            }
        }
        internal static void HandeShading(OpenGL gl, bool shading)
        {
            if (shading)
            {
                // Enable smooth shading or flat shading based on preference
                gl.ShadeModel(OpenGL.GL_SMOOTH);  // Or use GL_FLAT for flat shading
            }
            else
            {
                // Default back to flat shading or disable shading if preferred
                gl.ShadeModel(OpenGL.GL_FLAT);
            }
        }
        internal static void HandleCulling(OpenGL gl)
        {
            if (RenderSettings.BackfaceCullingEnabled)
            {
                if (RenderSettings.BackfaceCullingClockwise)
                {
                    // Enable backface culling
                    gl.Enable(OpenGL.GL_CULL_FACE);
                    // Set the front face winding order to clockwise (CW)
                    gl.FrontFace(OpenGL.GL_CW);
                    // Cull back faces
                    gl.CullFace(OpenGL.GL_BACK);
                }
                else
                {
                    // Enable backface culling
                    gl.Enable(OpenGL.GL_CULL_FACE);
                    // Set the front face winding order to counterclockwise (CCW)
                    gl.FrontFace(OpenGL.GL_CCW);
                    // Cull back faces
                    gl.CullFace(OpenGL.GL_BACK);
                }
            }
        }

        internal static void RenderTestExtents(OpenGL gl) //test
        {
            // Set the color to blue (R, G, B)
            gl.Color(RenderSettings.Color_Extent[0], RenderSettings.Color_Extent[1], RenderSettings.Color_Extent[2]);

            gl.Begin(OpenGL.GL_LINES);

            foreach (Extent ex in RayCaster.Extents)
            {
                float x1 = ex.minX, x2 = ex.maxX;
                float y1 = ex.minY, y2 = ex.maxY;
                float z1 = ex.minZ, z2 = ex.maxZ;

                // 12 lines of a box (edges)
                // Bottom face
                gl.Vertex(x1, y1, z1); gl.Vertex(x2, y1, z1);
                gl.Vertex(x2, y1, z1); gl.Vertex(x2, y2, z1);
                gl.Vertex(x2, y2, z1); gl.Vertex(x1, y2, z1);
                gl.Vertex(x1, y2, z1); gl.Vertex(x1, y1, z1);

                // Top face
                gl.Vertex(x1, y1, z2); gl.Vertex(x2, y1, z2);
                gl.Vertex(x2, y1, z2); gl.Vertex(x2, y2, z2);
                gl.Vertex(x2, y2, z2); gl.Vertex(x1, y2, z2);
                gl.Vertex(x1, y2, z2); gl.Vertex(x1, y1, z2);

                // Vertical edges
                gl.Vertex(x1, y1, z1); gl.Vertex(x1, y1, z2);
                gl.Vertex(x2, y1, z1); gl.Vertex(x2, y1, z2);
                gl.Vertex(x2, y2, z1); gl.Vertex(x2, y2, z2);
                gl.Vertex(x1, y2, z1); gl.Vertex(x1, y2, z2);
            }

            gl.End();
        }

    }
}
