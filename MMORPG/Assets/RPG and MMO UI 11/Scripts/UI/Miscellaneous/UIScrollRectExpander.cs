using UnityEngine;

namespace DuloGames.UI
{
    /// <summary>
    /// The scroll rect expander is really simple, you place it on the scroll bar and it monitors when the bar is disabled.
    /// </summary>
    public class UIScrollRectExpander : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float m_ExpandWidth = 0f;
        [SerializeField] private RectTransform m_Target;
        #pragma warning restore 0649

        private bool m_Expanded = false;

        protected void OnEnable()
        {
            if (this.gameObject.activeSelf)
                this.Collapse();
        }

        protected void OnDisable()
        {
            if (!this.gameObject.activeSelf)
                this.Expand();
        }

        private void Expand()
        {
            if (this.m_Expanded || this.m_Target == null)
                return;

            this.m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_Target.rect.width + this.m_ExpandWidth);
            this.m_Expanded = true;
        }

        private void Collapse()
        {
            if (!this.m_Expanded || this.m_Target == null)
                return;

            this.m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_Target.rect.width - this.m_ExpandWidth);
            this.m_Expanded = false;
        }
    }
}
