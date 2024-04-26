using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class UISliderDisplayValue : MonoBehaviour
    {
        public enum DisplayValue
        {
            Raw,
            Percentage
        }

        #pragma warning disable 0649
        [SerializeField] private Slider m_slider;
        [SerializeField] private Text m_Text;
        [SerializeField] private DisplayValue m_Display = DisplayValue.Percentage;
        [SerializeField] private string m_Format = "0";
        [SerializeField] private string m_Append = "%";
        #pragma warning restore 0649

        protected void Awake()
        {
            if (this.m_slider == null) this.m_slider = this.gameObject.GetComponent<Slider>();
        }

        protected void OnEnable()
        {
            if (this.m_slider != null)
            {
                this.m_slider.onValueChanged.AddListener(SetValue);
                this.SetValue(this.m_slider.value);
            }
        }

        protected void OnDisable()
        {
            if (this.m_slider != null)
            {
                this.m_slider.onValueChanged.RemoveListener(SetValue);
            }
        }

        public void SetValue(float value)
        {
            if (this.m_Text != null)
            {
                if (this.m_Display == DisplayValue.Percentage)
                    this.m_Text.text = (value * 100f).ToString(this.m_Format) + this.m_Append;
                else
                    this.m_Text.text = value.ToString(this.m_Format) + this.m_Append;
            }
        }
    }
}
