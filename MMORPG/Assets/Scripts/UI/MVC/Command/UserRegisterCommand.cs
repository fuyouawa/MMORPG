using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserRegisterCommand : AbstractCommand
{
    protected async override void OnExecute()
    {
        var model = this.GetModel<UserLoginModel>();

        if (model.RegisterUsername.Length < 4 || model.RegisterUsername.Length > 12)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
            return;
        }
        if (model.RegisterPassword.Length < 8 || model.RegisterPassword.Length > 16)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
            return;
        }
        if (model.RegisterPassword != model.RegisterVerifyPassword)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "两次密码输入不相同!" });
            return;
        }

        var request = new RegisterRequest
        {
            Username = model.RegisterUsername,
            Password = model.RegisterPassword
        };
        SceneHelper.BeginSpinnerBox(new() { Description = "注册中......" });
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<RegisterResponse>();
        SceneHelper.EndSpinnerBox();

        if (response.Error == NetError.Success)
        {
            SceneHelper.ShowMessageBox(new()
            {
                Description = $"注册成功!"
            });
        }
        else
        {
            SceneHelper.ShowMessageBox(new()
            {
                Description = $"注册失败!\n原因:{response.Error.GetInfo().Description}"
            });
        }
    }
}