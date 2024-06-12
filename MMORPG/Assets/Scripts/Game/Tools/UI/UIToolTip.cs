using System;
using MMORPG.Tool;
using TMPro;
using UnityEngine;

namespace MMORPG.Game
{
    public enum UIToolTipAlignment
    {
        BottomLeft
    }

    public class UIToolTip : MonoBehaviour
    {

        public static UIToolTip Instance {  get; private set; }

        public TextMeshProUGUI TextToolTip;
        public UIToolTipAlignment Alignment = UIToolTipAlignment.BottomLeft;

        public Vector2 Offset;

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        private float _targetAlpha = 0;
        private float _smoothing = 6f;

        private void Start()
        {
            Instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = transform as RectTransform;
        }
        private void Update()
        {
            if (!Mathf.Approximately(_canvasGroup.alpha, _targetAlpha))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, _smoothing * Time.deltaTime);
            }
        }
        public void InternalShow(Transform follow)
        {
            _targetAlpha = 1;

            var alignmentOffset = Alignment switch
            {
                UIToolTipAlignment.BottomLeft => new Vector3(
                    _rectTransform.sizeDelta.x / 2,
                    -_rectTransform.sizeDelta.y / 2, 0),
                _ => throw new ArgumentOutOfRangeException()
            };

            transform.position = follow.position + alignmentOffset + new Vector3(Offset.x, Offset.y, 0);
        }
        public void InternalHide()
        {
            _targetAlpha = 0;
        }

        public static void Show(Transform follow)
        {
            if (Instance == null) return;

            Instance.InternalShow(follow);
        }

        public static void Hide()
        {
            if (Instance == null) return;

            Instance.InternalHide();
        }

        public static void Cleanup()
        {
            if (Instance == null) return;

            Instance.TextToolTip.text = string.Empty;
        }

        public static void AddTitle(string content, int fontSize = 40)
        {
            AppendContent(content, Color.white, fontSize);
        }

        public static void AddLine(string content, int fontSize = 30)
        {
            AppendContent("\n");
            AppendContent(content, Color.white, fontSize);
        }

        public static void AddColumn(string content, int fontSize = 30)
        {
            AppendContent(content, Color.white, fontSize);
        }

        public static void AddTitle(string content, Color color, int fontSize = 40)
        {
            AppendContent(content, color, fontSize);
        }

        public static void AddLine(string content, Color color, int fontSize = 30)
        {
            AppendContent("\n");
            AppendContent(content, color, fontSize);
        }

        public static void AddColumn(string content, Color color, int fontSize = 30)
        {
            AppendContent(content, color, fontSize);
        }

        private static void AppendContent(string content, Color color, int fontSize)
        {
            if (Instance == null) return;
            Instance.TextToolTip.text += $"<color={color.ToHex()}><size={fontSize}>{content}</size></color>";
        }

        private static void AppendContent(string content)
        {
            if (Instance == null) return;
            Instance.TextToolTip.text += $"{content}";
        }
    }
}
