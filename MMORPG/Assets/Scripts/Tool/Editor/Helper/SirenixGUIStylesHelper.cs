using UnityEngine;

namespace MMORPG.Tool
{
    public static class SirenixGUIStylesHelper
    {
        public static Font _yaHeiMonacoHybridFont;

        private static GUIStyle _messageBox;

        public static Font YaHeiMonacoHybridFont
        {
            get
            {
                return _yaHeiMonacoHybridFont ??= Resources.Load<Font>("YaHeiMonacoHybrid");
            }
        }

        public static GUIStyle MessageBox
        {
            get
            {
                if (_messageBox == null)
                {
                    _messageBox = new GUIStyle("HelpBox")
                    {
                        margin = new RectOffset(6, 6, 4, 4),
                        fontSize = 12,
                        richText = true,
                        font = YaHeiMonacoHybridFont
                    };
                }
                return _messageBox;
            }
        }
    }
}
