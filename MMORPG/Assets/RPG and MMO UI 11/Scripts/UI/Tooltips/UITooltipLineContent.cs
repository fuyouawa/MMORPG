using System;

namespace DuloGames.UI
{
    [Serializable]
    public class UITooltipLineContent
    {
        public UITooltipLines.LineStyle LineStyle;
        public string CustomLineStyle;
        public string Content;
        public bool IsSpacer;

        public UITooltipLineContent()
        {
            this.LineStyle = UITooltipLines.LineStyle.Default;
            this.CustomLineStyle = string.Empty;
            this.Content = string.Empty;
            this.IsSpacer = false;
        }
    }
}
