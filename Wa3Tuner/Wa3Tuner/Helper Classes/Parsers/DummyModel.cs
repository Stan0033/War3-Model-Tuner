using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public enum dLightType
    {
        Directional, Omnidirectional, Ambient
    }

   public class DummyModel
    {
        public string Name;
        public List<dMesh> Meshes = new List<dMesh>();
        public List<dNode> Nodes = new List<dNode>();
    }

    public class dMesh
    {
        public string Name;
        public dNode AttachedTo;
        public List<dVertex> Vertices = new List<dVertex>();
        public List<dFace> Faces = new List<dFace>();
    }

    public class dFace
    {
        public List<dVertex> Vertices = new List<dVertex>();
    }

    public class dVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexturePosition;
      
    }

    public class dNode
    {
        public string Name;
        public Vector3 PivotPoint;
        public dNode Parent;
        public dTransformation Translation = new dTransformation();
        public dTransformation Rotation = new dTransformation();
        public dTransformation Scaling = new dTransformation();
    }

    public class dLight : dNode
    {
        public dLightType Type;
        public Vector3 Color;
        public float Intensity;
    }

    public class dTransformation
    {
        public List<dKeyframe> List = new List<dKeyframe>();
    }

    public class dKeyframe
    {
        public int Time;
        public Vector3 Data;
    }

      public  static class D3dsParser
    {
        public static CModel ParseAndConvert(string path)
        {
            var dummy = Parse3DS(path);
            CModel model = new CModel();
            Dictionary<CBone, int> ParentIndexing = new Dictionary<CBone, int>();
            foreach (var mesh in dummy.Meshes)
            {
                CGeoset NewGeoset = new CGeoset(model);
                
                foreach (var node in dummy.Nodes)
                {
                    if (node is dLight)
                    {
                        dLight l = (dLight)node;
                        CLight light = new CLight(model);
                        light.Name = node.Name;
                        switch (l.Type)
                        {
                            case dLightType.Ambient: light.Type = MdxLib.Model.ELightType.Ambient; break;
                            case dLightType.Omnidirectional: light.Type = MdxLib.Model.ELightType.Omnidirectional; break;
                            case dLightType.Directional: light.Type = MdxLib.Model.ELightType.Directional; break;
                                
                        }
                        model.Nodes.Add(light);
                    }
                    else
                    {
                        CBone bone = new CBone(model);
                        bone.Name = node.Name;

                        if (node.Translation.List.Count > 0)
                        {

                        }
                        if (node.Rotation.List.Count > 0)
                        {

                        }
                        if (node.Scaling.List.Count > 0)
                        {

                        }
                        model.Nodes.Add(bone);
                    }
                }
                CreateSequencesBasedOnKeyframes(dummy, model);

                foreach (var vertex in mesh.Vertices)
                {
                    CGeosetVertex v = new CGeosetVertex(model);
                    v.Position = new MdxLib.Primitives.CVector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                    v.Normal = new MdxLib.Primitives.CVector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    v.TexturePosition = new MdxLib.Primitives.CVector2(vertex.TexturePosition.X, vertex.TexturePosition.Y);
                    /*  if (vertex.AttachedTo.Count > 0)
                      {
                          CGeosetGroup group = new CGeosetGroup(model);
                          foreach (var node in vertex.AttachedTo)
                          {
                              CGeosetGroupNode gnode = new CGeosetGroupNode(model);
                              CBone bone = new CBone(model);

                             if (model.Nodes.Any(x=>x.Name == node.Name))
                              {
                                  bone = model.Nodes.First(x => x.Name == node.Name) as CBone;
                              }
                              else
                              {
                                  bone.Name = "Generated3DsBone_" + IDCounter.Next_();
                              }
                              gnode.Node.Attach(bone);
                              group.Nodes.Add(gnode);

                          }
                          v.Group.Attach(group);
                          NewGeoset.Groups.Add(group);*/
                }
           
                foreach (var face in mesh.Faces)
                {
                    CGeosetTriangle t = new CGeosetTriangle(model);
                    int index1 =mesh.Vertices.IndexOf(  face.Vertices[0]);
                    int index2 =mesh.Vertices.IndexOf(  face.Vertices[1]);
                    int index3 =mesh.Vertices.IndexOf(  face.Vertices[2]);
                    t.Vertex1.Attach(NewGeoset.Vertices[index1]);
                    t.Vertex2.Attach(NewGeoset.Vertices[index2]);
                    t.Vertex3.Attach(NewGeoset.Vertices[index3]);
                    NewGeoset.Triangles.Add(t);
                }


               
            }
            return model;
        }

        private static void CreateSequencesBasedOnKeyframes(DummyModel model, CModel ToModel)
        {
            List<int> tracks = new List<int>();
            List<KeyValuePair<int, int>> Intervals = new List<KeyValuePair<int, int>>();

            // Collect all unique keyframe times
            foreach (var node in model.Nodes)
            {
                foreach (var keyframe in node.Translation.List)
                {
                    if (!tracks.Contains(keyframe.Time)) tracks.Add(keyframe.Time);
                }
                foreach (var keyframe in node.Rotation.List)
                {
                    if (!tracks.Contains(keyframe.Time)) tracks.Add(keyframe.Time);
                }
                foreach (var keyframe in node.Scaling.List)
                {
                    if (!tracks.Contains(keyframe.Time)) tracks.Add(keyframe.Time);
                }
            }

            // Sort the keyframe times
            tracks.Sort();

            // Create unique intervals based on sorted keyframe times
            if (tracks.Count > 0)
            {
                int start = tracks[0];

                for (int i = 1; i < tracks.Count; i++)
                {
                    if (tracks[i] > tracks[i - 1] + 1) // Detect discontinuity
                    {
                        Intervals.Add(new KeyValuePair<int, int>(start, tracks[i - 1]));
                        start = tracks[i];
                    }
                }

                // Add the last interval
                Intervals.Add(new KeyValuePair<int, int>(start, tracks[^1]));
            }

            // Convert those intervals into sequences
            for (int i = 0; i < Intervals.Count; i++)
            {
                CSequence sequence = new CSequence(ToModel);
                sequence.IntervalStart = Intervals[i].Key;
                sequence.IntervalEnd = Intervals[i].Value;
                sequence.Name = $"Generated Sequence {i}";
                ToModel.Sequences.Add(sequence);
            }
        }

        public static DummyModel Parse3DS(string filename)
        {
            DummyModel model = new DummyModel { Name = Path.GetFileNameWithoutExtension(filename) };

            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ushort chunkId = reader.ReadUInt16();
                    int chunkSize = reader.ReadInt32();

                    switch (chunkId)
                    {
                        case 0x4D4D: // Main chunk
                        case 0x3D3D: // 3D Editor chunk
                            break; // These just contain sub-chunks

                        case 0x4000: // Object block
                            string objectName = ReadNullTerminatedString(reader);
                            dMesh mesh = new dMesh { Name = objectName };
                            model.Meshes.Add(mesh);
                            ReadMeshData(reader, mesh, chunkSize - (objectName.Length + 1 + 6));
                            break;

                        case 0xB000: // Keyframe chunk (animation)
                            ReadKeyframeData(reader, model, chunkSize - 6);
                            break;

                        default:
                            reader.BaseStream.Seek(chunkSize - 6, SeekOrigin.Current);
                            break;
                    }
                }
            }

            return model;
        }

        private static void ReadMeshData(BinaryReader reader, dMesh mesh, int chunkSize)
        {
            long endPos = reader.BaseStream.Position + chunkSize;

            while (reader.BaseStream.Position < endPos)
            {
                ushort chunkId = reader.ReadUInt16();
                int chunkSizeInner = reader.ReadInt32();

                switch (chunkId)
                {
                    case 0x4100: // Mesh data
                        break;

                    case 0x4110: // Vertices
                        int vertexCount = reader.ReadUInt16();
                        for (int i = 0; i < vertexCount; i++)
                        {
                            Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            mesh.Vertices.Add(new dVertex { Position = position });
                        }
                        break;

                    case 0x4120: // Faces
                        int faceCount = reader.ReadUInt16();
                        for (int i = 0; i < faceCount; i++)
                        {
                            dFace face = new dFace
                            {
                                Vertices = new List<dVertex>
                            {
                                mesh.Vertices[reader.ReadUInt16()],
                                mesh.Vertices[reader.ReadUInt16()],
                                mesh.Vertices[reader.ReadUInt16()]
                            }
                            };
                            reader.ReadUInt16(); // Face flags (ignored)
                            mesh.Faces.Add(face);
                        }
                        break;

                    case 0x4160: // Local transformation matrix (Pivot Point)
                        dNode node = new dNode { Name = mesh.Name };
                        node.PivotPoint = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        break;

                    default:
                        reader.BaseStream.Seek(chunkSizeInner - 6, SeekOrigin.Current);
                        break;
                }
            }
        }

        private static void ReadKeyframeData(BinaryReader reader, DummyModel model, int chunkSize)
        {
            long endPos = reader.BaseStream.Position + chunkSize;

            while (reader.BaseStream.Position < endPos)
            {
                ushort chunkId = reader.ReadUInt16();
                int chunkSizeInner = reader.ReadInt32();

                switch (chunkId)
                {
                    case 0xB002: // Object animation
                        string objectName = ReadNullTerminatedString(reader);
                        dNode node = new dNode { Name = objectName };
                        model.Nodes.Add(node);
                        ReadNodeKeyframes(reader, node, chunkSizeInner - (objectName.Length + 1 + 6));
                        break;

                    default:
                        reader.BaseStream.Seek(chunkSizeInner - 6, SeekOrigin.Current);
                        break;
                }
            }
        }

        private static void ReadNodeKeyframes(BinaryReader reader, dNode node, int chunkSize)
        {
            long endPos = reader.BaseStream.Position + chunkSize;

            while (reader.BaseStream.Position < endPos)
            {
                ushort chunkId = reader.ReadUInt16();
                int chunkSizeInner = reader.ReadInt32();

                switch (chunkId)
                {
                    case 0xB020: // Position keyframes
                        int count = reader.ReadUInt16();
                        for (int i = 0; i < count; i++)
                        {
                            int time = reader.ReadInt32();
                            Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            node.Translation.List.Add(new dKeyframe { Time = time, Data = position });
                        }
                        break;

                    case 0xB021: // Rotation keyframes
                        int countRot = reader.ReadUInt16();
                        for (int i = 0; i < countRot; i++)
                        {
                            int time = reader.ReadInt32();
                            float angle = reader.ReadSingle();
                            Vector3 axis = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            node.Rotation.List.Add(new dKeyframe { Time = time, Data = axis * angle });
                        }
                        break;

                    default:
                        reader.BaseStream.Seek(chunkSizeInner - 6, SeekOrigin.Current);
                        break;
                }
            }
        }

        private static string ReadNullTerminatedString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}
