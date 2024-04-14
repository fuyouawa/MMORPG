using Common.Network;
using Common.Proto;
using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartSceneUIManager : MonoSingleton<StartSceneUIManager>
{
    public GameObject AccountPanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    public TMP_InputField LoginUsernameInput;
    public TMP_InputField LoginPasswordInput;
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;
    public TMP_InputField RegisterVeriftyPasswordInput;

    async void Start()
    {
        AccountPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        await ConnectServer();
    }

    public void ShowLogin()
    {
        PanelSwitcher.FadeOut(AccountPanel);
        PanelSwitcher.FadeIn(LoginPanel);
    }

    public void CloseLogin()
    {
        PanelSwitcher.FadeIn(AccountPanel);
        PanelSwitcher.FadeOut(LoginPanel);
    }

    public void ShowRegister()
    {
        PanelSwitcher.FadeOut(AccountPanel);
        PanelSwitcher.FadeIn(RegisterPanel);
    }

    public void CloseRegister()
    {
        PanelSwitcher.FadeIn(AccountPanel);
        PanelSwitcher.FadeOut(RegisterPanel);
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
