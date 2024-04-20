using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum MessageBoxStyle
{
    LongDesc,   // 用于显示长消息
    ShortDesc   // 用于显示短消息, 适合只有两行文本的
}

public enum MessageBoxResult
{
    Confirm,    // 确认
    Cancel      // 取消
}

/// <summary>
/// 消息框的配置
/// </summary>
public class MessageBoxConfig
{
    public string Title = "提示";
    public string Description = "我是一个消息框";
    public string ConfirmButtonText = "确认";
    public string CancalButtonText = "取消";
    public bool ShowConfirmButton = true;
    public bool ShowCancalButton = false;
    public Action<MessageBoxResult> OnChose = null;    // 当用户选择了"确认"或"取消"的其中一个按钮
    public Action OnOpen = null;
    public TaskCompletionSource<MessageBoxResult> OnChoseTcs = null;
    public MessageBoxStyle Style = MessageBoxStyle.LongDesc;
}

public class MessageBoxManager : MonoBehaviour
{
    public ModalWindowManager LongDescModalWindow;
    public ModalWindowManager ShortDescModalWindow;

    public bool IsShowing => GetWindow().isOn;

    public MessageBoxConfig Config { get; set; }

    private void Awake()
    {
        void OnConfirm()
        {
            Config.OnChose?.Invoke(MessageBoxResult.Confirm);
            Config.OnChoseTcs?.TrySetResult(MessageBoxResult.Confirm);
        }

        void OnCancel()
        {
            Config.OnChose?.Invoke(MessageBoxResult.Cancel);
            Config.OnChoseTcs?.TrySetResult(MessageBoxResult.Cancel);
        }

        void OnOpen()
        {
            Config.OnOpen?.Invoke();
        }

        LongDescModalWindow.confirmButton.onClick.AddListener(OnConfirm);
        LongDescModalWindow.cancelButton.onClick.AddListener(OnCancel);
        LongDescModalWindow.onOpen.AddListener(OnOpen);

        ShortDescModalWindow.confirmButton.onClick.AddListener(OnConfirm);
        ShortDescModalWindow.cancelButton.onClick.AddListener(OnCancel);
        ShortDescModalWindow.onOpen.AddListener(OnOpen);
    }

    public void Show()
    {
        if (IsShowing)
            throw new Exception("当前已有MessageBox正在显示!");

        var window = GetWindow();
        window.titleText = Config.Title;
        window.descriptionText = Config.Description;
        window.confirmButton.buttonText = Config.ConfirmButtonText;
        window.cancelButton.buttonText = Config.CancalButtonText;
        window.showConfirmButton = Config.ShowConfirmButton;
        window.showCancelButton = Config.ShowCancalButton;

        window.cancelButton.UpdateUI();
        window.confirmButton.UpdateUI();
        window.UpdateUI();
        window.Open();
    }

    private ModalWindowManager GetWindow() => Config.Style switch
    {
        MessageBoxStyle.LongDesc => LongDescModalWindow,
        MessageBoxStyle.ShortDesc => ShortDescModalWindow,
        _ => throw new NotImplementedException()
    };
}