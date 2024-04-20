using System.Collections.Generic;
using UnityEngine;

namespace Michsky.MUIP
{
    [CreateAssetMenu(fileName = "New Icon Library", menuName = "Modern UI Pack/New Icon Library")]
    public class IconLibrary : ScriptableObject
    {
        // Settings
        public bool alwaysUpdate = false;
        public bool optimizeUpdates = true;

        // Editor Only
        public Texture2D searchIcon;

        // Library
        public List<IconItem> icons = new List<IconItem>();

        [System.Serializable]
        public class IconItem
        {
            public string iconTitle = "Icon";
            public Texture2D iconPreview;
            public Sprite iconSprite32;
            public Sprite iconSprite64;
            public Sprite iconSprite128;
            public Sprite iconSprite256;
        }
    }
}