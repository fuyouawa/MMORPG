using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    public class RangeMaxSlider : Slider
    {
        public RangeMinSlider minSlider;
        public TextMeshProUGUI label;
        public string numberFormat;

        public float realValue;
        private bool assignedRealValue = false;

        protected override void Start()
        {
            realValue = maxValue;
            base.Start();
        }

        protected override void Set(float input, bool sendCallback)
        {
            if (minSlider == null)
                minSlider = transform.parent.Find("Min Slider").GetComponent<RangeMinSlider>();

            if (!assignedRealValue)
            {
                realValue = maxValue;
                assignedRealValue = true;
            }

            else
                realValue = maxValue - input + minValue;

            if (wholeNumbers == true)
                realValue = Mathf.Round(realValue);

            if (realValue <= minSlider.value)
                return;

            if (label != null)
                label.text = realValue.ToString(numberFormat);

            base.Set(input, sendCallback);
        }

        public void Refresh(float input)
        {
            Set(input, false);
        }
    }
}