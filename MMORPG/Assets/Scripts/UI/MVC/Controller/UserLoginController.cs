using Common.Network;
using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using Michsky.MUIP;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;


public class UserLoginArch : Architecture<UserLoginArch>
{
    protected override void Init()
    {
        this.RegisterModel(new UserLoginModel());
    }
}

public class UserLoginController : MonoBehaviour, IController
{
    private GameObject _loginPanel;
    private GameObject _registerPanel;
    private GameObject _operationPanel;

    private CustomInputField _loginUsernameInput;
    private CustomInputField _loginPasswordInput;
    private CustomInputField _registerUsernameInput;
    private CustomInputField _registerPasswordnput;
    private CustomInputField _registerVerifyPasswordnput;

    private UserLoginModel _model;

    private async void Start()
    {
        _model = this.GetModel<UserLoginModel>();

        _loginPanel = transform.Find("Login Panel").gameObject;
        _registerPanel = transform.Find("Register Panel").gameObject;
        _operationPanel = transform.Find("Operation Panel").gameObject;

        _loginUsernameInput = _loginPanel.transform.FindIncludeAllChildren("Username Input").GetComponent<CustomInputField>();
        _loginPasswordInput = _loginPanel.transform.FindIncludeAllChildren("Password Input").GetComponent<CustomInputField>();

        _registerUsernameInput = _registerPanel.transform.FindIncludeAllChildren("Username Input").GetComponent<CustomInputField>();
        _registerPasswordnput = _registerPanel.transform.FindIncludeAllChildren("Password Input").GetComponent<CustomInputField>();
        _registerVerifyPasswordnput = _registerPanel.transform.FindIncludeAllChildren("Verify Password Input").GetComponent<CustomInputField>();

        await ConnectServer();
    }

    public void UpdateModel()
    {
        _model.LoginUsername = _loginUsernameInput.inputText.text;
        _model.LoginPassword = _loginPasswordInput.inputText.text;
        _model.RegisterUsername = _registerUsernameInput.inputText.text;
        _model.RegisterPassword = _registerPasswordnput.inputText.text;
        _model.RegisterVerifyPassword = _registerVerifyPasswordnput.inputText.text;
    }

    public IArchitecture GetArchitecture()
    {
        return UserLoginArch.Interface;
    }

    private void OnDestroy()
    {
        _model = null;
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

    public void DoLogin()
    {
        UpdateModel();
        this.SendCommand<UserLoginCommand>();
    }

    public void DoRegister()
    {
        UpdateModel();
        this.SendCommand<UserRegisterCommand>();
    }


    public void ShowLoginPanel()
    {
        PanelSwitcher.FadeOut(_operationPanel);
        PanelSwitcher.FadeIn(_loginPanel);
    }

    public void ShowRegisterPanel()
    {
        PanelSwitcher.FadeOut(_operationPanel);
        PanelSwitcher.FadeIn(_registerPanel);
    }
    public void CloseLoginPanel()
    {
        PanelSwitcher.FadeIn(_operationPanel);
        PanelSwitcher.FadeOut(_loginPanel);
    }

    public void CloseRegisterPanel()
    {
        PanelSwitcher.FadeIn(_operationPanel);
        PanelSwitcher.FadeOut(_registerPanel);
    }
}
