using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    internal static class ColorCollecton
    {
        public static List<string> TC = new();
        public static List<string> TG = new();
        public static List<string> Glows = new();
        
        public static List<string> Stars = new();
        
        public static void Free()
        {
            TC = new ();
            TG = new();
            Glows = new();
            Stars = new();
        }
        public static void Init()
        {
            TC = new List<string>()
            {
                "ReplaceableTextures\\TeamColor\\TeamColor00.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor01.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor02.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor03.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor04.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor05.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor06.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor07.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor08.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor09.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor10.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor11.blp",
                "ReplaceableTextures\\TeamColor\\TeamColor12.blp",
                "Textures\\White_64_Foam1.blp"
            };
            TG = new List<string>()
            {
                "ReplaceableTextures\\TeamGlow\\TeamGlow00.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow01.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow02.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow03.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow04.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow05.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow06.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow07.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow08.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow09.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow10.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow11.blp",
                "ReplaceableTextures\\TeamGlow\\TeamGlow12.blp",
                "Textures\\GenericGlow64.blp"
            };
            Glows = new List<string>()
            {
                "Textures\\Red_Glow1.blp", // red
                "Textures\\Blue_Glow2.blp", // blue

                "", // teal
                "Textures\\Purple_Glow.blp", // purple
                "UI\\Glues\\SinglePlayer\\HumanCampaign3D\\Yellow_Glow.blp", // yellow
                "", // orange
                "Abilities\\Spells\\Orc\\Berserker\\Green_Glow2.blp", // green
                "", // pink
                "", // grey
                "", // light blue
                "", // light blue
                "", // dark green
                "", // brown
                "", // black
                "UI\\Glues\\SinglePlayer\\NightElfCampaign3D\\NightElfFemaleEyeGlow1.blp" // white
            };
            Stars = new List<string>()
            {
                "Textures\\Red_star2.blp", //red
                "Textures\\Blue_Star2.blp", //red
                "Textures\\Blue_Star2.blp", //red
                "Textures\\Blue_Star.blp", //teal
                "Textures\\Purple_Star.blp", //purple
                "Textures\\Yellow_Star.blp", //yellow
                "UI\\Glues\\SinglePlayer\\OrcCampaign3D\\Yellow_Star_Dim.blp", //orange
                "Textures\\Green_Star.blp", //green
                "", //gray
                "", //light blue
                "", //dark green
                "", //brown
                "", //black
                "Objects\\Spawnmodels\\Demon\\InfernalMeteor\\star2Ax.blp" //whte
            };
        }
    }
}
