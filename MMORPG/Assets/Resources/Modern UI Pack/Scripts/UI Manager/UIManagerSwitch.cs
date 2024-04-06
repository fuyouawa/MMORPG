using UnityEngine;
using UnityEngine.UI;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerSwitch : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        [HideInInspector] public bool overrideColors = false;

        [Header("Resources")]
        [SerializeField] private Image border;
        [SerializeField] private Image background;
        [SerializeField] private Image handleOn;
        [SerializeField] private Image handleOff;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateSwitch();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateSwitch(); }
        }

        void UpdateSwitch()
        {
            if (overrideColors == false)
            {
                border.color = new Color(UIManagerAsset.switchBorderColor.r, UIManagerAsset.switchBorderColor.g, UIManagerAsset.switchBorderColor.b, border.color.a);
                background.color = new Color(UIManagerAsset.switchBackgroundColor.r, UIManagerAsset.switchBackgroundColor.g, UIManagerAsset.switchBackgroundColor.b, background.color.a);
                handleOn.color = new Color(UIManagerAsset.switchHandleOnColor.r, UIManagerAsset.switchHandleOnColor.g, UIManagerAsset.switchHandleOnColor.b, handleOn.color.a);
                handleOff.color = new Color(UIManagerAsset.switchHandleOffColor.r, UIManagerAsset.switchHandleOffColor.g, UIManagerAsset.switchHandleOffColor.b, handleOff.color.a);
            }
        }
    }
}