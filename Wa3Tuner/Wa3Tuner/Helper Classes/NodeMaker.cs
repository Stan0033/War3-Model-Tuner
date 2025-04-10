﻿using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
namespace Wa3Tuner
{
   public   class NodeMaker
    {
        private static  CModel Dummy= new CModel();
        public   CParticleEmitter2 ItemPixie = new CParticleEmitter2(Dummy);
        public   CParticleEmitter2 Dust = new CParticleEmitter2(Dummy);
        public   CParticleEmitter2 Smoke = new CParticleEmitter2(Dummy);
        public   CParticleEmitter2 BlastFlare = new CParticleEmitter2(Dummy);
        public   CParticleEmitter2 Fire = new CParticleEmitter2(Dummy);
        public NodeMaker()
        {






            Fire.Head = true;
            Fire.EmissionRate.MakeStatic(88);
            Fire.Speed.MakeStatic(44);
            Fire.Width.MakeStatic(50);
            Fire.Length.MakeStatic(50);
            Fire.FilterMode = EParticleEmitter2FilterMode.Additive;
            Fire.RequiredTexturePath = @"Textures\Flame4.blp";
            Fire.Segment1.Alpha = 255;
            Fire.Segment2.Alpha = 255;
            Fire.Segment3.Alpha = 255;
            Fire.Segment1.Scaling = 12;
            Fire.Segment2.Scaling = 12;
            Fire.Segment3.Scaling = 12;
            Fire.Rows = 1;
            Fire.Columns = 1;
            Fire.Time = 1;
            Fire.LifeSpan = 1;  



            //---------------------------------------------------------------
            ItemPixie.FilterMode = EParticleEmitter2FilterMode.Additive;
            ItemPixie.Width.MakeStatic(42);
            ItemPixie.Length.MakeStatic(42);
            ItemPixie.Gravity.MakeStatic(150);
            ItemPixie.Speed.MakeStatic(0);
            ItemPixie.Variation.MakeStatic(0);
            ItemPixie.Unshaded = true;
            ItemPixie.ModelSpace = true;
            ItemPixie.Time = 0.5f;
            ItemPixie.TailLength = 0.5f;
            ItemPixie.LifeSpan = 0.7f;
            ItemPixie.Rows = 1;
            ItemPixie.Columns = 1;
            ItemPixie.EmissionRate.MakeStatic(18);
            ItemPixie.Segment1.Alpha = 22;
            ItemPixie.Segment2.Alpha = 255;
            ItemPixie.Segment3.Alpha = 0;
            ItemPixie.Segment1.Scaling = 0.1f;
            ItemPixie.Segment3.Scaling = 0.1f;
            ItemPixie.Segment2.Scaling = 14.6f;
            ItemPixie.RequiredTexturePath = @"Textures\Yellow_Star_Dim.blp";
            //----------------------------------------------------------------
            Smoke.Segment1.Scaling = 13.8f;
            Smoke.Segment2.Scaling = 20.7f;
            Smoke.Segment3.Scaling = 34.2f;
            Smoke.Segment1.Alpha = 70;
            Smoke.Segment2.Alpha = 168;
            Smoke.Segment3.Alpha = 0;
            Smoke.Segment1.Color = new MdxLib.Primitives.CVector3(102,102,102);
            Smoke.Segment2.Color = new MdxLib.Primitives.CVector3(102,102,102);
            Smoke.Segment3.Color = new MdxLib.Primitives.CVector3(63,63,63);
            Smoke.RequiredTexturePath = @"Textures\Dust5A.blp";
            Smoke.FilterMode = EParticleEmitter2FilterMode.Additive;
            Smoke.Unshaded = true;
            Smoke.Variation.MakeStatic(0.39f);
            Smoke.Latitude.MakeStatic(12.5f);
            Smoke.Speed.MakeStatic(80);
            Smoke.EmissionRate.MakeStatic(10);
            Smoke.Width.MakeStatic(25);
            Smoke.Length.MakeStatic(25);
            Smoke.Time = 0.5f;
            Smoke.LifeSpan = 1.635f;
            Smoke.Rows = 1;
            Smoke.Columns = 1;
            Smoke.TailLength = 1;
            //----------------------------------------------------------------
            Dust.RequiredTexturePath = @"Textures\Dust5A.blp";
            Dust.FilterMode = EParticleEmitter2FilterMode.Blend;
            Dust.SortPrimitivesFarZ = true;
            Dust.EmissionRate.MakeStatic(17);
            Dust.Speed.MakeStatic(120);
            Dust.Variation.MakeStatic(0.39f);
            Dust.Latitude.MakeStatic(13.5f);
            Dust.Width.MakeStatic(120);
            Dust.Length.MakeStatic(120);
            Dust.LifeSpan = 1.04f;
            Dust.Rows = 1;
            Dust.Columns = 1;
            Dust.TailLength = 1;
            Dust.Time = 0.5f;
            Dust.Segment1.Color = new MdxLib.Primitives.CVector3(181,107,10);
            Dust.Segment2.Color = new MdxLib.Primitives.CVector3(154,133,68);
            Dust.Segment3.Color = new MdxLib.Primitives.CVector3(233,228,212);
            Dust.Segment1.Alpha = 139;
            Dust.Segment1.Scaling = 19.8f;
            Dust.Segment2.Alpha = 225;
            Dust.Segment2.Scaling = 27.8f;
            Dust.Segment3.Alpha = 0;
            Dust.Segment3.Scaling = 48;
            //----------------------------------------------------------------
            BlastFlare.RequiredTexturePath = @"ReplaceableTextures\Weather\Clouds8x8.blp";
            BlastFlare.EmissionRate.MakeStatic(25);
            BlastFlare.Speed.MakeStatic(120);
            BlastFlare.Latitude.MakeStatic(21);
            BlastFlare.Width.MakeStatic(100);
            BlastFlare.Length.MakeStatic(100);
            BlastFlare.FilterMode = EParticleEmitter2FilterMode.Additive;
            BlastFlare.Unshaded = true;
            BlastFlare.Head = true;
            BlastFlare.Segment1.Color = new MdxLib.Primitives.CVector3(1,1,0);
            BlastFlare.Segment1.Alpha = 255f;
            BlastFlare.Segment1. Scaling = 25;

            BlastFlare.Segment1.Color = new MdxLib.Primitives.CVector3(0.24705882352f, 0.5f,1);
            BlastFlare.Segment1.Alpha = 143f;
            BlastFlare.Segment1.Scaling = 75;

            BlastFlare.Segment1.Color = new MdxLib.Primitives.CVector3(0, 0, 1);
            BlastFlare.Segment1.Alpha = 0f;
            BlastFlare.Segment1.Scaling = 125;
            BlastFlare.Rows = 8;
            BlastFlare.Columns = 8;
            BlastFlare.LifeSpan = 0.9f;
            BlastFlare.TailLength = 0.1f;
            BlastFlare.Time = 0.5f;
        }
    }
}
