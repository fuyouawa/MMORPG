using System;
using QFramework;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 通知框显示位置
    /// </summary>
    public enum NotificationBoxPosition
    {
        TopLeft, //TODO
        TopRight,
        BottomLeft, //TODO
        BottomRight //TODO
    }

    /// <summary>
    /// 通知框出现样式
    /// </summary>
    public enum NotificationBoxStyle
    {
        Fading, // 渐变
        Popup, // 弹出
        Sliding // 滑动
    }

    public record NotificationBoxConfig
    {
        public string Title = "通知";
        public string Description = "我是一个通知框";
        public NotificationBoxPosition Position = NotificationBoxPosition.TopRight;
        public NotificationBoxStyle Style = NotificationBoxStyle.Sliding;
    }

    public class NotificationBoxManager : MonoBehaviour
    {
        private static NotificationBoxManager _instance;
        public static NotificationBoxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindAnyObjectByType<NotificationBoxManager>();
                }
                return _instance;
            }
        }

        public Michsky.MUIP.NotificationManager FadingTL;
        public Michsky.MUIP.NotificationManager PopupTL;
        public Michsky.MUIP.NotificationManager SlidingTL;
        public Michsky.MUIP.NotificationManager FadingTR;
        public Michsky.MUIP.NotificationManager PopupTR;
        public Michsky.MUIP.NotificationManager SlidingTR;
        public Michsky.MUIP.NotificationManager FadingBL;
        public Michsky.MUIP.NotificationManager PopupBL;
        public Michsky.MUIP.NotificationManager SlidingBL;
        public Michsky.MUIP.NotificationManager FadingBR;
        public Michsky.MUIP.NotificationManager PopupBR;
        public Michsky.MUIP.NotificationManager SlidingBR;

        private RectTransform _instantiationsGroup;

        private void Awake()
        {
            _instantiationsGroup = new GameObject("Instantiations Group").AddComponent<RectTransform>();
            _instantiationsGroup.SetParent(transform, false);
        }

        public void Create(NotificationBoxConfig config)
        {
            var notification = Instantiate(GetNotification(config));
            notification.gameObject.SetActive(true);
            notification.gameObject.transform.SetParent(_instantiationsGroup, false);
            notification.title = config.Title;
            notification.description = config.Description;
            notification.closeBehaviour = Michsky.MUIP.NotificationManager.CloseBehaviour.Destroy;
            notification.UpdateUI();
            notification.Open();
        }

        private Michsky.MUIP.NotificationManager GetNotification(NotificationBoxConfig config)
        {
            switch (config.Position)
            {
                case NotificationBoxPosition.TopLeft:
                    return config.Style switch
                    {
                        NotificationBoxStyle.Fading => FadingTL,
                        NotificationBoxStyle.Popup => PopupTL,
                        NotificationBoxStyle.Sliding => SlidingTL,
                        _ => throw new NotImplementedException(),
                    };
                case NotificationBoxPosition.TopRight:
                    return config.Style switch
                    {
                        NotificationBoxStyle.Fading => FadingTR,
                        NotificationBoxStyle.Popup => PopupTR,
                        NotificationBoxStyle.Sliding => SlidingTR,
                        _ => throw new NotImplementedException(),
                    };
                case NotificationBoxPosition.BottomLeft:
                    return config.Style switch
                    {
                        NotificationBoxStyle.Fading => FadingBL,
                        NotificationBoxStyle.Popup => PopupBL,
                        NotificationBoxStyle.Sliding => SlidingBL,
                        _ => throw new NotImplementedException(),
                    };
                case NotificationBoxPosition.BottomRight:
                    return config.Style switch
                    {
                        NotificationBoxStyle.Fading => FadingBR,
                        NotificationBoxStyle.Popup => PopupBR,
                        NotificationBoxStyle.Sliding => SlidingBR,
                        _ => throw new NotImplementedException(),
                    };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
