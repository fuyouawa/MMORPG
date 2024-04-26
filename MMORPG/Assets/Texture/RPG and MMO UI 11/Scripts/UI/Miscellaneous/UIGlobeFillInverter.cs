using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class UIGlobeFillInverter : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Image m_Image;
        #pragma warning restore 0649

        public void OnChange(float value)
        {
            if (this.m_Image != null)
            {
                this.m_Image.fillAmount = 1f - value;
            }
        }
    }
}
