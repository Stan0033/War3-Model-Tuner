using SharpGL;
using System.Drawing.Imaging;
using System.Drawing;
using Wa3Tuner.Helper_Classes;
using System;
using MdxLib.Model;
using MdxLib.Primitives;
using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph;
using System.Windows;
using System.Linq;
using System.Numerics;
namespace Wa3Tuner.Helper_Classes
{
  
    public static class Renderer
    {
       public static void RendertestPoints(OpenGL gl)
        {
            float nodeSize = 10; // Point size in pixels

            gl.Enable(OpenGL.GL_POINT_SMOOTH); // Enable smoother points (optional)
            gl.PointSize(nodeSize);



            gl.Begin(OpenGL.GL_POINTS);

            foreach (xLine line in RayCaster.Lines)
            {
                gl.Color(1,1,1);

                gl.Vertex(line.one.X, line.one.Y, line.one.Z);
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
                    foreach (var node in PathManager.Selected.List)
                    {
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

        public static void RenderTestLines(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_LINES); // Start drawing lines

            foreach (xLine line in RayCaster.Lines)
            {
                // Draw the line between `one` and `two`
                gl.Vertex(line.one.X, line.one.Y, line.one.Z); // First point
                gl.Vertex(line.two.X, line.two.Y, line.two.Z); // Second point
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
            foreach (cLine line in model.GeosetLines)
            {
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
                foreach (cLine line in model.CollisionShapeLines)
                {
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
            foreach (var edge in geo.Edges)
            {
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
           foreach (CGeoset geo in currentModel.Geosets)
            {
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
        public static void RenderTriangles(OpenGL gl, CModel model, bool RenderTextures, List<Texture> textures, bool animated , bool RenderShading)
        {
            List<CGeoset> Geosets = model.Geosets.ObjectList;

            for (int i = 0; i < model.Geosets.Count; i++)
            {
                CGeoset geo = model.Geosets[i];

                int last = geo.Material.Object.Layers.Count - 1;
                bool Twosided = geo.Material.Object.Layers[last].TwoSided;

                if (!geo.isVisible && !animated) { continue; }
                if (animated && !geo.isVisibleAnimated) { continue; }

                var geoAnim = model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geo);
                Vector3 GeosetColor = new Vector3(1, 1, 1);

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

                    foreach (var triangle in geo.Triangles)
                    {
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
                            CVector3 edge1 = new CVector3(pos2.X - pos1.X, pos2.Y - pos1.Y, pos2.Z - pos1.Z);
                            CVector3 edge2 = new CVector3(pos3.X - pos1.X, pos3.Y - pos1.Y, pos3.Z - pos1.Z);
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



        private static void RenderSelectionCube(OpenGL gl, CGeoset geoset)
        {
            // Get the bounding box extents
            var radius = geoset.Extent.Radius;

            var center = Calculator.GetCentroidOfGeoset(geoset);

            // Define the 8 cube vertices using the center and radius
            float minX = center.X - radius, maxX = center.X + radius;
            float minY = center.Y - radius, maxY = center.Y + radius;
            float minZ = center.Z - radius, maxZ = center.Z + radius;

            // Cube vertices
            float[,] vertices = {
        { minX, minY, minZ }, { maxX, minY, minZ }, { maxX, maxY, minZ }, { minX, maxY, minZ }, // Front
        { minX, minY, maxZ }, { maxX, minY, maxZ }, { maxX, maxY, maxZ }, { minX, maxY, maxZ }  // Back
    };

            // Define cube edges (pairs of vertex indices)
            int[,] edges = {
        { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }, // Front face
        { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 }, // Back face
        { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }  // Connect front and back faces
    };

            // Set color for the lines
            gl.Color(RenderSettings.Color_SelectedGeoset[0],
                     RenderSettings.Color_SelectedGeoset[1],
                     RenderSettings.Color_SelectedGeoset[2],
                     1.0f); // Full opacity

            // Render the cube as lines
            gl.Begin(OpenGL.GL_LINES);
            for (int i = 0; i < edges.GetLength(0); i++)
            {
                int v1 = edges[i, 0], v2 = edges[i, 1];
                gl.Vertex(vertices[v1, 0], vertices[v1, 1], vertices[v1, 2]);
                gl.Vertex(vertices[v2, 0], vertices[v2, 1], vertices[v2, 2]);
            }
            gl.End();
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
        public static void RenderGroundTexture(OpenGL gl, Texture t, int size)
        {
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
            foreach (CGeoset geo in model.Geosets)
            {
                foreach (CGeosetVertex v in geo.Vertices)
                {
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
            foreach (var node in model.Nodes)
            {
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

            foreach (INode node in currentModel.Nodes)
            {
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
            foreach (CGeoset geo in geosets)
            {
                if (!geo.isVisible) continue;

                foreach (var vertex in geo.Vertices)
                {
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

        internal static float[][] CalculateEquilateralTriangle(CVector3 pos, float edgeLength, float[] cameraPos, float[] upVector)
        {
            // Calculate the forward vector from the camera to the position
            float[] forwardVector = { pos.X - cameraPos[0], pos.Y - cameraPos[1], pos.Z - cameraPos[2] };
            NormalizeVector(forwardVector);
            // Calculate the right vector as a cross product of the forward vector and up vector
            float[] rightVector = CrossProduct(forwardVector, upVector);
            NormalizeVector(rightVector);
            // Normalize the up vector to ensure consistency
            NormalizeVector(upVector);
            // Calculate the base of the triangle using the right vector
            float[][] triangleVertices = new float[3][];
            // Bottom-left vertex
            triangleVertices[0] = new float[]
            {
        pos.X - rightVector[0] * (edgeLength / 2),
        pos.Y - rightVector[1] * (edgeLength / 2),
        pos.Z - rightVector[2] * (edgeLength / 2)
            };
            // Bottom-right vertex
            triangleVertices[1] = new float[]
            {
        pos.X + rightVector[0] * (edgeLength / 2),
        pos.Y + rightVector[1] * (edgeLength / 2),
        pos.Z + rightVector[2] * (edgeLength / 2)
            };
            // Top vertex: at 60 degrees relative to the base to form an equilateral triangle
            float height = (float)(Math.Sqrt(3) / 2.0f * edgeLength);
            triangleVertices[2] = new float[]
            {
        pos.X + upVector[0] * height - forwardVector[0] * height * 0.1f, // Adding slight forward bias to avoid "stretch"
        pos.Y + upVector[1] * height - forwardVector[1] * height * 0.1f,
        pos.Z + upVector[2] * height - forwardVector[2] * height * 0.1f
            };
            return triangleVertices;
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
        internal static float[][] CalculateBillboardedSquare(
    CVector3 pos, float squareSize, float[] cameraPos, float[] upVector)
        {
            // Get the vector from the camera to the vertex position
            float[] lookVector = new float[3];
            lookVector[0] = pos.X - cameraPos[0];
            lookVector[1] = pos.Y - cameraPos[1];
            lookVector[2] = pos.Z - cameraPos[2];
            // Normalize the look vector
            float length = (float)Math.Sqrt(lookVector[0] * lookVector[0] + lookVector[1] * lookVector[1] + lookVector[2] * lookVector[2]);
            lookVector[0] /= length;
            lookVector[1] /= length;
            lookVector[2] /= length;
            // Calculate the right vector (cross product of look vector and up vector)
            float[] rightVector = new float[3];
            rightVector[0] = upVector[1] * lookVector[2] - upVector[2] * lookVector[1];
            rightVector[1] = upVector[2] * lookVector[0] - upVector[0] * lookVector[2];
            rightVector[2] = upVector[0] * lookVector[1] - upVector[1] * lookVector[0];
            // Normalize and scale the right vector by half the square size
            length = (float)Math.Sqrt(rightVector[0] * rightVector[0] + rightVector[1] * rightVector[1] + rightVector[2] * rightVector[2]);
            rightVector[0] = (rightVector[0] / length) * (squareSize / 2);
            rightVector[1] = (rightVector[1] / length) * (squareSize / 2);
            rightVector[2] = (rightVector[2] / length) * (squareSize / 2);
            // Normalize and scale the up vector by half the square size
            length = (float)Math.Sqrt(upVector[0] * upVector[0] + upVector[1] * upVector[1] + upVector[2] * upVector[2]);
            upVector[0] = (upVector[0] / length) * (squareSize / 2);
            upVector[1] = (upVector[1] / length) * (squareSize / 2);
            upVector[2] = (upVector[2] / length) * (squareSize / 2);
            // Calculate the positions of the four corners of the square
            float[][] squareVertices = new float[4][];
            squareVertices[0] = new float[] { pos.X - rightVector[0] - upVector[0], pos.Y - rightVector[1] - upVector[1], pos.Z - rightVector[2] - upVector[2] }; // Bottom-left
            squareVertices[1] = new float[] { pos.X + rightVector[0] - upVector[0], pos.Y + rightVector[1] - upVector[1], pos.Z + rightVector[2] - upVector[2] }; // Bottom-right
            squareVertices[2] = new float[] { pos.X + rightVector[0] + upVector[0], pos.Y + rightVector[1] + upVector[1], pos.Z + rightVector[2] + upVector[2] }; // Top-right
            squareVertices[3] = new float[] { pos.X - rightVector[0] + upVector[0], pos.Y - rightVector[1] + upVector[1], pos.Z - rightVector[2] + upVector[2] }; // Top-left
            return squareVertices;
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
    }
}
