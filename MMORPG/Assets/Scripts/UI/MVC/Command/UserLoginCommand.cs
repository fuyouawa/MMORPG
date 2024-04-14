using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserLoginCommand : AbstractCommand
{
    protected async override void OnExecute()
    {
        var model = this.GetModel<UserLoginModel>();

        if (model.LoginUsername.Length < 4 || model.LoginUsername.Length > 12)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
            return;
        }
        if (model.LoginPassword.Length < 8 || model.LoginPassword.Length > 16)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
            return;
        }

        var request = new LoginRequest
        {
            Username = model.LoginUsername,
            Password = model.LoginPassword
        };
        SceneHelper.BeginSpinnerBox(new() { Description = "登录中......" });
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<LoginResponse>();
        SceneHelper.EndSpinnerBox();

        if (response.Error == NetError.Success)
        {
            SceneHelper.SwitchScene("EnterScene");
        }
        else
        {
            SceneHelper.ShowMessageBox(new()
            {
                Description = $"登录失败!\n原因:{response.Error.GetInfo().Description}"
            });
        }
    }
}