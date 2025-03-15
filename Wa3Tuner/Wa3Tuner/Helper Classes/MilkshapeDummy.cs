using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    class MilkshapeDummy
    {
        public List<mMesh> Meshes;
        public List<mBone> Bones;
    }
    public class mMesh
    {
        public List<mVertex> Vertices;
        public List<mTriangle> Triangles;
    }
    class mMaterial
    {

    }
    public class mTriangle
    {
        public mVertex Vertex1, Vertex2, Vertex3;
    }
    public class mVertex
    {
        public Vector3 Positon, Normal;
        public Vector2 TexturePosition;
        public mBone AttachedTo;
       
    }
    public class mKeyframe
    {
        public int Time;
        public Vector3 Data;
    }
    
    public class mBone
    {
        public string Name;
        public mBone Parent;
        public List<mKeyframe> Translation;
        public List<mKeyframe> Rotation;
        public List<mKeyframe> Scaling;
    }


}
