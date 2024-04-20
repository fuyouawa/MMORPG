using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerDropdown : MonoBehaviour
    {
        [HideInInspector] public bool overrideColors = false;
        [HideInInspector] public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private UIManager UIManagerAsset;
        [SerializeField] private Image background;
        [SerializeField] private Image contentBackground;
        [SerializeField] private Image mainIcon;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private Image expandIcon;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateDropdown();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateDropdown(); }
        }

        void UpdateDropdown()
        {
            if (overrideFonts == false && mainText != null) { mainText.font = UIManagerAsset.dropdownFont; }
            if (overrideColors == false)
            {
                if (background != null) { background.color = UIManagerAsset.dropdownBackgroundColor; }
                if (contentBackground != null) { contentBackground.color = UIManagerAsset.dropdownContentBackgroundColor; }
                if (mainIcon != null) { mainIcon.color = UIManagerAsset.dropdownPrimaryColor; }
                if (mainText != null) { mainText.color = UIManagerAsset.dropdownPrimaryColor; }
                if (expandIcon != null) { expandIcon.color = UIManagerAsset.dropdownPrimaryColor; }
            }
        }
    }
}