using Common.Proto.Base;
using Common.Proto.Player;
using Common.Proto.User;
using Common.Tool;
using MMORPG;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginCommand : AbstractCommand
{
    private string _username;
    private string _password;

    public LoginCommand(string username, string password)
    {
        _username = username;
        _password = password;
    }

    protected async override void OnExecute()
    {
        var box = this.GetSystem<IBoxSystem>();
        if (_username.Length < 4 || _username.Length > 12)
        {
            box.ShowNotification("用户名长度必须在4-12字之间!");
            return;
        }
        if (_password.Length < 8 || _password.Length > 16)
        {
            box.ShowNotification("密码长度必须在8-16字之间!");
            return;
        }

        box.ShowSpinner("登录中......");
        var net = this.GetSystem<INetworkSystem>();
        net.SendToServer(new LoginRequest
        {
            Username = _username,
            Password = _password
        });
        var response = await net.ReceiveAsync<LoginResponse>();
        box.CloseSpinner();

        if (response.Error == NetError.Success)
        {
            Logger.Info("Network", $"'{_username}'登录成功");
            SceneManager.LoadScene("CharacterSelectScene");
        }
        else
        {
            Logger.Error("Network", $"'{_username}'登录失败:{response.Error.GetInfo().Description}");
            box.ShowMessage($"登录失败!\n原因:{response.Error.GetInfo().Description}");
        }
    }
}
