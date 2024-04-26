using UnityEngine;

namespace DuloGames.UI
{
    [ExecuteInEditMode][RequireComponent(typeof(RectTransform))]
    public class UICanvasAspectResize : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Camera m_Camera;
        #pragma warning restore 0649

        private RectTransform m_RectTransform;

        protected void Awake()
        {
            this.m_RectTransform = this.transform as RectTransform;
        }

        void Update()
        {
            if (this.m_Camera == null)
                return;

            this.m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_Camera.aspect * this.m_RectTransform.rect.size.y);
        }
    }
}
