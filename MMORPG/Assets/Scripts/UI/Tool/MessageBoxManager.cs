using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum MessageBoxStyle
{
    ForLongDesc,
    ForShortDesc
}

public struct MessageBoxConfig
{
    public MessageBoxStyle Style;
    public string Title;
    public string Description;
    public string ConfirmButtonText;
    public string CancalButtonText;
    public bool ShowConfirmButton;
    public bool ShowCancalButton;
    public Action OnConfirm;
    public Action OnOpen;
    public Action OnCancel;
}

public class MessageBoxManager : MonoSingleton<MessageBoxManager>
{
    public ModalWindowManager WindowStyle1;
    public ModalWindowManager WindowStyle2;


    public bool IsShowing => WindowStyle1.isOn || WindowStyle2.isOn;

    public MessageBoxConfig Config { get; private set; }
    public ModalWindowManager Window => Config.Style switch { MessageBoxStyle.ForLongDesc => WindowStyle1,  _ => WindowStyle2, };

    public void Show(MessageBoxConfig cfg)
    {
        if (IsShowing)
            throw new Exception("当前已有MessageBox正在显示!");
        UpdataConfig(cfg);
        
        SceneManager.Instance.Invoke(OpenWindow);
    }

    private void UpdataConfig(MessageBoxConfig cfg)
    {
        Config = cfg;
        Window.titleText = Config.Title;
        Window.descriptionText = Config.Description;
        Window.confirmButton.buttonText = Config.ConfirmButtonText;
        Window.cancelButton.buttonText = Config.CancalButtonText;

        Window.showConfirmButton = Config.ShowConfirmButton;
        Window.showCancelButton = Config.ShowCancalButton;
    }

    private void OpenWindow()
    {
        Window.cancelButton.UpdateUI();
        Window.confirmButton.UpdateUI();
        Window.UpdateUI();
        Window.Open();
    }

    public void OnWindowCancal()
    {
        Config.OnCancel?.Invoke();
    }

    public void OnWindowConfirm()
    {
        Config.OnConfirm?.Invoke();
    }

    public void OnWindowOpen()
    {
        Config.OnOpen?.Invoke();
    }
}

public enum MessageBoxResult
{
    Confirm,
    Cancel
}

public static class MessageBox
{
    public static void ShowInfo(
        string text,
        string title = "提示",
        string buttonText = "确认",
        Action closeCallback = null,
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        //TODO 图标
        Show(new MessageBoxConfig()
        {
            Style = style,
            Title = title,
            Description = text,
            ConfirmButtonText = buttonText,
            ShowCancalButton = false,
            ShowConfirmButton = true,
            OnConfirm = closeCallback,
        });
    }

    public static Task ShowInfoAsync(
        string text,
        string title = "提示",
        string buttonText = "确认",
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        var tcs = new TaskCompletionSource<bool>();
        void OnClose()
        {
            var suc = tcs.TrySetResult(true);
            Debug.Assert(suc);
        }
        ShowInfo(text, title, buttonText, OnClose, style);
        return tcs.Task;
    }


    public static void ShowWarn(
        string text,
        string title = "警告",
        string buttonText = "确认",
        Action closeCallback = null,
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        //TODO 图标
        Show(new MessageBoxConfig()
        {
            Style = style,
            Title = title,
            Description = text,
            ConfirmButtonText = buttonText,
            ShowCancalButton = false,
            ShowConfirmButton = true,
            OnConfirm = closeCallback,
        });
    }

    public static Task ShowWarnAsync(
        string text,
        string title = "警告",
        string buttonText = "确认",
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        var tcs = new TaskCompletionSource<bool>();
        void OnClose()
        {
            var suc = tcs.TrySetResult(true);
            Debug.Assert(suc);
        }
        ShowWarn(text, title, buttonText, OnClose, style);
        return tcs.Task;
    }

    public static void ShowError(
        string text,
        string title = "错误",
        string buttonText = "确认",
        Action closeCallback = null,
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        //TODO 图标
        Show(new MessageBoxConfig()
        {
            Style = style,
            Title = title,
            Description = text,
            ConfirmButtonText = buttonText,
            ShowCancalButton = false,
            ShowConfirmButton = true,
            OnConfirm = closeCallback,
        });
    }

    public static Task ShowErrorAsync(
        string text,
        string title = "错误",
        string buttonText = "确认",
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        var tcs = new TaskCompletionSource<bool>();
        void OnClose()
        {
            var suc = tcs.TrySetResult(true);
            Debug.Assert(suc);
        }
        ShowError(text, title, buttonText, OnClose, style);
        return tcs.Task;
    }

    public static void ShowOption(
        string text,
        Action<MessageBoxResult> resultCallback = null,
        string title = "选择",
        string confirmButtonText = "确认",
        string cancelButtonText = "取消",
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        //TODO 图标
        Show(new MessageBoxConfig()
        {
            Style = style,
            Title = title,
            Description = text,
            ConfirmButtonText = confirmButtonText,
            CancalButtonText = cancelButtonText,
            ShowCancalButton = true,
            ShowConfirmButton = true,
            OnConfirm = () => resultCallback(MessageBoxResult.Confirm),
            OnCancel = () => resultCallback(MessageBoxResult.Cancel)
        });
    }

    public static Task<MessageBoxResult> ShowOptionAsync(
        string text,
        string title = "选择",
        string confirmButtonText = "确认",
        string cancelButtonText = "取消",
        MessageBoxStyle style = MessageBoxStyle.ForLongDesc)
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        void OnResult(MessageBoxResult res)
        {
            var suc = tcs.TrySetResult(res);
            Debug.Assert(suc);
        }
        ShowOption(text, OnResult, title, confirmButtonText, cancelButtonText, style);
        return tcs.Task;
    }

    public static void Show(MessageBoxConfig cfg)
    {
        MessageBoxManager.Instance.Show(cfg);
    }
}