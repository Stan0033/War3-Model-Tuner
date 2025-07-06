using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;


namespace Wa3Tuner.Helper_Classes
{
    public static class NodePresetHandler
    {
        public static void Save(INode node, string name, string path)
        {
            string saveLocation = Path.Combine(path, name + ".war3SFXp");
            StringBuilder sb = new StringBuilder();
            if (node is CParticleEmitter emitter)
            {
                sb.AppendLine("[emitter1]");
                sb.AppendLine("EmissionRate: " + (emitter.EmissionRate.Static?  emitter.EmissionRate.GetValue().ToString(): "0"));
                sb.AppendLine("LifeSpan: " + (emitter.LifeSpan.Static?  emitter.LifeSpan.GetValue().ToString(): "0"));
                sb.AppendLine("InitialVelocity: " + (emitter.InitialVelocity.Static?  emitter.InitialVelocity.GetValue().ToString(): "0"));
                sb.AppendLine("Gravity: " + (emitter.Gravity.Static?  emitter.Gravity.GetValue().ToString(): "0"));
                sb.AppendLine("Longitude: " + (emitter.Longitude.Static?  emitter.Longitude.GetValue().ToString(): "0"));
                sb.AppendLine("Longitude: " + (emitter.Latitude.Static?  emitter.Latitude.GetValue().ToString(): "0"));
                sb.AppendLine("FileName: " +  emitter.FileName );
                sb.AppendLine("EmitterUsesMdl: " +  emitter.EmitterUsesMdl );
                sb.AppendLine("EmitterUsesTga: " +  emitter.EmitterUsesTga );
            }
           else if (node is CParticleEmitter2 emitter2)
            {
                sb.AppendLine("[emitter2]");
                sb.AppendLine("EmissionRate: " + (emitter2.EmissionRate.Static ? emitter2.EmissionRate.GetValue().ToString() : "0"));
                sb.AppendLine("Speed: " + (emitter2.Speed.Static ? emitter2.Speed.GetValue().ToString() : "0"));
                sb.AppendLine("Variation: " + (emitter2.Variation.Static ? emitter2.Variation.GetValue().ToString() : "0"));
                sb.AppendLine("Latitude: " + (emitter2.Latitude.Static ? emitter2.Latitude.GetValue().ToString() : "0"));
                sb.AppendLine("Width: " + (emitter2.Width.Static ? emitter2.Width.GetValue().ToString() : "0"));
                sb.AppendLine("Length: " + (emitter2.Length.Static ? emitter2.Length.GetValue().ToString() : "0"));
                sb.AppendLine("Gravity: " + (emitter2.Gravity.Static ? emitter2.Gravity.GetValue().ToString() : "0"));
                if (emitter2.Texture.Object!=null) { sb.AppendLine($"Texture: {emitter2.Texture.Object.FileName}");  }
                sb.AppendLine($"Filtermode: {emitter2.FilterMode.ToString()}");
                sb.AppendLine($"Unshaded: {emitter2.Unshaded.ToString()}");
                sb.AppendLine($"Unfogged: {emitter2.Unfogged.ToString()}");
                sb.AppendLine($"LineEmitter: {emitter2.LineEmitter.ToString()}");
                sb.AppendLine($"SortPrimitivesFarZ: {emitter2.SortPrimitivesFarZ.ToString()}");
                sb.AppendLine($"ModelSpace: {emitter2.ModelSpace.ToString()}");
                sb.AppendLine($"Head: {emitter2.Head.ToString()}");
                sb.AppendLine($"Tail: {emitter2.Tail.ToString()}");
                sb.AppendLine($"Squirt: {emitter2.Squirt.ToString()}");
                sb.AppendLine($"Rows: {emitter2.Rows.ToString()}");
                sb.AppendLine($"Columns: {emitter2.Columns.ToString()}");
                sb.AppendLine($"LifeSpan: {emitter2.LifeSpan.ToString()}");
                sb.AppendLine($"TailLength: {emitter2.TailLength.ToString()}");
                sb.AppendLine($"PriorityPlane: {emitter2.PriorityPlane.ToString()}");
                sb.AppendLine($"ReplaceableId: {emitter2.ReplaceableId.ToString()}");
                sb.AppendLine($"Time: {emitter2.Time.ToString()}");

                sb.AppendLine($"HeadLife.Start: {emitter2.HeadLife.Start.ToString()}");
                sb.AppendLine($"HeadLife.End: {emitter2.HeadLife.End.ToString()}");
                sb.AppendLine($"HeadLife.Repeat: {emitter2.HeadLife.Repeat.ToString()}");
                sb.AppendLine($"HeadDecay.Start: {emitter2.HeadDecay.Start.ToString()}");
                sb.AppendLine($"HeadDecay.End: {emitter2.HeadDecay.End.ToString()}");
                sb.AppendLine($"HeadDecay.Repeat: {emitter2.HeadDecay.Repeat.ToString()}");

                sb.AppendLine($"TailLife.Start: {emitter2.TailLife.Start.ToString()}");
                sb.AppendLine($"TailLife.End: {emitter2.TailLife.End.ToString()}");
                sb.AppendLine($"TailLife.Repeat: {emitter2.TailLife.Repeat.ToString()}");
                sb.AppendLine($"TailDecay.Start: {emitter2.TailDecay.Start.ToString()}");
                sb.AppendLine($"TailDecay.End: {emitter2.HeadDecay.End.ToString()}");
                sb.AppendLine($"TailDecay.Repeat: {emitter2.HeadDecay.Repeat.ToString()}");

                sb.AppendLine($"Segment1.Alpha: {emitter2.Segment1.Alpha.ToString()}");
                sb.AppendLine($"Segment1.Color: {emitter2.Segment1.Color.ToString()}");
                sb.AppendLine($"Segment1.Scaling: {emitter2.Segment1.Scaling.ToString()}");

                sb.AppendLine($"Segment2.Alpha: {emitter2.Segment2.Alpha.ToString()}");
                sb.AppendLine($"Segment2.Color: {emitter2.Segment2.Color.ToString()}");
                sb.AppendLine($"Segment2.Scaling: {emitter2.Segment1.Scaling.ToString()}");

                sb.AppendLine($"Segment3.Alpha: {emitter2.Segment3.Alpha.ToString()}");
                sb.AppendLine($"Segment3.Color: {emitter2.Segment3.Color.ToString()}");
                sb.AppendLine($"Segment3.Scaling: {emitter2.Segment3.Scaling.ToString()}");
            }
            else if (node is CLight light)
            {
                sb.AppendLine("[light]");
                sb.AppendLine("Color: "+(light.Color.Static ? light.Color.GetValue().ToString() : "1, 1, 1"));
                sb.AppendLine("AmbientColor: " + (light.AmbientColor.Static ? light.AmbientColor.GetValue().ToString() : "1, 1, 1"));
                sb.AppendLine("AttenuationEnd: " + (light.AttenuationEnd.Static ? light.AttenuationEnd.GetValue().ToString() : "0"));
                sb.AppendLine("AttenuationStart: " + (light.AttenuationStart.Static ? light.AttenuationStart.GetValue().ToString() : "0"));
                sb.AppendLine("Intensity: " + (light.Intensity.Static ? light.Intensity.GetValue().ToString() : "1, 1, 1"));
                sb.AppendLine("AmbientIntensity: " + (light.AmbientIntensity.Static ? light.AmbientIntensity.GetValue().ToString() : "0"));
              sb.AppendLine("Type: " +light.Type.ToString());
            }
            else if (node is CRibbonEmitter ribbon)
            {
                sb.AppendLine("[ribbon]");
                sb.AppendLine("Color: " + (ribbon.Color.Static ? ribbon.Color.GetValue().ToString() : "0"));
                sb.AppendLine("Alpha: " + (ribbon.Alpha.Static ? ribbon.Alpha.GetValue().ToString() : "0"));
                sb.AppendLine("HeightAbove: " + (ribbon.HeightAbove.Static ? ribbon.HeightAbove.GetValue().ToString() : "0"));
                sb.AppendLine("HeightBelow: " + (ribbon.HeightBelow.Static ? ribbon.HeightBelow.GetValue().ToString() : "0"));
                sb.AppendLine("TextureSlot: " + (ribbon.TextureSlot.Static ? ribbon.TextureSlot.GetValue().ToString() : "0"));
                if (ribbon.Material.Object != null)
                {
                 
                    var material = ribbon.Material.Object;
                    if (material.Layers.Count > 0)
                    {
                        if (material.Layers[0].Texture.Object != null)
                        {
                            string filename = material.Layers[0].Texture.Object.FileName;
                            sb.AppendLine($"Material: {filename}");
                        }
                       
                    }
                   
                }
                sb.AppendLine("Columns: " + ribbon.Columns);
                sb.AppendLine("Rows: " + ribbon.Rows);
                sb.AppendLine("EmissionRate: " + ribbon.EmissionRate);
                sb.AppendLine("LifeSpan: " + ribbon.LifeSpan);
                sb.AppendLine("Gravity: " + ribbon.Gravity);
            }
            else if (node is CEvent ev)
            {
               
                sb.AppendLine("[event]");
                sb.AppendLine($"Name: {ev.Name}");
            }
            else if (node is CCollisionShape cols)
            {

                sb.AppendLine("[cols]");
                sb.AppendLine($"Type: {cols.Type}");
                sb.AppendLine($"Radius: {cols.Radius}");
                sb.AppendLine($"Vertex1: {cols.Vertex1}");
                sb.AppendLine($"Vertex2: {cols.Vertex2}");
             
            }
            else
            {
                MessageBox.Show("Node type is not supported for preset"); return;
            }
            if (!Directory.Exists(path))  Directory.CreateDirectory(path);
           
            File.WriteAllText(saveLocation, sb.ToString() );
        }
        public static INode?   Load(string filename, CModel model)
        {
             
            string[] lines = File.ReadAllLines(filename);
            if (lines[0] == "[emitter1]")
            {
                CParticleEmitter emitter = new CParticleEmitter(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");
                    if (parts[0] == "LifeSpan") emitter.LifeSpan.MakeStatic(float.Parse(parts[1]));

                    else if (parts[0] == "EmissionRate") emitter.EmissionRate.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "InitialVelocity") emitter.InitialVelocity.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "Gravity") emitter.Gravity.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "Longitude") emitter.Longitude.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "Latitude") emitter.Latitude.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "FileName") emitter.FileName = parts[1];
                    else if (parts[0] == "EmitterUsesMdl") emitter.EmitterUsesMdl =bool.Parse( parts[1]);
                    else if (parts[0] == "EmitterUsesTga") emitter.EmitterUsesTga =bool.Parse( parts[1]);
                }
                return emitter;
            }
            else if (lines[0] == "[emitter2]")
            {
                CParticleEmitter2 emitter = new CParticleEmitter2(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");

                    if (parts[0] == "Width") emitter.Width.MakeStatic(float.Parse(parts[1]));
                    if (parts[0] == "Length") emitter.Length.MakeStatic(float.Parse(parts[1]));
                    if (parts[0] == "Speed") emitter.Speed.MakeStatic(float.Parse(parts[1]));
                    if (parts[0] == "Variation") emitter.Variation.MakeStatic(float.Parse(parts[1]));
                    if (parts[0] == "Latitude") emitter.Latitude.MakeStatic(float.Parse(parts[1]));
                    if (parts[0] == "Gravity") emitter.Gravity.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "EmissionRate") emitter.EmissionRate.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "Texture") CreateTexture(emitter, parts[1], model);
                    else if (parts[0] == "Unfogged") emitter.Unfogged = bool.Parse(parts[1]);
                    else if (parts[0] == "Unshaded") emitter.Unshaded = bool.Parse(parts[1]);
                    else if (parts[0] == "SortPrimitivesFarZ") emitter.SortPrimitivesFarZ = bool.Parse(parts[1]);
                    else if (parts[0] == "LineEmitter") emitter.LineEmitter = bool.Parse(parts[1]);
                    else if (parts[0] == "Head") emitter.Head = bool.Parse(parts[1]);
                    else if (parts[0] == "Tail") emitter.Tail = bool.Parse(parts[1]);
                    else if (parts[0] == "Squirt") emitter.Squirt = bool.Parse(parts[1]);
                    else if (parts[0] == "ModelSpace") emitter.ModelSpace = bool.Parse(parts[1]);
                    else if (parts[0] == "Rows") emitter.Rows = int.Parse(parts[1]);
                    else if (parts[0] == "TailLength") emitter.TailLength = float.Parse(parts[1]);
                    else if (parts[0] == "Time") emitter.Time = float.Parse(parts[1]);
                    else if (parts[0] == "PriorityPlane") emitter.PriorityPlane = int.Parse(parts[1]);
                    else if (parts[0] == "ReplaceableId") emitter.ReplaceableId = int.Parse(parts[1]);


                    else if (parts[0] == "HeadLife.Start") emitter.HeadLife._Start = int.Parse(parts[1]);  
                    else if (parts[0] == "HeadLife.End") emitter.HeadLife._End = int.Parse(parts[1]);  
                    else if (parts[0] == "HeadLife.Repeat") emitter.HeadLife._Repeat = int.Parse(parts[1]);  

                    else if (parts[0] == "HeadDecay.Start") emitter.HeadDecay._Start = int.Parse(parts[1]);  
                    else if (parts[0] == "HeadDecay.End") emitter.HeadDecay._End = int.Parse(parts[1]);  
                    else if (parts[0] == "HeadDecay.Repeat") emitter.HeadDecay._Repeat = int.Parse(parts[1]);

                    else if (parts[0] == "TailLife.Start") emitter.TailLife._Start = int.Parse(parts[1]);
                    else if (parts[0] == "TailLife.End") emitter.TailLife._End = int.Parse(parts[1]);
                    else if (parts[0] == "TailLife.Repeat") emitter.TailLife._Repeat = int.Parse(parts[1]);

                    else if (parts[0] == "TailDecay.Start") emitter.TailDecay._Start = int.Parse(parts[1]);
                    else if (parts[0] == "TailDecay.End") emitter.TailDecay._End = int.Parse(parts[1]);
                    else if (parts[0] == "TailDecay.Repeat") emitter.TailDecay._Repeat = int.Parse(parts[1]);
                }
                return emitter;
            }
            else if (lines[0] == "[ribbon]")
            {
                CRibbonEmitter emitter = new CRibbonEmitter(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");

                    if (parts[0] == "EmissionRate") emitter.Color.MakeStatic(ExtractVector(parts[1]));
                   
                    else if (parts[0] == "Alpha") emitter.Alpha.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "HeightAbove") emitter.HeightAbove.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "HeightBelow") emitter.HeightBelow.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "TextureSlot") emitter.TextureSlot.MakeStatic(int.Parse(parts[1]));
                    else if (parts[0] == "Columns") emitter.Columns = int.Parse(parts[1]);
                    else if (parts[0] == "Rows") emitter.Rows = int.Parse(parts[1]);
                    else if (parts[0] == "EmissionRate") emitter.EmissionRate = int.Parse(parts[1]);
                    else if (parts[0] == "Gravity") emitter.Gravity = float.Parse(parts[1]);
                     
                }
                return emitter;
            }
            else if (lines[0] == "[light]")
            {
                CLight light = new CLight(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");
                  if (parts[0] == "Color")   light.Color.MakeStatic(ExtractVector(parts[1]));
                    else if (parts[0] == "AmbientColor") light.AmbientColor.MakeStatic(ExtractVector(parts[1]));
                    else if (parts[0] == "AttenuationEnd") light.AttenuationEnd.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "AttenuationStart") light.AttenuationStart.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "Intensity") light.Intensity.MakeStatic(float.Parse(parts[1]));
                    else if (parts[0] == "AmbientIntensity") light.AmbientIntensity.MakeStatic(float.Parse(parts[1]));
                    else  if (parts[0] == "Type") light.Type = GetLightType(parts[1]); 
                }
                return light;
            }
            else if (lines[0] == "[event]")
            {
                CEvent ev = new CEvent(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");
                    if (parts[0] == "Name")ev.Name = parts[1];break;
                }
                if (ev.Name.Length != 8)
                {
                    ev.Name = "INVALID NAME FOR EVENT OBJECT";
                }
                return ev;
            }
            else if (lines[0] == "[cols]")
            {
                CCollisionShape cs = new CCollisionShape(model);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(": ");
                    if (parts.Length == 2)
                    {
                        if (parts[0] == "Type")
                        {
                            if (parts[1] == "Box") { cs.Type = ECollisionShapeType.Box; }
                            else
                            {
                                cs.Type = ECollisionShapeType.Sphere;
                            }
                        }
                        if (parts[0] == "Radius")
                        {
                            cs.Radius = float.Parse(parts[1]);
                        }
                        if (parts[0] == "Vertex1")
                        {

                            cs.Vertex1 = Extractor.GetVertex(parts[1]);
                        }
                        if (parts[0] == "Vertex2")
                        {
                            cs.Vertex2 = Extractor.GetVertex(parts[1]);
                        }
                    }
                }
                return cs;


            }
            return null;
        }

        private static void CreateTexture(CParticleEmitter2 emitter, string v, CModel model)
        {
            string path = v.Trim();
            if (path.Length == 0)return;
            var texture = model.Textures.FirstOrDefault(x=>x.FileName == path);
            if (texture == null)
            {
                CTexture tex = new CTexture(model);
                tex.FileName=path;
                model.Textures.Add(tex);
                emitter.Texture.Attach(tex);
            }
            else
            {
                emitter.Texture.Attach(texture);
            }
        }

        private static MdxLib.Model.ELightType GetLightType(string input)
        {
            if (Enum.TryParse(typeof(MdxLib.Model.ELightType), input, true, out var result))
            {
                return (MdxLib.Model.ELightType)result;
            }

            return MdxLib.Model.ELightType.Omnidirectional;
        }


        

        private static CVector3 ExtractVector(string v)
        {
            string[] parts = v.Split(",");
            
            float one = float.Parse(parts[0].Trim());
            float two = float.Parse(parts[1].Trim());
            float three = float.Parse(parts[2].Trim());
            return new CVector3(one, two, three);
        }
    }
}
