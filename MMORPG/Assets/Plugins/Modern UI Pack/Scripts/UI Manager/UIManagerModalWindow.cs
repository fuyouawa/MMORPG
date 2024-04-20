using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerModalWindow : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private Image contentBackground;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateModalWindow();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateModalWindow(); }
        }

        void UpdateModalWindow()
        {
            if (background != null) { background.color = UIManagerAsset.modalWindowBackgroundColor; }
            if (contentBackground != null) { contentBackground.color = UIManagerAsset.modalWindowContentPanelColor; }
            if (icon != null) { icon.color = UIManagerAsset.modalWindowIconColor; }
            if (title != null) { title.color = UIManagerAsset.modalWindowTitleColor; title.font = UIManagerAsset.modalWindowTitleFont; }
            if (description != null) { description.color = UIManagerAsset.modalWindowDescriptionColor; description.font = UIManagerAsset.modalWindowContentFont; }
        }
    }
}