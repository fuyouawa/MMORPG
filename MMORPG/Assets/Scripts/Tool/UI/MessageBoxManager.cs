using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum MessageBoxStyle
{
    LongDesc,
    ShortDesc
}

public enum MessageBoxResult
{
    Confirm,
    Cancel
}

public class MessageBoxConfig
{
    public string Title = "提示";
    public string Description = "我是一个消息框";
    public string ConfirmButtonText = "确认";
    public string CancalButtonText = "取消";
    public bool ShowConfirmButton = true;
    public bool ShowCancalButton = false;
    public Action<MessageBoxResult> OnChose;
    public Action OnOpen;
    public MessageBoxStyle Style = MessageBoxStyle.LongDesc;
}

public class MessageBoxManager : MonoBehaviour
{
    public ModalWindowManager LongDescModalWindow;
    public ModalWindowManager ShortDescModalWindow;

    public bool IsShowing => GetWindow().isOn;

    public MessageBoxConfig Config { get; set; }

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

    public void OnWindowCancal()
    {
        Config.OnChose?.Invoke(MessageBoxResult.Cancel);
    }

    public void OnWindowConfirm()
    {
        Config.OnChose?.Invoke(MessageBoxResult.Confirm);
    }

    public void OnWindowOpen()
    {
        Config.OnOpen?.Invoke();
    }
}