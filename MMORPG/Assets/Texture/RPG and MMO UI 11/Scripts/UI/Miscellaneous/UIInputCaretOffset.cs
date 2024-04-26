using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [RequireComponent(typeof(InputField))][DefaultExecutionOrder(100)]
    public class UIInputCaretOffset : MonoBehaviour
    {
        [SerializeField] private Vector2 m_Offset = Vector2.zero;
        private Vector2 m_InitialPosition = Vector2.zero;

        public void ApplyOffset()
        {
            foreach (Transform trans in this.transform)
            {
                if (trans.gameObject.name.ToLower().Contains("caret"))
                {
                    RectTransform rect = (trans as RectTransform);

                    this.m_InitialPosition = rect.anchoredPosition;
                    rect.anchoredPosition = this.m_InitialPosition + this.m_Offset;
                    break;
                }
            }
        }

        public void LateUpdate()
        {
            ApplyOffset();
        }
    }
}
