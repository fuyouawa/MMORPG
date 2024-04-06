using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerHSelector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        [HideInInspector] public bool overrideColors = false;
        [HideInInspector] public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private List<GameObject> images = new List<GameObject>();
        [SerializeField] private List<GameObject> imagesHighlighted = new List<GameObject>();
        [SerializeField] private List<GameObject> texts = new List<GameObject>();

        Color latestColor;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateSelector();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateSelector(); }
        }

        void UpdateSelector()
        {
            if (overrideColors == false && latestColor != UIManagerAsset.selectorColor)
            {
                for (int i = 0; i < images.Count; ++i)
                {
                    Image currentImage = images[i].GetComponent<Image>();
                    currentImage.color = new Color(UIManagerAsset.selectorColor.r, UIManagerAsset.selectorColor.g, UIManagerAsset.selectorColor.b, currentImage.color.a);
                }

                for (int i = 0; i < imagesHighlighted.Count; ++i)
                {
                    Image currentAlphaImage = imagesHighlighted[i].GetComponent<Image>();
                    currentAlphaImage.color = new Color(UIManagerAsset.selectorHighlightedColor.r, UIManagerAsset.selectorHighlightedColor.g, UIManagerAsset.selectorHighlightedColor.b, currentAlphaImage.color.a);
                }

                latestColor = UIManagerAsset.selectorColor;
            }

            for (int i = 0; i < texts.Count; ++i)
            {
                TextMeshProUGUI currentText = texts[i].GetComponent<TextMeshProUGUI>();

                if (overrideColors == false) { currentText.color = new Color(UIManagerAsset.selectorColor.r, UIManagerAsset.selectorColor.g, UIManagerAsset.selectorColor.b, currentText.color.a); }
                if (overrideFonts == false) { currentText.font = UIManagerAsset.selectorFont; }
            }
        }
    }
}