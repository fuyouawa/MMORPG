using UnityEngine;

namespace DuloGames.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class UICanvasAnchorToCamera : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Camera m_Camera;
        #pragma warning restore 0649
        [SerializeField][Range(0f, 1f)] float m_Vertical = 0f;
        [SerializeField][Range(0f, 1f)] float m_Horizontal = 0f;

        private RectTransform m_RectTransform;

        protected void Awake()
        {
            this.m_RectTransform = this.transform as RectTransform;
        }
        
        void Update()
        {
            if (this.m_Camera == null)
                return;

            Vector3 newPos = this.m_Camera.ViewportToWorldPoint(new Vector3(this.m_Horizontal, this.m_Vertical, this.m_Camera.farClipPlane));
            newPos.z = this.m_RectTransform.position.z;
            this.m_RectTransform.position = newPos;
        }
    }
}
