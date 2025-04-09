using MdxLib.Model;
using System;

namespace Wa3Tuner
{
    internal class Duplicator
    {
        internal static CMaterialLayer DuplicateLayer(CMaterialLayer original,   CModel owner)
        {
            CMaterialLayer l = new CMaterialLayer(owner);

            l.Unfogged = original.Unfogged;
            l.Unshaded = original.Unshaded;
            l.TwoSided = original.TwoSided;
            l.NoDepthTest = original.NoDepthTest;
            l.NoDepthSet = original.NoDepthSet;
            l.SphereEnvironmentMap = original.SphereEnvironmentMap;
            l.FilterMode = original.FilterMode;
            if (original.TextureAnimation.Object != null)
            {
                l.TextureAnimation.Attach(original.TextureAnimation.Object);
            }
            if (original.Alpha.Static)
            {
                l.Alpha.MakeStatic(original.Alpha.GetValue());
            }
            else
            {
                l.Alpha.MakeAnimated();
                foreach (var v in original.Alpha)
                {
                    l.Alpha.Add(new MdxLib.Animator.CAnimatorNode<float>(v));
                }
            }
            if (original.TextureId.Static)
            {
                l.TextureId.MakeStatic(original.TextureId.GetValue());
            }
            else
            {
                foreach (var v in original.TextureId)
                {
                    l.TextureId.Add(new MdxLib.Animator.CAnimatorNode<int>(v));
                }
            }
            l.Texture.Attach(original.Texture.Object);

            return l;
        }

        internal static CTextureAnimation DuplicateTextureAnim(CTextureAnimation ta, CModel owner)
        {
            CTextureAnimation copy = new CTextureAnimation(owner);
            if (ta.Translation.Animated)
            {
                copy.Translation.MakeAnimated();
                foreach (var v in ta.Translation)
                {
                    copy.Translation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>( v));
                }
            }
            if (ta.Rotation.Animated)
            {
                copy.Rotation.MakeAnimated();
                foreach (var v in ta.Rotation)
                {
                    copy.Rotation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector4>(v));
                }
            }
            if (ta.Scaling.Animated)
            {
                copy.Scaling.MakeAnimated();
                foreach (var v in ta.Scaling)
                {
                    copy.Scaling.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>(v));
                }
            }

            return copy; ;
        }
    }
}