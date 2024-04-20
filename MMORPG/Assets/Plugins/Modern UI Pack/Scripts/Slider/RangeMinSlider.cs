using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    public class RangeMinSlider : Slider
    {
        [Header("RESOURCES")]
        public RangeMaxSlider maxSlider;
        public TextMeshProUGUI label;
        public string numberFormat;

        protected override void Set(float input, bool sendCallback)
        {
            if (maxSlider == null)
                maxSlider = transform.parent.Find("Max Slider").GetComponent<RangeMaxSlider>();

            float newValue = input;
            
            if (wholeNumbers == true)
                newValue = Mathf.Round(newValue);

            if (newValue >= maxSlider.realValue && maxSlider.realValue != maxSlider.minValue)
                return;

            if (label != null)
                label.text = newValue.ToString(numberFormat);
            
            base.Set(input, sendCallback);
        }

        public void Refresh(float input)
        {
            Set(input, false);
        }
    }
}