using UnityEngine;
using UnityEngine.UI;

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class UIManagerProgressBarLoop : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool hasBackground;
        public bool useRegularBackground;
        public bool overrideColors = false;

        [Header("Resources")]
        public Image bar;
        [HideInInspector] public Image background;

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

            this.enabled = true;

            if (UIManagerAsset.enableDynamicUpdate == false)
            {
                UpdateProgressBar();
                this.enabled = false;
            }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateProgressBar(); }
        }

        void UpdateProgressBar()
        {
            if (overrideColors == false)
            {
                bar.color = UIManagerAsset.progressBarColor;

                if (hasBackground == true)
                {
                    if (useRegularBackground == true) { background.color = UIManagerAsset.progressBarBackgroundColor; }
                    else { background.color = UIManagerAsset.progressBarLoopBackgroundColor; }
                }
            }
        }
    }
}