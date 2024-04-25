using UnityEngine;

namespace DuloGames.UI
{
    public interface IColorSchemeElement
    {
        ColorSchemeShade shade { get; set; }
        void Apply(Color color);
    }
}
