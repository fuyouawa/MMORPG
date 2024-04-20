using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerInputField : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI placeholderText;
        [SerializeField] private Image filledImage;
        [SerializeField] private Image backgroundImage;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateInputField();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateInputField(); }
        }

        void UpdateInputField()
        {
            if (overrideColors == false)
            {
                mainText.color = new Color(UIManagerAsset.inputFieldColor.r, UIManagerAsset.inputFieldColor.g, UIManagerAsset.inputFieldColor.b, mainText.color.a);
                placeholderText.color = new Color(UIManagerAsset.inputFieldColor.r, UIManagerAsset.inputFieldColor.g, UIManagerAsset.inputFieldColor.b, placeholderText.color.a);
                filledImage.color = new Color(UIManagerAsset.inputFieldColor.r, UIManagerAsset.inputFieldColor.g, UIManagerAsset.inputFieldColor.b, filledImage.color.a);
                backgroundImage.color = new Color(UIManagerAsset.inputFieldColor.r, UIManagerAsset.inputFieldColor.g, UIManagerAsset.inputFieldColor.b, backgroundImage.color.a);
            }

            if (overrideFonts == false)
            {
                mainText.font = UIManagerAsset.inputFieldFont;
                placeholderText.font = UIManagerAsset.inputFieldFont;
            }
        }
    }
}