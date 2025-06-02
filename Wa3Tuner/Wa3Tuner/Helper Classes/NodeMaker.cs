using MdxLib.Model;
using MdxLib.Primitives;
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
        public static CParticleEmitter2 ItemPixie = new CParticleEmitter2(Dummy);
        public static CParticleEmitter2 Dust = new CParticleEmitter2(Dummy);
        public static CParticleEmitter2 Smoke = new CParticleEmitter2(Dummy);
        public static CParticleEmitter2 BlastFlare = new CParticleEmitter2(Dummy);
        public static CParticleEmitter2 Fire = new CParticleEmitter2(Dummy);
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
            ItemPixie.ReplaceableId = 0;
            ItemPixie.Columns = 1;
            ItemPixie.EmissionRate.MakeStatic(18);
           
            ItemPixie.Segment1 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255,255,255), 22, 0.1f);
            ItemPixie.Segment2 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255, 255, 255), 255, 0.1f);
            ItemPixie.Segment3 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255, 255, 255), 0, 14.6f);
            ItemPixie.RequiredTexturePath = @"Textures\Yellow_Star_Dim.blp";
            //----------------------------------------------------------------
             Smoke.Segment1 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(102, 102, 102), 70, 13.8f);
            Smoke.Segment2 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(102, 102, 102), 168, 20.7f);
            Smoke.Segment3 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(63, 63, 63), 0, 34.2f);

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
            Smoke.ReplaceableId = 0;
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
             Dust.Segment1 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(10, 107, 181), 139, 19.8f);
            Dust.Segment2 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(68, 133, 154), 225, 27.8f);
            Dust.Segment3 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(212, 228, 233), 0, 27.8f);
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
 

            BlastFlare.Segment1 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255,0,0), 255, 19.8f);
            BlastFlare.Segment2 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255,128, 62), 143f, 0.75f);
            BlastFlare.Segment3 = new MdxLib.Primitives.CSegment(Calculator.RGB2NRRGB(255, 0, 0), 0,1.25f);


            BlastFlare.Rows = 8;
            BlastFlare.Columns = 8;
            BlastFlare.LifeSpan = 0.9f;
            BlastFlare.TailLength = 0.1f;
            BlastFlare.Time = 0.5f;
        }
    }
}
