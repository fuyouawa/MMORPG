using UnityEngine;
using UnityEngine.UI;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Modern UI Pack/Image/Icon Manager")]
    [RequireComponent(typeof(Image))]
    public class IconManager : MonoBehaviour
    {
        // Resources
        public IconLibrary iconLibrary;

        // Info
        public string selectedIconID;
        public int selectedIconIndex;
        [Range(0, 3)] public int spriteSize;

        Image imageObject;
        [HideInInspector] public string currentSize;
        [HideInInspector] public bool size32;
        [HideInInspector] public bool size64;
        [HideInInspector] public bool size128;
        [HideInInspector] public bool size256;

        void Awake()
        {
            try
            {
                if (iconLibrary == null) { iconLibrary = Resources.Load<IconLibrary>("Icon Library"); }
                if (imageObject == null) { imageObject = gameObject.GetComponent<Image>(); }

                this.enabled = true;
                UpdateElement();
            }

            catch { Debug.LogWarning("<b>Icon Library</b> is missing, but it should be assigned.", this); }
        }

        void Update()
        {
            if (iconLibrary.alwaysUpdate == true) { UpdateElement(); }
            if (Application.isPlaying == true && iconLibrary.optimizeUpdates == true) { this.enabled = false; }
        }

        public void UpdateElement()
        {
            if (iconLibrary == null)
            {
                this.enabled = false;
                return;
            }

            for (int i = 0; i < iconLibrary.icons.Count; i++)
            {
                if (selectedIconID == iconLibrary.icons[i].iconTitle && gameObject.activeInHierarchy == true)
                {
                    if (spriteSize == 0) { imageObject.sprite = iconLibrary.icons[i].iconSprite32; }
                    else if (spriteSize == 1) { imageObject.sprite = iconLibrary.icons[i].iconSprite64; }
                    else if (spriteSize == 2) { imageObject.sprite = iconLibrary.icons[i].iconSprite128; }
                    else if (spriteSize == 3) { imageObject.sprite = iconLibrary.icons[i].iconSprite256; }
                    break;
                }
            }

            if (iconLibrary.alwaysUpdate == false)
                this.enabled = false;
        }

        public void UpdateSpriteSize(int spriteIndex, int newSize)
        {
            if (newSize == 0) { imageObject.sprite = iconLibrary.icons[spriteIndex].iconSprite32; }
            else if (newSize == 1) { imageObject.sprite = iconLibrary.icons[spriteIndex].iconSprite64; }
            else if (newSize == 2) { imageObject.sprite = iconLibrary.icons[spriteIndex].iconSprite128; }
            else if (newSize == 3) { imageObject.sprite = iconLibrary.icons[spriteIndex].iconSprite256; }
        }

        public void ChangeIcon(string newSprite, int preferredSize)
        {
            int selectedSpriteIndex = -1;

            for (int i = 0; i < iconLibrary.icons.Count; i++)
            {
                if (newSprite == iconLibrary.icons[i].iconTitle)
                {
                    selectedSpriteIndex = i;
                    break;
                }
            }

            if (selectedSpriteIndex != -1) { UpdateSpriteSize(selectedSpriteIndex, preferredSize); }
            else { Debug.Log("<b>[Icon Manager]</b> Cannot find an icon named '" + newSprite + "'"); }
        }
    }
}