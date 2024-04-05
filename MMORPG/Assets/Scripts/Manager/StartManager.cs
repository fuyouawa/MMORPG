using Common.Proto;
using Common.Proto.User;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartManager : MonoSingleton<StartManager>
{
    public GameObject AccountPanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    public TMP_InputField LoginUsernameInput;
    public TMP_InputField LoginPasswordInput;
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;
    public TMP_InputField RegisterVeriftyPasswordInput;


    void Start()
    {
        AccountPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        ConnectServer();
    }
    public void ConnectServer()
    {
        Task.Run(GameClient.Instance.ConnectAsync);
    }

    public void ShowLogin()
    {
        AccountPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

    public void CloseLogin()
    {
        AccountPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        AccountPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void CloseRegister()
    {
        AccountPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void TryLogin()
    {
        //TODO ’À∫≈√‹¬ÎπÊ∑∂ºÏ≤È
        GameClient.Instance.Session.SendAsync(new UserLoginRequest() { 
            Username = LoginUsernameInput.text,
            Password = LoginPasswordInput.text,
        });
    }


    public void TryRegister()
    {
        //TODO ’À∫≈√‹¬ÎπÊ∑∂ºÏ≤È
        //GameClient.Instance.Session.SendAsync(new UserRegisterRequest()
        //{
        //    Username = RegisterUsernameInput.text,
        //    Password = RegisterPasswordInput.text,
        //});
    }
}
