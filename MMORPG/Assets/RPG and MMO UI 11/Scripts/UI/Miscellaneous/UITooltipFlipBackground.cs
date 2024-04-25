using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class UITooltipFlipBackground : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UITooltip m_Tooltip;
        [SerializeField] private Graphic m_Graphic;
        #pragma warning restore 0649

        private UIFlippable m_Flippable;

        protected void Awake()
        {
            if (this.m_Graphic == null || this.m_Tooltip == null)
                return;

            this.m_Flippable = this.m_Graphic.gameObject.GetComponent<UIFlippable>();

            if (this.m_Flippable == null)
                this.m_Flippable = this.m_Graphic.gameObject.AddComponent<UIFlippable>();
        }

        protected void OnEnable()
        {
            if (this.m_Graphic == null || this.m_Tooltip == null)
                return;

            this.m_Tooltip.onAnchorEvent.AddListener(OnAnchor);
        }

        protected void OnDisable()
        {
            if (this.m_Graphic == null || this.m_Tooltip == null)
                return;

            this.m_Tooltip.onAnchorEvent.RemoveListener(OnAnchor);
        }

        public void OnAnchor(UITooltip.Anchor anchor)
        {
            if (this.m_Graphic == null || this.m_Flippable == null)
                return;

            switch (anchor)
            {
                case UITooltip.Anchor.Left:
                    this.m_Flippable.horizontal = false;
                    break;
                case UITooltip.Anchor.Right:
                    this.m_Flippable.horizontal = true;
                    break;
                case UITooltip.Anchor.Bottom:
                    this.m_Flippable.vertical = false;
                    break;
                case UITooltip.Anchor.Top:
                    this.m_Flippable.vertical = true;
                    break;
                case UITooltip.Anchor.BottomLeft:
                    this.m_Flippable.horizontal = false;
                    this.m_Flippable.vertical = false;
                    break;
                case UITooltip.Anchor.BottomRight:
                    this.m_Flippable.horizontal = true;
                    this.m_Flippable.vertical = false;
                    break;
                case UITooltip.Anchor.TopLeft:
                    this.m_Flippable.horizontal = false;
                    this.m_Flippable.vertical = true;
                    break;
                case UITooltip.Anchor.TopRight:
                    this.m_Flippable.horizontal = true;
                    this.m_Flippable.vertical = true;
                    break;
            }
        }
    }
}
