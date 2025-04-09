using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public static class CopiedKeyframesData
    {
        public static TransformationType CopiedNodeKeyframeType = TransformationType.None;
        public static bool Cut = false;
        public static INode CopiedNode;
        public static CSequence Sequence;
    }
}
