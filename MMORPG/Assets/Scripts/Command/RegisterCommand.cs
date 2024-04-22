using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RegisterCommand : AbstractCommand
{
    private string _username;
    private string _password;
    private string _password2;

    public RegisterCommand(string username, string password, string password2)
    {
        _username = username;
        _password = password;
        _password2 = password2;
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
        if (_password != _password2)
        {
            box.ShowNotification("两次密码输入不相同!");
            return;
        }

        box.ShowSpinner("注册中......");
        var net = this.GetSystem<INetworkSystem>();
        net.SendToServer(new RegisterRequest
        {
            Username = _username,
            Password = _password
        });
        var response = await net.ReceiveAsync<RegisterResponse>();
        box.CloseSpinner();

        if (response.Error == NetError.Success)
        {
            Logger.Info("Network", $"'{_username}'注册成功!");
            box.ShowMessage("注册成功!");
        }
        else
        {
            Logger.Error("Network", $"'{_username}'注册失败:{response.Error.GetInfo().Description}");
            box.ShowMessage($"注册失败!\n原因:{response.Error.GetInfo().Description}");
        }
    }
}