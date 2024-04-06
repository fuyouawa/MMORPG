using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.MUIP
{
    [RequireComponent(typeof(Animator))]
    public class ContextMenuManager : MonoBehaviour
    {
        // Resources
        public Canvas mainCanvas;
        public Camera targetCamera;
        public GameObject contextContent;
        public Animator contextAnimator;
        public GameObject contextButton;
        public GameObject contextSeparator;
        public GameObject contextSubMenu;

        // Settings
        [SerializeField] private bool debugMode;
        public bool autoSubMenuPosition = true;
        public SubMenuBehaviour subMenuBehaviour;
        public CameraSource cameraSource = CameraSource.Main;

        // Bounds
        public CursorBoundHorizontal horizontalBound;
        public CursorBoundVertical verticalBound;
        [Range(-50, 50)] public int vBorderTop = -10;
        [Range(-50, 50)] public int vBorderBottom = 10;
        [Range(-50, 50)] public int hBorderLeft = 15;
        [Range(-50, 50)] public int hBorderRight = -15;

        Vector2 uiPos;
        Vector3 cursorPos;
        Vector3 contentPos = new Vector3(0, 0, 0);
        Vector3 contextVelocity = Vector3.zero;

        RectTransform contextRect;
        RectTransform contentRect;

        [HideInInspector] public bool isOn;

        public enum CameraSource { Main, Custom }
        public enum SubMenuBehaviour { Hover, Click }
        public enum CursorBoundHorizontal { Left, Right }
        public enum CursorBoundVertical { Bottom, Top }

        void Awake()
        {
            if (mainCanvas == null) { mainCanvas = gameObject.GetComponentInParent<Canvas>(); }
            if (contextAnimator == null) { contextAnimator = gameObject.GetComponent<Animator>(); }
            if (cameraSource == CameraSource.Main) { targetCamera = Camera.main; }

            contextRect = gameObject.GetComponent<RectTransform>();
            contentRect = contextContent.GetComponent<RectTransform>();
            contentPos = new Vector3(vBorderTop, hBorderLeft, 0);
            gameObject.transform.SetAsLastSibling();
#if UNITY_2022_1_OR_NEWER
            subMenuBehaviour = SubMenuBehaviour.Click;
#endif
        }

        public void CheckForBound()
        {
            if (uiPos.x <= -100) 
            {
                horizontalBound = CursorBoundHorizontal.Left;
                contentPos = new Vector3(hBorderLeft, contentPos.y, 0); contentRect.pivot = new Vector2(0f, contentRect.pivot.y); 
            }

            else if (uiPos.x >= 100)
            {
                horizontalBound = CursorBoundHorizontal.Right;
                contentPos = new Vector3(hBorderRight, contentPos.y, 0); contentRect.pivot = new Vector2(1f, contentRect.pivot.y);
            }

            if (uiPos.y <= -75)
            {
                verticalBound = CursorBoundVertical.Bottom;
                contentPos = new Vector3(contentPos.x, vBorderBottom, 0); contentRect.pivot = new Vector2(contentRect.pivot.x, 0f);
            }

            else if (uiPos.y >= 75)
            {
                verticalBound = CursorBoundVertical.Top;
                contentPos = new Vector3(contentPos.x, vBorderTop, 0); contentRect.pivot = new Vector2(contentRect.pivot.x, 1f);
            }
        }

        public void SetContextMenuPosition()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();
#endif

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                contextRect.position = targetCamera.ScreenToWorldPoint(cursorPos);
                contextRect.localPosition = new Vector3(contextRect.localPosition.x, contextRect.localPosition.y, 0);
                contextContent.transform.localPosition = Vector3.SmoothDamp(contextContent.transform.localPosition, contentPos, ref contextVelocity, 0);
            }

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                contextRect.position = cursorPos;
                contextContent.transform.position = new Vector3(cursorPos.x + contentPos.x, cursorPos.y + contentPos.y, 0);
            }

            uiPos = contextRect.anchoredPosition;
            CheckForBound();

            if (debugMode == true)
            {
                PrintDebug();
            }
        }

        public void SetFixedPosition()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();
#endif
            SetContextMenuPosition();

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                contextRect.position = targetCamera.ScreenToWorldPoint(cursorPos);
                contextRect.localPosition = new Vector3(contextRect.localPosition.x, contextRect.localPosition.y, 0);
                contextContent.transform.localPosition = Vector3.SmoothDamp(contextContent.transform.localPosition, contentPos, ref contextVelocity, 0);
            }

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                contextRect.position = cursorPos;
                contextContent.transform.position = new Vector3(cursorPos.x + contentPos.x, cursorPos.y + contentPos.y, 0);
            }

            uiPos = contextRect.anchoredPosition;
            CheckForBound();

            if (debugMode == true)
            {
                PrintDebug();
            }
        }

        void ProcessContextRect()
        {
            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                contextRect.position = targetCamera.ScreenToWorldPoint(cursorPos);
                contextRect.localPosition = new Vector3(contextRect.localPosition.x, contextRect.localPosition.y, 0);
                contextContent.transform.localPosition = Vector3.SmoothDamp(contextContent.transform.localPosition, contentPos, ref contextVelocity, 0);
            }

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                contextRect.position = cursorPos;
                contextContent.transform.position = new Vector3(cursorPos.x + contentPos.x, cursorPos.y + contentPos.y, 0);
            }
        }

        void PrintDebug()
        {
            Debug.Log("<b>[Context Menu]</b> UI Pos: " + uiPos + ", H: " + horizontalBound + ", V: " + verticalBound, this);
        }

        public void Open() 
        { 
            contextAnimator.Play("Menu In"); 
            isOn = true; 
        }

        public void Close()
        { 
            contextAnimator.Play("Menu Out"); 
            isOn = false; 
        }

        public void OpenInFixedPosition()
        {
            SetFixedPosition();
            Open();
        }

        #region Obsolote
        public void OpenContextMenu() { Open(); }
        public void CloseOnClick() { Close(); }   
        #endregion
    }
}