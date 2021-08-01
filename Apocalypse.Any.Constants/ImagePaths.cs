using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Apocalypse.Any.Constants
{
    public static class ImagePaths
    {
        public const int empty = 0;
        public const int gamesheetExtended = 1;
        public const int faces = 2;
        public const int ships = 3;
        public const int fog_edit = 4;
        public const int mediumShips_edit = 5;
        // public const int projectile = gamesheetExtended;
        public const int stars = 6;
        public const int debris = 7;
        public const int hud_misc_edit = 8;
        public const int apocalypse_logo = 9;
        public const int game_over_glitch = 10;
        public const int dialogue = 11;
        public const int miniCity = 12;
        public const int miniCity2 = 13;
        public const int blank = 14;
        public const int planetsRandom0_edit = 15;
        public const int planetsRandom1_edit = 16;
        public const int planetsRandom2_edit = 17;
        public const int planetsRandom3_edit = 18;
        public const int planetsRandom4_edit = 19;
        
        
        
        private static Dictionary<int, string> InternalDictionary = new Dictionary<int, string>()
        {
            { blank, "Image/blank" },
            { empty ,  "Image/blank"},
            { gamesheetExtended , "Image/gamesheetExtended"},
            { faces , "Image/faces"},
            { ships , "Image/ships"},
            { fog_edit , "Image/fog_edit"},
            { stars  , "Image/Scene/Star256x256" },
            // { projectile , "Image/projectile"},
            { mediumShips_edit, "Image/mediumShips_edit"},
            { planetsRandom0_edit ,  "Image/planetsRandom0_edit"},
            { planetsRandom1_edit ,  "Image/planetsRandom1_edit"},
            { planetsRandom2_edit ,  "Image/planetsRandom2_edit"},
            { planetsRandom3_edit ,  "Image/planetsRandom3_edit"},
            { planetsRandom4_edit ,  "Image/planetsRandom4_edit"},
            { debris, "Image/debris"},
            { hud_misc_edit, "Image/hud_misc_edit"},
            { apocalypse_logo, "Image/SplashScreen/apocalypse_logo" },
            { game_over_glitch, "Image/SplashScreen/game_over_glitch" },
            { dialogue, "Image/dialogue"},
            { miniCity,"Image/miniCity" },
            { miniCity2, "Image/miniCity2" },
        };

        
        public static string ConvertToString(int path) => InternalDictionary[path];
        
        public const int UndefinedFrame = 0;
        public const int MiniCityImagePath = 1;
        public const int PlayerFrame = 2;
        public const int EnemyFrame = 3;
        public const int PlanetFrame = 4;
        public const int FaceFrame = 5;
        public const int FogFrame = 6;
        public const int HUDFrame = 7;
        public const int ShipsFrame = 8;
        public const int MediumShipFrame = 9;
        public const int MineralFrame = 10;
        public const int ExplosionFrame = 11;
        public const int AsteroidFrame = 12;
        public const int ProjectileFrame = 13;
        public const int DialogueFrame = 14;
        public const int ThrustFrame = 15;
        public const int ItemFrame = 16;
        public const int RandomPlanetFrame0 = 17;
        public const int RandomPlanetFrame1 = 18;
        public const int RandomPlanetFrame2 = 19;
        public const int RandomPlanetFrame3 = 20;
        public const int DefaultFrame = 21;
    }
}

