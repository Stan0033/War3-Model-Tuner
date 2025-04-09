using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Ribbon;
namespace Wa3Tuner
{
    public static class NodeCloner
    {
        public static INode Clone(INode node, CModel model)
        {
            INode Clone = null;
            if ( node is CBone bone)
            {
                Clone = new CBone(model);
            }
            if (node is CHelper helper)
            {
                Clone = new CHelper(model);
            }
            if (node is CCollisionShape cs)
            {
                CCollisionShape cols = new CCollisionShape(model);
                cols.Type = cs.Type;
                cols.Radius = cs.Radius;
                cols.Vertex1 = new MdxLib.Primitives.CVector3(cs.Vertex1);
                cols.Vertex2 = new MdxLib.Primitives.CVector3(cs.Vertex2);
                Clone = cols;
                     }
            if (node is CLight light)
            {
                CLight LightClone = new CLight(model);
                LightClone.Type = light.Type;
                if (light.Visibility.Static) { LightClone.Visibility.MakeStatic(light.Visibility.GetValue()); }
                else
                {
                    foreach (var keyframe in light.Visibility) { LightClone.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (light.Color.Static) { LightClone.Color.MakeStatic(new MdxLib.Primitives.CVector3(light.Color.GetValue()) ); }
                else
                {
                    foreach (var keyframe in light.Color) { LightClone.Color.Add(new CAnimatorNode<CVector3>(keyframe.Time, new CVector3(keyframe.Value) )); }
                }
                if (light.AmbientColor.Static) { LightClone.Color.MakeStatic(new MdxLib.Primitives.CVector3(light.AmbientColor.GetValue())); }
                else
                {
                    foreach (var keyframe in light.AmbientColor) { LightClone.Color.Add(new CAnimatorNode<CVector3>(keyframe.Time, new CVector3( keyframe.Value))); }
                }
                if (light.Intensity.Static) { LightClone.Intensity.MakeStatic( light.Intensity.GetValue()); }
                else
                {
                    foreach (var keyframe in light.Intensity) { LightClone.Intensity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value) ); }
                }
                if (light.AmbientIntensity.Static) { LightClone.AmbientIntensity.MakeStatic(light.AmbientIntensity.GetValue()); }
                else
                {
                    foreach (var keyframe in light.Intensity) { LightClone.AmbientIntensity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (light.AttenuationStart.Static) { LightClone.AttenuationStart.MakeStatic(light.AttenuationStart.GetValue()); }
                else
                {
                    foreach (var keyframe in light.AttenuationStart) { LightClone.AttenuationStart.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (light.AttenuationEnd.Static) { LightClone.AttenuationEnd.MakeStatic(light.AttenuationEnd.GetValue()); }
                else
                {
                    foreach (var keyframe in light.AttenuationEnd) { LightClone.AttenuationEnd.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                Clone = LightClone; 
            }
            if (node is CParticleEmitter emitter1)
            {
                CParticleEmitter emitterClone = (CParticleEmitter) node;
                emitterClone.FileName = emitter1.FileName;
                emitterClone.EmitterUsesMdl = emitter1.EmitterUsesMdl;
                emitterClone.EmitterUsesTga = emitter1.EmitterUsesTga;
                if (emitter1.EmissionRate.Static) { emitterClone.EmissionRate.MakeStatic(emitter1.EmissionRate.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.EmissionRate) { emitterClone.EmissionRate.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter1.Gravity.Static) { emitterClone.Gravity.MakeStatic(emitter1.Gravity.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.Gravity) { emitterClone.Gravity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter1.LifeSpan.Static) { emitterClone.LifeSpan.MakeStatic(emitter1.LifeSpan.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.LifeSpan) { emitterClone.LifeSpan.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter1.InitialVelocity.Static) { emitterClone.InitialVelocity.MakeStatic(emitter1.InitialVelocity.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.InitialVelocity) { emitterClone.InitialVelocity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter1.Longitude.Static) { emitterClone.Longitude.MakeStatic(emitter1.Longitude.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.Longitude) { emitterClone.Longitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter1.Latitude.Static) { emitterClone.Latitude.MakeStatic(emitter1.Latitude.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter1.Latitude) { emitterClone.Latitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                Clone = emitterClone;
            }
            if (node is CParticleEmitter2 emitter2)
            {
                CParticleEmitter2 CloneEmitter2 = new CParticleEmitter2(model);
                CloneEmitter2.RequiredTexturePath = emitter2.RequiredTexturePath;
                CloneEmitter2.Unshaded = emitter2.Unshaded;
                CloneEmitter2.Unfogged = emitter2.Unfogged;
                CloneEmitter2.Tail = emitter2.Tail;
                CloneEmitter2.SortPrimitivesFarZ = emitter2.SortPrimitivesFarZ;
                CloneEmitter2.ModelSpace = emitter2.ModelSpace;
                CloneEmitter2.Head = emitter2.Head;
                CloneEmitter2.Squirt = emitter2.Squirt;
                CloneEmitter2.XYQuad = emitter2.XYQuad;
                CloneEmitter2.LineEmitter = emitter2.LineEmitter;
                CloneEmitter2.Time = emitter2.Time;
                CloneEmitter2.Rows = emitter2.Rows;
                CloneEmitter2.Columns = emitter2.Columns;
                CloneEmitter2.LifeSpan = emitter2.LifeSpan;
                CloneEmitter2.TailLength = emitter2.TailLength;
                CloneEmitter2.ReplaceableId = emitter2.ReplaceableId;
                CloneEmitter2.PriorityPlane = emitter2.PriorityPlane;
                CloneEmitter2.FilterMode = emitter2.FilterMode;
                if (model.Textures.Count > 0)  CloneEmitter2.Texture.Attach(model.Textures[0]);
                CloneEmitter2.HeadLife = new CInterval(emitter2.HeadLife);
                CloneEmitter2.HeadDecay = new CInterval(emitter2.HeadDecay);
                CloneEmitter2.TailLife = new CInterval(emitter2.TailLife);
                CloneEmitter2.TailDecay = new CInterval(emitter2.TailDecay);
                CloneEmitter2.Segment1 = new CSegment(emitter2.Segment1);
                CloneEmitter2.Segment2 = new CSegment(emitter2.Segment2);
                CloneEmitter2.Segment3 = new CSegment(emitter2.Segment3);
                if (emitter2.Latitude.Static) { CloneEmitter2.Latitude.MakeStatic(emitter2.Latitude.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Latitude) { CloneEmitter2.Latitude.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Gravity.Static) { CloneEmitter2.Gravity.MakeStatic(emitter2.Gravity.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Gravity) { CloneEmitter2.Gravity.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Speed.Static) { CloneEmitter2.Speed.MakeStatic(emitter2.Speed.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Speed) { CloneEmitter2.Speed.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Width.Static) { CloneEmitter2.Speed.MakeStatic(emitter2.Width.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Width) { CloneEmitter2.Width.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Length.Static) { CloneEmitter2.Length.MakeStatic(emitter2.Width.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Width) { CloneEmitter2.Length.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Variation.Static) { CloneEmitter2.Variation.MakeStatic(emitter2.Width.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Variation) { CloneEmitter2.Variation.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.EmissionRate.Static) { CloneEmitter2.EmissionRate.MakeStatic(emitter2.Width.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.EmissionRate) { CloneEmitter2.EmissionRate.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (emitter2.Visibility.Static) { CloneEmitter2.Visibility.MakeStatic(emitter2.Width.GetValue()); }
                else
                {
                    foreach (var keyframe in emitter2.Visibility) { CloneEmitter2.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                Clone = CloneEmitter2;
            }
            if (node is CRibbonEmitter ribbon)
            {
                CRibbonEmitter ribobnClone = new CRibbonEmitter(model);
                ribobnClone.Rows = ribbon.Rows;
                ribobnClone.Columns = ribbon.Columns;
                ribobnClone.EmissionRate = ribbon.EmissionRate;
                ribobnClone.LifeSpan = ribbon.LifeSpan;
                ribobnClone.Gravity = ribbon.Gravity;
                if (model.Materials.Count > 0) ribobnClone.Material.Attach(model.Materials[0]);
                if (ribbon.Alpha.Static) { ribobnClone.Alpha.MakeStatic(ribbon.Alpha.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.Alpha) { ribobnClone.Alpha.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (ribbon.Visibility.Static) { ribobnClone.Visibility.MakeStatic(ribbon.Visibility.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.Visibility) { ribobnClone.Visibility.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (ribbon.HeightAbove.Static) { ribobnClone.HeightAbove.MakeStatic(ribbon.HeightAbove.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.HeightAbove) { ribobnClone.HeightAbove.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (ribbon.HeightBelow.Static) { ribobnClone.HeightBelow.MakeStatic(ribbon.HeightBelow.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.HeightBelow) { ribobnClone.HeightBelow.Add(new CAnimatorNode<float>(keyframe.Time, keyframe.Value)); }
                }
                if (ribbon.TextureSlot.Static) { ribobnClone.TextureSlot.MakeStatic(ribbon.TextureSlot.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.TextureSlot) { ribobnClone.TextureSlot.Add(new CAnimatorNode<int>(keyframe.Time, keyframe.Value)); }
                }
                if (ribbon.Color.Static) { ribobnClone.Color.MakeStatic(ribbon.Color.GetValue()); }
                else
                {
                    foreach (var keyframe in ribbon.Color) { ribobnClone.Color.Add(new CAnimatorNode<CVector3>(keyframe.Time, new CVector3(keyframe.Value))); }
                }
                Clone = ribobnClone;
            }
                Clone.Billboarded = node.Billboarded;
            Clone.BillboardedLockX = node.BillboardedLockX;
            Clone.BillboardedLockY = node.BillboardedLockY;
            Clone.BillboardedLockZ = node.BillboardedLockZ;
            Clone.CameraAnchored = node.CameraAnchored;
            Clone.DontInheritRotation = node.DontInheritRotation;
            Clone.DontInheritScaling = node.DontInheritScaling;
            Clone.DontInheritTranslation = node.DontInheritTranslation;
            foreach (var keyframe in node.Translation) Clone.Translation.Add(new CAnimatorNode<MdxLib.Primitives.CVector3>(keyframe.Time, new MdxLib.Primitives.CVector3(keyframe.Value)));
            foreach (var keyframe in node.Rotation) Clone.Rotation.Add(new CAnimatorNode<MdxLib.Primitives.CVector4>(keyframe.Time, new MdxLib.Primitives.CVector4(keyframe.Value)));
            foreach (var keyframe in node.Scaling) Clone.Scaling.Add(new CAnimatorNode<MdxLib.Primitives.CVector3>(keyframe.Time, new MdxLib.Primitives.CVector3(keyframe.Value)));
            Clone.Name = node.Name; 
            return Clone;
            
        }
    }
}
