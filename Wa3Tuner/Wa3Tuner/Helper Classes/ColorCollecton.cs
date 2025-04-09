using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    internal static class ColorCollecton
    {
        public static List<string> TC;
        public static List<string> TG;
        public static List<string> Glows;

        public static List<string> Stars;
        
        public static void Free()
        {
            TC = null;
            TG = null;
            Glows = null;
            Stars = null;
        }
        public static void Init()
        {
            TC = new List<string>();
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor00.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor01.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor02.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor03.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor04.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor05.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor06.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor07.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor08.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor09.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor10.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor11.blp");
            TC.Add("ReplaceableTextures\\TeamColor\\TeamColor12.blp");
            TC.Add("Textures\\White_64_Foam1.blp");
            TG = new List<string>();
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow00.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow01.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow02.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow03.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow04.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow05.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow06.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow07.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow08.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow09.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow10.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow11.blp");
            TG.Add("ReplaceableTextures\\TeamGlow\\TeamGlow12.blp");
            TG.Add("Textures\\GenericGlow64.blp");
            Glows = new List<string>();
            Glows.Add("Textures\\Red_Glow1.blp"); // red
            Glows.Add("Textures\\Blue_Glow2.blp"); // blue
            
            Glows.Add(""); // teal
            Glows.Add("Textures\\Purple_Glow.blp"); // purple
            Glows.Add("UI\\Glues\\SinglePlayer\\HumanCampaign3D\\Yellow_Glow.blp"); // yellow
            Glows.Add(""); // orange
            Glows.Add("Abilities\\Spells\\Orc\\Berserker\\Green_Glow2.blp"); // green
            Glows.Add(""); // pink
            Glows.Add(""); // grey
            Glows.Add(""); // light blue
            Glows.Add(""); // light blue
            Glows.Add(""); // dark green
            Glows.Add(""); // brown
            Glows.Add(""); // black
            Glows.Add("UI\\Glues\\SinglePlayer\\NightElfCampaign3D\\NightElfFemaleEyeGlow1.blp"); // white
            Stars = new List<string>();
            Stars.Add("Textures\\Red_star2.blp"); //red
            Stars.Add("Textures\\Blue_Star2.blp"); //red
            Stars.Add("Textures\\Blue_Star2.blp"); //red
            Stars.Add("Textures\\Blue_Star.blp"); //teal
            Stars.Add("Textures\\Purple_Star.blp"); //purple
            Stars.Add("Textures\\Yellow_Star.blp"); //yellow
            Stars.Add("UI\\Glues\\SinglePlayer\\OrcCampaign3D\\Yellow_Star_Dim.blp"); //orange
            Stars.Add("Textures\\Green_Star.blp"); //green
            Stars.Add(""); //gray
            Stars.Add(""); //light blue
            Stars.Add(""); //dark green
            Stars.Add(""); //brown
            Stars.Add(""); //black
            Stars.Add("Objects\\Spawnmodels\\Demon\\InfernalMeteor\\star2Ax.blp"); //whte
        }
    }
}
