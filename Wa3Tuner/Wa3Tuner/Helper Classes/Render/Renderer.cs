using SharpGL;
 using System.Drawing.Imaging;
using System.Drawing;
using Wa3Tuner.Helper_Classes;
using System;
using MdxLib.Model;
using MdxLib.Primitives;
using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
namespace Wa3Tuner.Helper_Classes
{
    enum PointType { Square, Triangle,
        Sphere
    }
    public static class Renderer
    {
       /* public static w3Geoset CreateCubeFromExtent(CExtent extent)
        {
            w3Geoset geo = new w3Geoset();
            // Get the min and max coordinates from the extent
            float minX = extent.Min.X;
            float minY = extent.Y;
            float minZ = extent.Min.Z;
            float maxX = extent.Max.X;
            float maxY = extent.Max.Y;
            float maxZ = extent.Max.Z;
            // Define the vertices of the cube
            CGeosetVertex v0 = new CGeosetVertex(minX, minY, minZ); // Bottom-left-back
            w3Vertex v1 = new w3Vertex(maxX, minY, minZ); // Bottom-right-back
            w3Vertex v2 = new w3Vertex(maxX, maxY, minZ); // Top-right-back
            w3Vertex v3 = new w3Vertex(minX, maxY, minZ); // Top-left-back
            w3Vertex v4 = new w3Vertex(minX, minY, maxZ); // Bottom-left-front
            w3Vertex v5 = new w3Vertex(maxX, minY, maxZ); // Bottom-right-front
            w3Vertex v6 = new w3Vertex(maxX, maxY, maxZ); // Top-right-front
            w3Vertex v7 = new w3Vertex(minX, maxY, maxZ); // Top-left-front
            // Add vertices to the geoset
            geo.Vertices.Add(v0);
            geo.Vertices.Add(v1);
            geo.Vertices.Add(v2);
            geo.Vertices.Add(v3);
            geo.Vertices.Add(v4);
            geo.Vertices.Add(v5);
            geo.Vertices.Add(v6);
            geo.Vertices.Add(v7);
            // Define the triangles of the cube using variable names
            geo.Triangles.Add(new w3Triangle(v0, v1, v2)); // Back face
            geo.Triangles.Add(new w3Triangle(v0, v2, v3));
            geo.Triangles.Add(new w3Triangle(v4, v5, v6)); // Front face
            geo.Triangles.Add(new w3Triangle(v4, v6, v7));
            geo.Triangles.Add(new w3Triangle(v0, v1, v5)); // Bottom face
            geo.Triangles.Add(new w3Triangle(v0, v5, v4));
            geo.Triangles.Add(new w3Triangle(v2, v3, v7)); // Top face
            geo.Triangles.Add(new w3Triangle(v2, v7, v6));
            geo.Triangles.Add(new w3Triangle(v1, v2, v6)); // Right face
            geo.Triangles.Add(new w3Triangle(v1, v6, v5));
            geo.Triangles.Add(new w3Triangle(v0, v3, v7)); // Left face
            geo.Triangles.Add(new w3Triangle(v0, v7, v4));
            // Finalize the geometry
            geo.RecalculateEdges();
            geo.ID = IDCounter.Next();
            geo.Material_ID = 0;
            return geo;
        }
        */
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
            gl.Color(0.0f, 0.0f, 1.0f); // Blue color
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
                gl.Color(0.0f, 0.0f, 1.0f); // Blue color
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
        public static void RenderEdges(OpenGL gl, CModel currentModel, List<CGeoset> ModifiedGeosets = null)
        {
            List<CGeoset> Geosets = ModifiedGeosets != null ? ModifiedGeosets : currentModel.Geosets.ObjectList;
            foreach (CGeoset geo in Geosets)
            {
                if (!geo.isVisible) continue;
                    gl.Begin(OpenGL.GL_LINES); // Start drawing lines once for all edges
                    foreach (var edge in geo.Edges)
                    {
                        if (edge.isSelected)
                        {
                            gl.Color(RenderSettings.EdgeColorSelected[0], RenderSettings.EdgeColorSelected[1], RenderSettings.EdgeColorSelected[2]);
                        }
                        else
                        {
                            gl.Color(RenderSettings.EdgeColor[0], RenderSettings.EdgeColor[1], RenderSettings.EdgeColor[2]);
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
        public static void RenderTriangles(OpenGL gl, CModel model, bool RenderTextures, List<Texture> textures)
        {
            List<CGeoset> Geosets = model.Geosets.ObjectList;

            for (int i = 0; i < model.Geosets.Count; i++)
            {
                CGeoset geo = model.Geosets[i];
                if (!geo.isVisible) continue;

                bool hasTexture = RenderTextures && textures[i] != null;

                // FIRST PASS: Render the normal texture or color
                if (hasTexture)
                {
                    gl.Enable(OpenGL.GL_TEXTURE_2D);
                    textures[i].Bind(gl);
                }

                gl.Begin(OpenGL.GL_TRIANGLES);
                foreach (var triangle in geo.Triangles)
                {
                    var v1 = triangle.Vertex1;
                    var v2 = triangle.Vertex2;
                    var v3 = triangle.Vertex3;

                    if (triangle.isSelected)
                    {
                        gl.Color(RenderSettings.Color_VertexSelected[0],
                                 RenderSettings.Color_VertexSelected[1],
                                 RenderSettings.Color_VertexSelected[2],
                                 1.0f); // Full alpha for normal rendering
                    }
                    else
                    {
                        gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }

                    if (hasTexture) gl.TexCoord(v1.Object.TexturePosition.X, v1.Object.TexturePosition.Y);
                    gl.Vertex(v1.Object.Position.X, v1.Object.Position.Y, v1.Object.Position.Z);

                    if (hasTexture) gl.TexCoord(v2.Object.TexturePosition.X, v2.Object.TexturePosition.Y);
                    gl.Vertex(v2.Object.Position.X, v2.Object.Position.Y, v2.Object.Position.Z);

                    if (hasTexture) gl.TexCoord(v3.Object.TexturePosition.X, v3.Object.TexturePosition.Y);
                    gl.Vertex(v3.Object.Position.X, v3.Object.Position.Y, v3.Object.Position.Z);
                }
                gl.End();

                if (hasTexture) gl.Disable(OpenGL.GL_TEXTURE_2D);

                // SECOND PASS: Transparent overlay for selected geosets
                if (geo.isSelected)
                {
                    RenderSelectionCube(gl, geo);
                    
                }
            }
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
            gl.Color(RenderSettings.SelectedGeoset[0],
                     RenderSettings.SelectedGeoset[1],
                     RenderSettings.SelectedGeoset[2],
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
                    gl.Color(RenderSettings.NormalsColor[0], RenderSettings.NormalsColor[1], RenderSettings.NormalsColor[2]); // Green
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
            gl.Color(RenderSettings.RiggingColor[0], RenderSettings.RiggingColor[1], RenderSettings.RiggingColor[2]); // RGB for orange
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
            gl.Color(RenderSettings.SkeletonColor[0], RenderSettings.SkeletonColor[0], RenderSettings.SkeletonColor[2]); // RGB for purple
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
        internal static void RenderNodes(OpenGL gl, CModel currentModel)
        {
            float rectWidth = 0.5f * RenderSettings.PointSize;  // Default width of the rectangle or triangle
            float rectHeight = 0.025f * RenderSettings.PointSize; // Adjusted height of the triangle to avoid being too tall
            // Camera position (eye)
            float[] cameraPos = { CameraControl.eyeX, CameraControl.eyeY, CameraControl.eyeZ };
            // Up vector (camera up direction)
            float[] baseUpVector = { CameraControl.UpX, CameraControl.UpY, CameraControl.UpZ };
            // Enable polygon offset to avoid Z-fighting
            gl.Enable(OpenGL.GL_POLYGON_OFFSET_FILL);
            gl.PolygonOffset(-1.0f, -1.0f);
            // Fetch point size from settings
            float pointSize = RenderSettings.PointSize;
            foreach (INode node in currentModel.Nodes)
            {
                CVector3 pos = node.PivotPoint;
                // Set color based on selection state
                if (node.IsSelected)
                {
                    gl.Color(RenderSettings.NodeColorSelected[0], RenderSettings.NodeColorSelected[1], RenderSettings.NodeColorSelected[2]); // Color for selected nodes
                }
                else
                {
                    gl.Color(RenderSettings.NodeColor[0], RenderSettings.NodeColor[1], RenderSettings.NodeColor[2]); // Color for unselected nodes
                }
                // Determine if we draw squares or triangles based on settings
                if (RenderSettings.PointType == PointType.Square)
                {
                    // Get the four corners of the rectangle (adjusted for camera view)
                    float[][] rectangleVertices = CalculateBillboardedSquare(pos, rectWidth, cameraPos, (float[])baseUpVector.Clone());
                    // Draw the rectangle
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Vertex(rectangleVertices[0][0], rectangleVertices[0][1], rectangleVertices[0][2]); // Bottom-left
                    gl.Vertex(rectangleVertices[1][0], rectangleVertices[1][1], rectangleVertices[1][2]); // Bottom-right
                    gl.Vertex(rectangleVertices[2][0], rectangleVertices[2][1], rectangleVertices[2][2]); // Top-right
                    gl.Vertex(rectangleVertices[3][0], rectangleVertices[3][1], rectangleVertices[3][2]); // Top-left
                    gl.End();
                }
                else if (RenderSettings.PointType == PointType.Triangle)
                {
                    // Get the three vertices of the triangle (adjusted for camera view)
                    float[][] triangleVertices = CalculateEquilateralTriangle(pos, rectWidth, cameraPos, (float[])baseUpVector.Clone());
                    // Draw the triangle
                    gl.Begin(OpenGL.GL_TRIANGLES);
                    gl.Vertex(triangleVertices[0][0], triangleVertices[0][1], triangleVertices[0][2]); // Bottom-left
                    gl.Vertex(triangleVertices[1][0], triangleVertices[1][1], triangleVertices[1][2]); // Bottom-right
                    gl.Vertex(triangleVertices[2][0], triangleVertices[2][1], triangleVertices[2][2]); // Top
                    gl.End();
                }
            }
            // Disable polygon offset after rendering
            gl.Disable(OpenGL.GL_POLYGON_OFFSET_FILL);
        }

        internal static void RenderVertices(OpenGL gl, CModel model)
        {
            const float vertexSize = 5.0f; // Point size in pixels

            gl.Enable(OpenGL.GL_POINT_SMOOTH); // Enable smoother points (optional)
            gl.PointSize(vertexSize);

            List<CGeoset> geosets = model.Geosets.ObjectList;

            gl.Begin(OpenGL.GL_POINTS);
            foreach (CGeoset geo in geosets)
            {
                if (!geo.isVisible) continue;

                foreach (var vertex in geo.Vertices)
                {
                    var position = vertex.Position;

                    // Fixed colors (Red = Selected, Blue = Normal)
                    if (vertex.isSelected)
                        gl.Color(1.0f, 0.0f, 0.0f); // Red
                    else
                        gl.Color(0.0f, 0.0f, 1.0f); // Blue

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
        internal static void HandleLighting(OpenGL gl)
        {
            if (RenderSettings.EnableLighting)
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
