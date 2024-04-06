using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace Michsky.MUIP
{
    public class DemoElementSway : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Resources")]
        [SerializeField] private DemoElementSwayParent swayParent;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private RectTransform swayObject;
        [SerializeField] private CanvasGroup normalCG;
        [SerializeField] private CanvasGroup highlightedCG;
        [SerializeField] private CanvasGroup selectedCG;

        [Header("Settings")]
        [SerializeField] private float smoothness = 10;
        [SerializeField] private float transitionSpeed = 8;
        [SerializeField] [Range(0, 1)] private float dissolveAlpha = 0.5f;

        [Header("Events")]
        [SerializeField] private UnityEvent onClick;

        bool allowSway;
        [HideInInspector] public bool wmSelected;

        Vector3 cursorPos;
        Vector2 defaultPos;

        void Awake()
        {
            if (swayParent == null)
            {
                var tempSway = transform.parent.GetComponent<DemoElementSwayParent>();
                if (tempSway == null) { transform.parent.gameObject.AddComponent<DemoElementSwayParent>(); }
                swayParent = tempSway;
            }

            defaultPos = swayObject.anchoredPosition;
            normalCG.alpha = 1;
            highlightedCG.alpha = 0;
        }

        void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (allowSway == true) { cursorPos = Input.mousePosition; }
#elif ENABLE_INPUT_SYSTEM
            if (allowSway == true) { cursorPos = Mouse.current.position.ReadValue(); }
#endif

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay) { ProcessOverlay(); }
            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera) { ProcessSSC(); }
            else if (mainCanvas.renderMode == RenderMode.WorldSpace) { ProcessWorldSpace(); }
        }

        void ProcessOverlay()
        {
            if (allowSway == true) { swayObject.position = Vector2.Lerp(swayObject.position, cursorPos, Time.deltaTime * smoothness); }
            else { swayObject.localPosition = Vector2.Lerp(swayObject.localPosition, defaultPos, Time.deltaTime * smoothness); }
        }

        void ProcessSSC()
        {
            if (allowSway == true) { swayObject.position = Vector2.Lerp(swayObject.position, Camera.main.ScreenToWorldPoint(cursorPos), Time.deltaTime * smoothness); }
            else { swayObject.localPosition = Vector2.Lerp(swayObject.localPosition, defaultPos, Time.deltaTime * smoothness); }
        }

        void ProcessWorldSpace()
        {
            if (allowSway == true) 
            {
                Vector3 clampedPos = new Vector3(cursorPos.x, cursorPos.y, (mainCanvas.transform.position.z / 6f));
                swayObject.position = Vector3.Lerp(swayObject.position, Camera.main.ScreenToWorldPoint(clampedPos), Time.deltaTime * smoothness);
            }
            else { swayObject.localPosition = Vector3.Lerp(swayObject.localPosition, defaultPos, Time.deltaTime * smoothness); }
        }

        public void Dissolve()
        {
            if (wmSelected == true)
                return;

            StopCoroutine("DissolveHelper");
            StopCoroutine("HighlightHelper");
            StopCoroutine("ActiveHelper");

            StartCoroutine("DissolveHelper");
        }

        public void Highlight()
        {
            if (wmSelected == true)
                return;

            StopCoroutine("DissolveHelper");
            StopCoroutine("HighlightHelper");
            StopCoroutine("ActiveHelper");

            StartCoroutine("HighlightHelper");
        }

        public void Active()
        {
            if (wmSelected == true)
                return;

            StopCoroutine("DissolveHelper");
            StopCoroutine("HighlightHelper");
            StopCoroutine("HighlightHelper");

            StartCoroutine("ActiveHelper");
        }

        public void WindowManagerSelect()
        {
            wmSelected = true;

            StopCoroutine("ActiveHelper");
            StopCoroutine("HighlightHelper");
            StartCoroutine("WMSelectHelper");
        }

        public void WindowManagerDeselect()
        {
            wmSelected = false;

            StartCoroutine("WMDeselectHelper");
            StartCoroutine("DissolveHelper");
        }

        public void OnPointerEnter(PointerEventData data)
        {
            allowSway = true;
            swayParent.DissolveAll(this);
        }

        public void OnPointerExit(PointerEventData data)
        {
            allowSway = false;
            swayParent.HighlightAll();
        }

        public void OnPointerClick(PointerEventData data)
        {
            onClick.Invoke();
        }

        IEnumerator DissolveHelper()
        {
            while (normalCG.alpha > dissolveAlpha)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                highlightedCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                yield return null;
            }

            highlightedCG.alpha = 0;
            normalCG.alpha = dissolveAlpha;
            highlightedCG.gameObject.SetActive(false);
        }

        IEnumerator HighlightHelper()
        {
            while (normalCG.alpha < 1)
            {
                normalCG.alpha += Time.unscaledDeltaTime * transitionSpeed;
                highlightedCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                yield return null;
            }

            normalCG.alpha = 1;
            highlightedCG.alpha = 0;
            highlightedCG.gameObject.SetActive(false);
        }

        IEnumerator ActiveHelper()
        {
            highlightedCG.gameObject.SetActive(true);

            while (highlightedCG.alpha < 1)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                highlightedCG.alpha += Time.unscaledDeltaTime * transitionSpeed;
                yield return null;
            }

            highlightedCG.alpha = 1;
            normalCG.alpha = 0;
        }

        IEnumerator WMSelectHelper()
        {
            selectedCG.gameObject.SetActive(true);

            while (selectedCG.alpha < 1)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                highlightedCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                selectedCG.alpha += Time.unscaledDeltaTime * transitionSpeed;
                yield return null;
            }

            highlightedCG.alpha = 0;
            normalCG.alpha = 0;
            selectedCG.alpha = 1;
        }

        IEnumerator WMDeselectHelper()
        {
            while (selectedCG.alpha > 0)
            {
                selectedCG.alpha -= Time.unscaledDeltaTime * transitionSpeed;
                yield return null;
            }

            selectedCG.alpha = 0;
            selectedCG.gameObject.SetActive(false);
        }
    }
}