using UnityEngine;
using TMPro;

namespace Michsky.MUIP
{
    public class RangeSlider : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0,2)] public int decimalPlaces = 0;
        public float minValue = 0;
        public float maxValue = 1;
        public bool showLabels = true;
        public bool useWholeNumbers = true;

        [Header("Min Slider")]
        public RangeMinSlider minSlider;
        public TextMeshProUGUI minSliderLabel;

        [Header("Max Slider")]
        public RangeMaxSlider maxSlider;
        public TextMeshProUGUI maxSliderLabel;

        public float CurrentLowerValue { get { return minSlider.value; } }
        public float CurrentUpperValue { get { return maxSlider.realValue; } }

        void Awake()
        {
            if (minSlider == null || maxSlider == null)
                return;

            if (showLabels == true)
            {
                minSlider.label = minSliderLabel;
                minSlider.numberFormat = "n" + decimalPlaces;
                maxSlider.label = maxSliderLabel;
                maxSlider.numberFormat = "n" + decimalPlaces;
            }

            else
            {
                minSliderLabel.gameObject.SetActive(false);
                maxSliderLabel.gameObject.SetActive(false);
            }

            if (useWholeNumbers == true)
            {
                minSlider.wholeNumbers = true;
                maxSlider.wholeNumbers = true;
            }

            minSlider.minValue = minValue;
            minSlider.maxValue = maxValue;
            minSlider.onValueChanged.AddListener(CheckForMinState);

            maxSlider.minValue = minValue;
            maxSlider.maxValue = maxValue;
        }

        public void CheckForMinState(float value)
        {
            if (minSlider.value >= maxSlider.realValue)
            {
                maxSlider.realValue = minSlider.value;
                minSlider.value = maxSlider.realValue - 1;
            }
        }
    }
}