using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class UITooltipFlipEffect : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UITooltip m_Tooltip;
        [SerializeField] private Graphic m_Graphic;
        #pragma warning restore 0649

        private UIFlippable m_Flippable;
        private Vector2 m_OriginalPivot;
        private Vector2 m_OriginalAnchorMin;
        private Vector2 m_OriginalAnchorMax;
        private Vector2 m_OriginalPosition;

        protected void Awake()
        {
            if (this.m_Graphic == null || this.m_Tooltip == null)
                return;

            this.m_Flippable = this.m_Graphic.gameObject.GetComponent<UIFlippable>();

            if (this.m_Flippable == null)
                this.m_Flippable = this.m_Graphic.gameObject.AddComponent<UIFlippable>();

            this.m_OriginalPivot = this.m_Graphic.rectTransform.pivot;
            this.m_OriginalAnchorMin = this.m_Graphic.rectTransform.anchorMin;
            this.m_OriginalAnchorMax = this.m_Graphic.rectTransform.anchorMax;
            this.m_OriginalPosition = this.m_Graphic.rectTransform.anchoredPosition;
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

            RectTransform rt = this.m_Graphic.rectTransform;

            switch (anchor)
            {
                case UITooltip.Anchor.Left:
                case UITooltip.Anchor.BottomLeft:
                case UITooltip.Anchor.TopLeft:
                case UITooltip.Anchor.Bottom:
                    this.m_Flippable.horizontal = false;
                    rt.pivot = this.m_OriginalPivot;
                    rt.anchorMin = this.m_OriginalAnchorMin;
                    rt.anchorMax = this.m_OriginalAnchorMax;
                    rt.anchoredPosition = this.m_OriginalPosition;
                    break;
                case UITooltip.Anchor.Right:
                case UITooltip.Anchor.BottomRight:
                case UITooltip.Anchor.TopRight:
                    this.m_Flippable.horizontal = true;
                    rt.pivot = new Vector2((this.m_OriginalPivot.x == 0f) ? 1f : 0f, this.m_OriginalPivot.y);
                    rt.anchorMin = new Vector2((this.m_OriginalAnchorMin.x == 0f) ? 1f : 0f, this.m_OriginalAnchorMin.y);
                    rt.anchorMax = new Vector2((this.m_OriginalAnchorMax.x == 0f) ? 1f : 0f, this.m_OriginalAnchorMax.y);
                    rt.anchoredPosition = new Vector2(this.m_OriginalPosition.x * -1, this.m_OriginalPosition.y);
                    break;
                case UITooltip.Anchor.Top:
                    this.m_Flippable.vertical = true;
                    rt.pivot = new Vector2(this.m_OriginalPivot.x, (this.m_OriginalPivot.y == 0f) ? 1f : 0f);
                    rt.anchorMin = new Vector2(this.m_OriginalAnchorMin.x, (this.m_OriginalAnchorMin.y == 0f) ? 1f : 0f);
                    rt.anchorMax = new Vector2(this.m_OriginalAnchorMax.x, (this.m_OriginalAnchorMax.y == 0f) ? 1f : 0f);
                    rt.anchoredPosition = new Vector2(this.m_OriginalPosition.x, this.m_OriginalPosition.y * -1);
                    break;
            }
        }
    }
}
