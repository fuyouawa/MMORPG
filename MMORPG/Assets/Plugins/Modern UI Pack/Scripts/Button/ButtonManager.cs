using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.MUIP
{
    [ExecuteInEditMode]
    public class ButtonManager : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Content
        public Sprite buttonIcon;
        public string buttonText = "Button";
        [Range(0.1f, 10)] public float iconScale = 1;
        [Range(10, 200)] public float textSize = 24;

        // Auto Size
        public bool autoFitContent = true;
        public Padding padding;
        [Range(0, 100)] public int spacing = 15;
        public HorizontalLayoutGroup disabledLayout;
        public HorizontalLayoutGroup normalLayout;
        public HorizontalLayoutGroup highlightedLayout;
        [SerializeField] private HorizontalLayoutGroup mainLayout;
        [SerializeField] private ContentSizeFitter mainFitter;
        [SerializeField] private ContentSizeFitter targetFitter;
        [SerializeField] private RectTransform targetRect;

        // Resources
        public CanvasGroup normalCG;
        public CanvasGroup highlightCG;
        public CanvasGroup disabledCG;
        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public TextMeshProUGUI disabledText;
        public Image normalImage;
        public Image highlightImage;
        public Image disabledImage;
        public AudioSource soundSource;
        [SerializeField] private GameObject rippleParent;

        // Settings
        public bool isInteractable = true;
        public bool enableIcon = false;
        public bool enableText = true;
        public bool useCustomContent = false;
        [SerializeField] private bool useCustomTextSize = false;
        public bool checkForDoubleClick = true;
        public bool enableButtonSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;
        public bool useRipple = true;
        [Range(0.1f, 1)] public float doubleClickPeriod = 0.25f;
        [Range(0.25f, 15)] public float fadingMultiplier = 8;
        [SerializeField] private AnimationSolution animationSolution = AnimationSolution.ScriptBased;

        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDoubleClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();

        // Ripple
        [SerializeField] private RippleUpdateMode rippleUpdateMode = RippleUpdateMode.UnscaledTime;
        [SerializeField] private Canvas targetCanvas;
        public Sprite rippleShape;
        [Range(0.1f, 5)] public float speed = 1f;
        [Range(0.5f, 25)] public float maxSize = 4f;
        public Color startColor = new Color(1f, 1f, 1f, 0.2f);
        public Color transitionColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private bool renderOnTop = false;
        [SerializeField] private bool centered = false;

        // Helpers
        bool isInitialized = false;
        Button targetButton;
        bool isPointerOn;
        bool waitingForDoubleClickInput;

#if UNITY_EDITOR
        public bool isPreset;
        public int latestTabIndex = 0;
#endif

        public enum AnimationSolution { Custom, ScriptBased }
        public enum RippleUpdateMode { Normal, UnscaledTime }

        [System.Serializable] public class Padding 
        {
            public int left = 20; 
            public int right = 20;
            public int top = 5;
            public int bottom = 5;
        }

        void OnEnable()
        {
            if (isInitialized == false) { Initialize(); }
            UpdateUI();
        }

        void OnDisable()
        {
            if (isInteractable == false)
                return;

            if (disabledCG != null) { disabledCG.alpha = 0; }
            if (normalCG != null) { normalCG.alpha = 1; }
            if (highlightCG != null) { highlightCG.alpha = 0; }
        }

        void Initialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) { return; }
#endif
            if (animationSolution == AnimationSolution.ScriptBased)
            {
                Animator tempAnimator = GetComponent<Animator>();
                if (tempAnimator != null) { Destroy(tempAnimator); }
            }

            if (gameObject.GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            if (useUINavigation == true) { AddUINavigation(); }
            if (targetCanvas == null) { targetCanvas = GetComponentInParent<Canvas>(); }
            if (normalCG == null) { normalCG = new GameObject().AddComponent<CanvasGroup>(); normalCG.gameObject.AddComponent<RectTransform>(); normalCG.transform.SetParent(transform); normalCG.gameObject.name = "Normal"; }
            if (highlightCG == null) { highlightCG = new GameObject().AddComponent<CanvasGroup>(); highlightCG.gameObject.AddComponent<RectTransform>(); highlightCG.transform.SetParent(transform); highlightCG.gameObject.name = "Highlight"; }
            if (disabledCG == null) { disabledCG = new GameObject().AddComponent<CanvasGroup>(); disabledCG.gameObject.AddComponent<RectTransform>(); disabledCG.transform.SetParent(transform); disabledCG.gameObject.name = "Disabled"; }

            if (useRipple == true && rippleParent != null) { rippleParent.SetActive(false); }
            else if (useRipple == false && rippleParent != null) { Destroy(rippleParent); }

            StartCoroutine("LayoutFix");
            isInitialized = true;
        }

        public void UpdateUI()
        {
            if (autoFitContent == false) 
            {
                if (mainFitter != null) { mainFitter.enabled = false; }
                if (mainLayout != null) { mainLayout.enabled = false; }
                if (targetFitter != null) 
                { 
                    targetFitter.enabled = false;

                    if (targetRect != null)
                    {
                        targetRect.anchorMin = new Vector2(0, 0);
                        targetRect.anchorMax = new Vector2(1, 1);
                        targetRect.offsetMin = new Vector2(0, 0);
                        targetRect.offsetMax = new Vector2(0, 0);
                    }
                }
            }

            else
            {
                if (mainFitter != null) { mainFitter.enabled = true; }
                if (mainLayout != null) { mainLayout.enabled = true; }
                if (targetFitter != null) { targetFitter.enabled = true; }
            }

            if (disabledLayout != null) { disabledLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); disabledLayout.spacing = spacing; }
            if (normalLayout != null) { normalLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); normalLayout.spacing = spacing; }
            if (highlightedLayout != null) { highlightedLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); highlightedLayout.spacing = spacing; }

            if (normalCG != null && isInteractable == true) { normalCG.alpha = 1; }
            if (disabledCG != null && isInteractable == false) { disabledCG.alpha = 1; }
            if (highlightCG != null) { highlightCG.alpha = 0; }

            if (useCustomContent == false)
            {
                // Set Text
                if (enableText == true)
                {
                    if (normalText != null)
                    {
                        normalText.gameObject.SetActive(true);
                        normalText.text = buttonText;
                        if (useCustomTextSize == false) { normalText.fontSize = textSize; }
                    }

                    if (highlightedText != null)
                    {
                        highlightedText.gameObject.SetActive(true);
                        highlightedText.text = buttonText;
                        if (useCustomTextSize == false) { highlightedText.fontSize = textSize; }
                    }

                    if (disabledText != null)
                    {
                        disabledText.gameObject.SetActive(true);
                        disabledText.text = buttonText;
                        if (useCustomTextSize == false) { disabledText.fontSize = textSize; }
                    }
                }

                else if (enableText == false)
                {
                    if (normalText != null) { normalText.gameObject.SetActive(false); }
                    if (highlightedText != null) { highlightedText.gameObject.SetActive(false); }
                    if (disabledText != null) { disabledText.gameObject.SetActive(false); }
                }

                // Set Icon
                if (enableIcon == true)
                {
                    Vector3 tempScale = new Vector3(iconScale, iconScale, iconScale);
                    if (normalImage != null) { normalImage.transform.parent.gameObject.SetActive(true); normalImage.sprite = buttonIcon; normalImage.transform.localScale = tempScale; }
                    if (highlightImage != null) { highlightImage.transform.parent.gameObject.SetActive(true); highlightImage.sprite = buttonIcon; ; highlightImage.transform.localScale = tempScale; }
                    if (disabledImage != null) { disabledImage.transform.parent.gameObject.SetActive(true); disabledImage.sprite = buttonIcon; ; disabledImage.transform.localScale = tempScale; }
                }

                else
                {
                    if (normalImage != null) { normalImage.transform.parent.gameObject.SetActive(false); }
                    if (highlightImage != null) { highlightImage.transform.parent.gameObject.SetActive(false); }
                    if (disabledImage != null) { disabledImage.transform.parent.gameObject.SetActive(false); }
                }
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false && autoFitContent == true)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                if (disabledCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCG.GetComponent<RectTransform>()); }
                if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
                if (highlightCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCG.GetComponent<RectTransform>()); }
            }
#endif

            if (Application.isPlaying == false || gameObject.activeInHierarchy == false) { return; }
            if (isInteractable == false) { StartCoroutine("SetDisabled"); }
            else if (isInteractable == true && disabledCG.alpha == 1) { StartCoroutine("SetNormal"); }

            StartCoroutine("LayoutFix");
        }

        public void SetText(string text) { buttonText = text; UpdateUI(); }
        public void SetIcon(Sprite icon) { buttonIcon = icon; UpdateUI(); }
        
        public void Interactable(bool value) 
        { 
            isInteractable = value;

            if (gameObject.activeInHierarchy == false) { return; }
            if (isInteractable == false) { StartCoroutine("SetDisabled"); }
            else if (isInteractable == true && disabledCG.alpha == 1) { StartCoroutine("SetNormal"); }
        }

        public void AddUINavigation()
        {
            targetButton = gameObject.AddComponent<Button>();
            targetButton.transition = Selectable.Transition.None;
            Navigation customNav = new Navigation();
            customNav.mode = navigationMode;

            if (navigationMode == Navigation.Mode.Vertical || navigationMode == Navigation.Mode.Horizontal) { customNav.wrapAround = wrapAround; }
            else if (navigationMode == Navigation.Mode.Explicit) { StartCoroutine("InitUINavigation", customNav); return; }

            targetButton.navigation = customNav;
        }

        public void CreateRipple(Vector2 pos)
        {
            if (rippleParent != null)
            {
                GameObject rippleObj = new GameObject();
                rippleObj.AddComponent<Image>();
                rippleObj.GetComponent<Image>().sprite = rippleShape;
                rippleObj.name = "Ripple";
                rippleParent.SetActive(true);
                rippleObj.transform.SetParent(rippleParent.transform);

                if (renderOnTop == true) { rippleParent.transform.SetAsLastSibling(); }
                else { rippleParent.transform.SetAsFirstSibling(); }

                if (centered == true) { rippleObj.transform.localPosition = new Vector2(0f, 0f); }
                else { rippleObj.transform.position = pos; }

                rippleObj.AddComponent<Ripple>();
                Ripple tempRipple = rippleObj.GetComponent<Ripple>();
                tempRipple.speed = speed;
                tempRipple.maxSize = maxSize;
                tempRipple.startColor = startColor;
                tempRipple.transitionColor = transitionColor;

                if (rippleUpdateMode == RippleUpdateMode.Normal) { tempRipple.unscaledTime = false; }
                else { tempRipple.unscaledTime = true; }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable == false || eventData.button != PointerEventData.InputButton.Left) { return; }
            if (enableButtonSounds == true && useClickSound == true && soundSource != null) { soundSource.PlayOneShot(clickSound); }

            // Invoke click actions
            onClick.Invoke();

            // Check for double click
            if (checkForDoubleClick == false || gameObject.activeInHierarchy == false) { return; }
            if (waitingForDoubleClickInput == true)
            {
                onDoubleClick.Invoke();
                waitingForDoubleClickInput = false;
                return;
            }

            waitingForDoubleClickInput = true;
            StopCoroutine("CheckForDoubleClick");
            StartCoroutine("CheckForDoubleClick");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (useRipple == true && isPointerOn == true)
#if ENABLE_LEGACY_INPUT_MANAGER
                if (targetCanvas != null && (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera || targetCanvas.renderMode == RenderMode.WorldSpace)) { CreateRipple(targetCanvas.worldCamera.ScreenToWorldPoint(Input.mousePosition)); }
                else { CreateRipple(Input.mousePosition); }
#elif ENABLE_INPUT_SYSTEM
                if (targetCanvas != null && (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera || targetCanvas.renderMode == RenderMode.WorldSpace)) { CreateRipple(targetCanvas.worldCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue())); }
                else { CreateRipple(Mouse.current.position.ReadValue()); }
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable == false)
                return;

            if (enableButtonSounds == true && useHoverSound == true && soundSource != null) { soundSource.PlayOneShot(hoverSound); }
            if (animationSolution == AnimationSolution.ScriptBased) { StartCoroutine("SetHighlight"); }

            isPointerOn = true;
            onHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (animationSolution == AnimationSolution.ScriptBased) { StartCoroutine("SetNormal"); }

            isPointerOn = false;
            onLeave.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (animationSolution == AnimationSolution.ScriptBased) { StartCoroutine("SetHighlight"); }
            if (useUINavigation == true) { onHover.Invoke(); }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (animationSolution == AnimationSolution.ScriptBased) { StartCoroutine("SetNormal"); }
            if (useUINavigation == true) { onLeave.Invoke(); }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (animationSolution == AnimationSolution.ScriptBased) { StartCoroutine("SetNormal"); }

            onClick.Invoke();
        }

        IEnumerator LayoutFix()
        {
            yield return new WaitForSecondsRealtime(0.025f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            if (disabledCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCG.GetComponent<RectTransform>()); }
            if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
            if (highlightCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCG.GetComponent<RectTransform>()); }
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");
            StopCoroutine("SetDisabled");

            while (normalCG.alpha < 0.99f)
            {
                normalCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            disabledCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetDisabled");

            while (highlightCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 1;
            disabledCG.alpha = 0;
        }

        IEnumerator SetDisabled()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (disabledCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 0;
            disabledCG.alpha = 1;
        }

        IEnumerator CheckForDoubleClick()
        {
            yield return new WaitForSecondsRealtime(doubleClickPeriod);
            waitingForDoubleClickInput = false;
        }

        IEnumerator InitUINavigation(Navigation nav)
        {
            yield return new WaitForSecondsRealtime(1);
            if (selectOnUp != null) { nav.selectOnUp = selectOnUp.GetComponent<Selectable>(); }
            if (selectOnDown != null) { nav.selectOnDown = selectOnDown.GetComponent<Selectable>(); }
            if (selectOnLeft != null) { nav.selectOnLeft = selectOnLeft.GetComponent<Selectable>(); }
            if (selectOnRight != null) { nav.selectOnRight = selectOnRight.GetComponent<Selectable>(); }
            targetButton.navigation = nav;
        }
    }
}