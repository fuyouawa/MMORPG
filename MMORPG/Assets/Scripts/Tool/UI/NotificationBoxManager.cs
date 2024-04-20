using System;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 通知框显示位置
/// </summary>
public enum NotificationBoxPosition
{
    TopLeft,        //TODO
    TopRight,
    BottomLeft,     //TODO
    BottomRight     //TODO
}

/// <summary>
/// 通知框出现样式
/// </summary>
public enum NotificationBoxStyle
{
    Fading,     // 渐变
    Popup,      // 弹出
    Sliding     // 滑动
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
    public Michsky.MUIP.NotificationManager NotifyFadingTL;
    public Michsky.MUIP.NotificationManager NotifyPopupTL;
    public Michsky.MUIP.NotificationManager NotifySlidingTL;
    public Michsky.MUIP.NotificationManager NotifyFadingTR;
    public Michsky.MUIP.NotificationManager NotifyPopupTR;
    public Michsky.MUIP.NotificationManager NotifySlidingTR;
    public Michsky.MUIP.NotificationManager NotifyFadingBL;
    public Michsky.MUIP.NotificationManager NotifyPopupBL;
    public Michsky.MUIP.NotificationManager NotifySlidingBL;
    public Michsky.MUIP.NotificationManager NotifyFadingBR;
    public Michsky.MUIP.NotificationManager NotifyPopupBR;
    public Michsky.MUIP.NotificationManager NotifySlidingBR;

    public NotificationBoxConfig Config { get; set; }

    private RectTransform _instantiationsGroup;

    private void Awake()
    {
        _instantiationsGroup = new GameObject("Instantiations Group").AddComponent<RectTransform>();
        _instantiationsGroup.SetParent(transform, false);
    }

    public void Create()
    {
        var notification = Instantiate(GetNotification());
        notification.gameObject.SetActive(true);
        notification.gameObject.transform.SetParent(_instantiationsGroup, false);
        notification.title = Config.Title;
        notification.description = Config.Description;
        notification.closeBehaviour = Michsky.MUIP.NotificationManager.CloseBehaviour.Destroy;
        notification.UpdateUI();
        notification.Open();
    }

    private Michsky.MUIP.NotificationManager GetNotification()
    {
        switch (Config.Position)
        {
            case NotificationBoxPosition.TopLeft:
                return Config.Style switch
                {
                    NotificationBoxStyle.Fading => NotifyFadingTL,
                    NotificationBoxStyle.Popup => NotifyPopupTL,
                    NotificationBoxStyle.Sliding => NotifySlidingTL,
                    _ => throw new NotImplementedException(),
                };
            case NotificationBoxPosition.TopRight:
                return Config.Style switch
                {
                    NotificationBoxStyle.Fading => NotifyFadingTR,
                    NotificationBoxStyle.Popup => NotifyPopupTR,
                    NotificationBoxStyle.Sliding => NotifySlidingTR,
                    _ => throw new NotImplementedException(),
                };
            case NotificationBoxPosition.BottomLeft:
                return Config.Style switch
                {
                    NotificationBoxStyle.Fading => NotifyFadingBL,
                    NotificationBoxStyle.Popup => NotifyPopupBL,
                    NotificationBoxStyle.Sliding => NotifySlidingBL,
                    _ => throw new NotImplementedException(),
                };
            case NotificationBoxPosition.BottomRight:
                return Config.Style switch
                {
                    NotificationBoxStyle.Fading => NotifyFadingBR,
                    NotificationBoxStyle.Popup => NotifyPopupBR,
                    NotificationBoxStyle.Sliding => NotifySlidingBR,
                    _ => throw new NotImplementedException(),
                };
            default:
                throw new NotImplementedException();
        }
    }
}