using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class UITooltipFlipTransform : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UITooltip m_Tooltip;
        [SerializeField] private RectTransform m_Transform;
        #pragma warning restore 0649

        private Vector2 m_OriginalPivot;
        private Vector2 m_OriginalAnchorMin;
        private Vector2 m_OriginalAnchorMax;
        private Vector2 m_OriginalPosition;

        protected void Awake()
        {
            if (this.m_Transform == null || this.m_Tooltip == null)
                return;
            
            this.m_OriginalPivot = this.m_Transform.pivot;
            this.m_OriginalAnchorMin = this.m_Transform.anchorMin;
            this.m_OriginalAnchorMax = this.m_Transform.anchorMax;
            this.m_OriginalPosition = this.m_Transform.anchoredPosition;
        }

        protected void OnEnable()
        {
            if (this.m_Transform == null || this.m_Tooltip == null)
                return;

            this.m_Tooltip.onAnchorEvent.AddListener(OnAnchor);
        }

        protected void OnDisable()
        {
            if (this.m_Transform == null || this.m_Tooltip == null)
                return;

            this.m_Tooltip.onAnchorEvent.RemoveListener(OnAnchor);
        }

        public void OnAnchor(UITooltip.Anchor anchor)
        {
            if (this.m_Transform == null)
                return;

            switch (anchor)
            {
                case UITooltip.Anchor.Left:
                case UITooltip.Anchor.BottomLeft:
                case UITooltip.Anchor.Bottom:
                    this.m_Transform.pivot = this.m_OriginalPivot;
                    this.m_Transform.anchorMin = this.m_OriginalAnchorMin;
                    this.m_Transform.anchorMax = this.m_OriginalAnchorMax;
                    this.m_Transform.anchoredPosition = this.m_OriginalPosition;
                    break;
                case UITooltip.Anchor.TopLeft:
                    this.m_Transform.pivot = new Vector2(this.m_OriginalPivot.x, (this.m_OriginalPivot.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMin = new Vector2(this.m_OriginalAnchorMin.x, (this.m_OriginalAnchorMin.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMax = new Vector2(this.m_OriginalAnchorMax.x, (this.m_OriginalAnchorMax.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchoredPosition = new Vector2(this.m_OriginalPosition.x, this.m_OriginalPosition.y * -1f);
                    break;
                case UITooltip.Anchor.Right:
                case UITooltip.Anchor.BottomRight:
                    this.m_Transform.pivot = new Vector2((this.m_OriginalPivot.x == 0f) ? 1f : 0f, this.m_OriginalPivot.y);
                    this.m_Transform.anchorMin = new Vector2((this.m_OriginalAnchorMin.x == 0f) ? 1f : 0f, this.m_OriginalAnchorMin.y);
                    this.m_Transform.anchorMax = new Vector2((this.m_OriginalAnchorMax.x == 0f) ? 1f : 0f, this.m_OriginalAnchorMax.y);
                    this.m_Transform.anchoredPosition = new Vector2(this.m_OriginalPosition.x * -1f, this.m_OriginalPosition.y);
                    break;
                case UITooltip.Anchor.TopRight:
                    this.m_Transform.pivot = new Vector2((this.m_OriginalPivot.x == 0f) ? 1f : 0f, (this.m_OriginalPivot.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMin = new Vector2((this.m_OriginalAnchorMin.x == 0f) ? 1f : 0f, (this.m_OriginalAnchorMin.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMax = new Vector2((this.m_OriginalAnchorMax.x == 0f) ? 1f : 0f, (this.m_OriginalAnchorMax.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchoredPosition = new Vector2(this.m_OriginalPosition.x * -1f, this.m_OriginalPosition.y * -1f);
                    break;
                case UITooltip.Anchor.Top:
                    this.m_Transform.pivot = new Vector2(this.m_OriginalPivot.x, (this.m_OriginalPivot.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMin = new Vector2(this.m_OriginalAnchorMin.x, (this.m_OriginalAnchorMin.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchorMax = new Vector2(this.m_OriginalAnchorMax.x, (this.m_OriginalAnchorMax.y == 0f) ? 1f : 0f);
                    this.m_Transform.anchoredPosition = new Vector2(this.m_OriginalPosition.x, this.m_OriginalPosition.y * -1f);
                    break;
            }
        }
    }
}
