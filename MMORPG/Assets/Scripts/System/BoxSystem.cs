using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;

public class ShowMessageBoxEvent
{
    public MessageBoxConfig Config;

    public ShowMessageBoxEvent(MessageBoxConfig config)
    {
        Config = config;
    }
}

public class ShowSpinnerBoxEvent
{
    public SpinnerBoxConfig Config;

    public ShowSpinnerBoxEvent(SpinnerBoxConfig config)
    {
        Config = config;
    }
}


public class ShowNotificationBoxEvent
{
    public NotificationBoxConfig Config;

    public ShowNotificationBoxEvent(NotificationBoxConfig config)
    {
        Config = config;
    }
}

public class CloseSpinnerBoxEvent
{
}

public interface IBoxSystem : ISystem
{
    public void ShowMessage(MessageBoxConfig config);
    public void ShowMessage(string description, string title = "消息");

    public Task<MessageBoxResult> ShowMessageAsync(MessageBoxConfig config);
    public Task<MessageBoxResult> ShowMessageAsync(string description, string title = "消息", string confirmButtonText = "确认");

    public void ShowNotification(NotificationBoxConfig config);
    public void ShowNotification(string description, string title="通知");

    public void ShowSpinner(SpinnerBoxConfig config);
    public void ShowSpinner(string description);
    public void CloseSpinner();
}

public class BoxSystem : AbstractSystem, IBoxSystem
{
    public void ShowMessage(MessageBoxConfig config)
    {
        this.SendEvent(new ShowMessageBoxEvent(config));
    }

    public void ShowMessage(string description, string title = "消息")
    {
        ShowMessage(new() { Description = description, Title = title });
    }

    public Task<MessageBoxResult> ShowMessageAsync(MessageBoxConfig config)
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        config.OnChoseTcs = tcs;
        ShowMessage(config);
        return tcs.Task;
    }

    public Task<MessageBoxResult> ShowMessageAsync(string description, string title = "消息", string confirmButtonText = "确认")
    {
        return ShowMessageAsync(new MessageBoxConfig() {
            Description = description,
            Title = title,
            ConfirmButtonText = confirmButtonText });
    }

    public void ShowNotification(NotificationBoxConfig config)
    {
        this.SendEvent(new ShowNotificationBoxEvent(config));
    }

    public void ShowNotification(string description, string title = "通知")
    {
        ShowNotification(new NotificationBoxConfig() {
            Description = description,
            Title = title });
    }

    public void ShowSpinner(SpinnerBoxConfig config)
    {
        this.SendEvent(new ShowSpinnerBoxEvent(config));
    }

    public void ShowSpinner(string description)
    {
        ShowSpinner(new SpinnerBoxConfig() { Description = description });
    }

    public void CloseSpinner()
    {
        this.SendEvent(new CloseSpinnerBoxEvent());
    }


    protected override void OnInit()
    {
    }
}
