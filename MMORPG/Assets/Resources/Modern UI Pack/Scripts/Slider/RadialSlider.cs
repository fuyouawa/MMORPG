using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    public class RadialSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private const string PREFS_UI_SAVE_NAME = "Radial";

        // Content
        public float currentValue = 50.0f;

        // Resources
        public Image sliderImage;
        public Transform indicatorPivot;
        public TextMeshProUGUI valueText;

        // Settings
        public float minValue = 0;
        public float maxValue = 100;
        [Range(0, 8)] public int decimals;
        public bool isPercent;
        public StartPoint startPoint = StartPoint.Left;

        // Saving
        public bool rememberValue;
        public string sliderTag;

        // Events
        [System.Serializable]
        public class SliderEvent : UnityEvent<float> { }
        [SerializeField]
        public SliderEvent onValueChanged = new SliderEvent();
        public UnityEvent onPointerEnter;
        public UnityEvent onPointerExit;

        private GraphicRaycaster graphicRaycaster;
        private RectTransform hitRectTransform;
        private bool isPointerDown;
        private float currentAngle;
        private float currentAngleOnPointerDown;
        private float valueDisplayPrecision;

        public enum StartPoint { Left, Right, Top, Down }

        public float SliderAngle
        {
            get { return currentAngle; }
            set { currentAngle = Mathf.Clamp(value, 0.0f, 360.0f); }
        }

        // Slider value with applied display precision, i.e. the number of decimals to show.
        public float SliderValue
        {
            get { return (long)(SliderValueRaw * valueDisplayPrecision) / valueDisplayPrecision; }
            set { SliderValueRaw = value; }
        }

        // Raw slider value, i.e. without any display precision applied to its value.
        public float SliderValueRaw
        {
            get { return SliderAngle / 360.0f * maxValue; }
            set { SliderAngle = value * 360.0f / maxValue; }
        }

        private void Awake()
        {
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

            if (graphicRaycaster == null)
                Debug.LogWarning("<b>[Radial Slider]</b> Could not find GraphicRaycaster component in parent.", this);
        }

        private void Start()
        {
            valueDisplayPrecision = Mathf.Pow(10, decimals);

            if (rememberValue == true) { LoadState(); }
            else { SliderAngle = currentValue * 3.6f; }

            SliderValue = currentValue;
            onValueChanged.Invoke(SliderValueRaw);
            UpdateUI();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            hitRectTransform = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();
            isPointerDown = true;
            currentAngleOnPointerDown = SliderAngle;
            HandleSliderMouseInput(eventData, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (HasValueChanged())
                SaveState();

            hitRectTransform = null;
            isPointerDown = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (currentValue >= minValue) { HandleSliderMouseInput(eventData, false); }
            else if (currentValue <= minValue) { SliderValueRaw = minValue; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke();
        }

        public void LoadState()
        {
            currentAngle = PlayerPrefs.GetFloat(sliderTag + PREFS_UI_SAVE_NAME);
        }

        public void SaveState()
        {
            if (!rememberValue)
                return;

            PlayerPrefs.SetFloat(sliderTag + PREFS_UI_SAVE_NAME, currentAngle);
        }

        public void UpdateUI()
        {
            if (SliderValueRaw >= minValue)
            {
                float normalizedAngle = SliderAngle / 360.0f;
                indicatorPivot.transform.localEulerAngles = new Vector3(180.0f, 0.0f, SliderAngle);
                sliderImage.fillAmount = normalizedAngle;

                valueText.text = string.Format("{0}{1}", SliderValue, isPercent ? "%" : "");
                currentValue = SliderValue;
            }
        }

        private bool HasValueChanged()
        {
            return SliderAngle != currentAngleOnPointerDown;
        }

        private void HandleSliderMouseInput(PointerEventData eventData, bool allowValueWrap)
        {
            if (!isPointerDown)
                return;

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(hitRectTransform, eventData.position, eventData.pressEventCamera, out localPos);
            float newAngle = Mathf.Atan2(-localPos.y, localPos.x) * Mathf.Rad2Deg + 180f;

            if (!allowValueWrap)
            {
                currentAngle = SliderAngle;
                bool needsClamping = Mathf.Abs(newAngle - currentAngle) >= 180;

                if (needsClamping)
                    newAngle = currentAngle < newAngle ? 0.0f : 360.0f;
            }

            SliderAngle = newAngle;
            UpdateUI();

            if (HasValueChanged())
                onValueChanged.Invoke(SliderValueRaw);
        }
    }
}