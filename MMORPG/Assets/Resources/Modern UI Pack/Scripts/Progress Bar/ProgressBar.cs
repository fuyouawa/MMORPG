using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.MUIP
{
    public class ProgressBar : MonoBehaviour
    {
        // Content
        public float currentPercent;
        [Range(0, 100)] public int speed;
        public float minValue = 0;
        public float maxValue = 100;
        public float valueLimit = 100;

        // Resources
        public Image loadingBar;
        public TextMeshProUGUI textPercent;

        // Settings
        public bool isOn;
        public bool restart;
        public bool invert;
        public bool addPrefix;
        public bool addSuffix = true;
        public string prefix = "";
        public string suffix = "%";
        public bool isLooped = false;
        [Range(0, 5)] public int decimals = 0;

        // Events
        [System.Serializable] 
        public class ProgressBarEvent : UnityEvent<float> { }
        public ProgressBarEvent onValueChanged;
        [HideInInspector] public Slider eventSource;

        void Start()
        {
            UpdateUI();
            InitializeEvents();
        }

        void Update()
        {
            if (isOn == false)
                return;

            if (currentPercent <= maxValue && invert == false) { currentPercent += speed * Time.deltaTime; }
            else if (currentPercent >= minValue && invert == true) { currentPercent -= speed * Time.deltaTime; }

            if (currentPercent >= maxValue && speed != 0 && restart == true && invert == false) { currentPercent = 0; }
            else if (currentPercent <= minValue && speed != 0 && restart == true && invert == true) { currentPercent = maxValue; }
            else if (currentPercent >= maxValue && speed != 0 && restart == false && invert == false) { currentPercent = maxValue; }
            else if (currentPercent <= minValue && speed != 0 && restart == false && invert == true) { currentPercent = minValue; }

            UpdateUI();
        }

        public void UpdateUI()
        {
            loadingBar.fillAmount = currentPercent / maxValue;

            if (addSuffix == true) { textPercent.text = currentPercent.ToString("F" + decimals) + suffix; }
            else { textPercent.text = currentPercent.ToString("F" + decimals); }

            if (addPrefix == true)
                textPercent.text = prefix + textPercent.text;

            if (eventSource != null)
                eventSource.value = currentPercent;
        }

        public void InitializeEvents()
        {
            if (Application.isPlaying == true && onValueChanged.GetPersistentEventCount() != 0)
            {
                if (eventSource == null)
                    eventSource = gameObject.AddComponent(typeof(Slider)) as Slider;

                eventSource.transition = Selectable.Transition.None;
                eventSource.minValue = minValue;
                eventSource.maxValue = maxValue;
                eventSource.onValueChanged.AddListener(onValueChanged.Invoke);
            }
        }

        public void ClearEvents() { eventSource.onValueChanged.RemoveAllListeners(); }
        public void ChangeValue(float newValue) { currentPercent = newValue; UpdateUI(); }
    }
}