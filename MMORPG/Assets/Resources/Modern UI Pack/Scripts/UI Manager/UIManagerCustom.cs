using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Modern UI Pack/UI Manager/UI Manager (Custom Object)")]
    public class UIManagerCustom : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;

        [Header("Settings")]
        public ObjectType objectType;

        [Header("Color")]
        public ColorType colorType = ColorType.Primary;
        public bool keepAlphaValue = false;
        public bool useCustomColor = false;

        [Header("Font")]
        public FontType fontType = FontType.Primary;
        public bool useCustomFont = false;

        Image imageObject;
        TextMeshProUGUI textObject;

        public enum ObjectType
        {
            Text,
            Image
        }

        public enum ColorType
        {
            Primary,
            Secondary
        }

        public enum FontType
        {
            Primary,
            Secondary
        }

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }
            if (!UIManagerAsset.enableDynamicUpdate) { UpdateElement(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate) { UpdateElement(); }
        }

        public void UpdateElement()
        {
            // Get objects
            if (objectType == ObjectType.Image && imageObject == null) { imageObject = gameObject.GetComponent<Image>(); }
            else if (objectType == ObjectType.Text && textObject == null) { textObject = gameObject.GetComponent<TextMeshProUGUI>(); }

            // Check for image
            if (objectType == ObjectType.Image && imageObject != null)
            {
                if (!keepAlphaValue)
                {
                    if (colorType == ColorType.Primary) { imageObject.color = UIManagerAsset.customObjPrimaryColor; }
                    else if (colorType == ColorType.Secondary) { imageObject.color = UIManagerAsset.customObjSecondaryColor; }
                }

                else
                {
                    if (colorType == ColorType.Primary) { imageObject.color = new Color(UIManagerAsset.customObjPrimaryColor.r, UIManagerAsset.customObjPrimaryColor.g, UIManagerAsset.customObjPrimaryColor.b, imageObject.color.a); }
                    else if (colorType == ColorType.Secondary) { imageObject.color = new Color(UIManagerAsset.customObjSecondaryColor.r, UIManagerAsset.customObjSecondaryColor.g, UIManagerAsset.customObjSecondaryColor.b, imageObject.color.a); }
                }
            }

            // Check for text
            else if (objectType == ObjectType.Text && textObject != null)
            {
                if (!useCustomColor)
                {
                    if (!keepAlphaValue)
                    {
                        if (colorType == ColorType.Primary) { textObject.color = UIManagerAsset.customObjPrimaryColor; }
                        else if (colorType == ColorType.Secondary) { textObject.color = UIManagerAsset.customObjSecondaryColor; }
                    }

                    else
                    {
                        if (colorType == ColorType.Primary) { textObject.color = new Color(UIManagerAsset.customObjPrimaryColor.r, UIManagerAsset.customObjPrimaryColor.g, UIManagerAsset.customObjPrimaryColor.b, textObject.color.a); }
                        else if (colorType == ColorType.Secondary) { textObject.color = new Color(UIManagerAsset.customObjSecondaryColor.r, UIManagerAsset.customObjSecondaryColor.g, UIManagerAsset.customObjSecondaryColor.b, textObject.color.a); }
                    }
                }

                if (!useCustomFont)
                {
                    if (fontType == FontType.Primary) { textObject.font = UIManagerAsset.customObjPrimaryFont; }
                    else if (fontType == FontType.Secondary) { textObject.font = UIManagerAsset.customObjSecondaryFont; }
                }
            }
        }
    }
}