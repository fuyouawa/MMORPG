using Common.Network;
using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartService : MonoBehaviour
{
    public TMP_InputField LoginUsernameInput;
    public TMP_InputField LoginPasswordInput;
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;
    public TMP_InputField RegisterVeriftyPasswordInput;

    private async void Start()
    {
        await ConnectServer();
    }

    public async Task ConnectServer()
    {
        Socket socket;
        while (true)
        {
            // 显示旋转加载框
            SceneHelper.BeginSpinnerBox(new SpinnerBoxConfig() { Description = "连接服务器中......" });
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
                SceneHelper.EndSpinnerBox();
                break;
            }
            catch (System.Exception ex)
            {
                SceneHelper.EndSpinnerBox();
                // 显示消息框
                await SceneHelper.ShowMessageBoxAsync(new MessageBoxConfig()
                {
                    Title = "错误",
                    Description = $"连接服务器失败:{ex}",
                    ConfirmButtonText = "重新连接",
                });
                continue;
            }
        }
        // 开始事件循环
        await NetClient.StartSessionAsync(socket);
    }

    public async void DoLogin()
    {
        if (LoginUsernameInput.text.Length < 4 || LoginUsernameInput.text.Length > 12)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
            return;
        }
        if (LoginPasswordInput.text.Length < 8 || LoginPasswordInput.text.Length > 16)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
            return;
        }

        var request = new LoginRequest
        {
            Username = LoginUsernameInput.text,
            Password = LoginPasswordInput.text
        };
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<LoginResponse>();
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

    public async void DoRegister()
    {
        if (RegisterUsernameInput.text.Length < 4 || RegisterUsernameInput.text.Length > 12)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
            return;
        }
        if (RegisterPasswordInput.text.Length < 8 || RegisterPasswordInput.text.Length > 16)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
            return;
        }
        if (RegisterPasswordInput.text != RegisterVeriftyPasswordInput.text)
        {
            SceneHelper.CreateNotificationBox(new() { Description = "两次密码输入不相同!" });
            return;
        }

        var request = new RegisterRequest
        {
            Username = RegisterUsernameInput.text,
            Password = RegisterPasswordInput.text
        };
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<RegisterResponse>();
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