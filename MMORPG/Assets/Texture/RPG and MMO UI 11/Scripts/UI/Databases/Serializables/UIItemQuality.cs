using UnityEngine;
using System;

namespace DuloGames.UI
{
    [Serializable]
    public enum UIItemQuality : int
    {
        Poor = 0,
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5
    }
    
    public class UIItemQualityColor
    {
        public const string Poor = "9d9d9dff";
        public const string Common = "ffffffff";
        public const string Uncommon = "1eff00ff";
        public const string Rare = "0070ffff";
        public const string Epic = "a335eeff";
        public const string Legendary = "ff8000ff";

        public static string GetHexColor(UIItemQuality quality)
        {
            switch (quality)
            {
                case UIItemQuality.Poor: return Poor;
                case UIItemQuality.Common: return Common;
                case UIItemQuality.Uncommon: return Uncommon;
                case UIItemQuality.Rare: return Rare;
                case UIItemQuality.Epic: return Epic;
                case UIItemQuality.Legendary: return Legendary;
            }

            return Poor;
        }

        public static Color GetColor(UIItemQuality quality)
        {
            return CommonColorBuffer.StringToColor(GetHexColor(quality));
        }
    }
}
