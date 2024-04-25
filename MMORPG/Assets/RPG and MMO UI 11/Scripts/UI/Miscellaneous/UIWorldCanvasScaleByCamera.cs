using UnityEngine;

namespace DuloGames.UI
{
    [ExecuteInEditMode]
    public class UIWorldCanvasScaleByCamera : MonoBehaviour {
    
        #pragma warning disable 0649
        [SerializeField] private Camera m_Camera;
        [SerializeField] private Canvas m_Canvas;
        #pragma warning restore 0649

        protected void Update()
        {
            if (this.m_Camera == null || this.m_Canvas == null)
                return;

            float camHeight;
            float distanceToMain = Vector3.Distance(this.m_Camera.transform.position, this.m_Canvas.transform.position);

            if (this.m_Camera.orthographic)
                camHeight = this.m_Camera.orthographicSize * 2.0f;
            else
                camHeight = 2.0f * distanceToMain * Mathf.Tan((this.m_Camera.fieldOfView * 0.5f) * Mathf.Deg2Rad);

            float scaleFactor = Screen.height / (this.m_Canvas.transform as RectTransform).rect.height;
            float scale = (camHeight / Screen.height) * scaleFactor;

            this.m_Canvas.transform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }
}
