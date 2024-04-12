using Common.Proto.Base;
using Common.Proto.Player;
using Common.Tool;
using TMPro;
using UnityEngine;

namespace Serivce {
    public class PlayerService : MonoSingleton<PlayerService>
    {
        public TMP_InputField LoginUsernameInput;
        public TMP_InputField LoginPasswordInput;
        public TMP_InputField RegisterUsernameInput;
        public TMP_InputField RegisterPasswordInput;
        public TMP_InputField RegisterVeriftyPasswordInput;

        public async void TryLogin()
        {
            if (LoginUsernameInput.text.Length < 4 || LoginUsernameInput.text.Length > 12)
            {
                SceneManager.Instance.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
                return;
            }
            if (LoginPasswordInput.text.Length < 8 || LoginPasswordInput.text.Length > 16)
            {
                SceneManager.Instance.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
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
                SceneManager.Instance.SwitchScene("EnterScene");
            }
            else
            {
                SceneManager.Instance.ShowMessageBox(new()
                {
                    Description = $"登录失败!\n原因:{response.Error.GetInfo().Description}"
                });
            }
        }

        public async void TryRegister()
        {
            if (RegisterUsernameInput.text.Length < 4 || RegisterUsernameInput.text.Length > 12)
            {
                SceneManager.Instance.CreateNotificationBox(new() { Description = "用户名长度必须在4-12字之间!" });
                return;
            }
            if (RegisterPasswordInput.text.Length < 8 || RegisterPasswordInput.text.Length > 16)
            {
                SceneManager.Instance.CreateNotificationBox(new() { Description = "密码长度必须在8-16字之间!" });
                return;
            }
            if (RegisterPasswordInput.text != RegisterVeriftyPasswordInput.text)
            {
                SceneManager.Instance.CreateNotificationBox(new() { Description = "两次密码输入不相同!" });
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
                SceneManager.Instance.ShowMessageBox(new()
                {
                    Description = $"注册成功!"
                });
            }
            else
            {
                SceneManager.Instance.ShowMessageBox(new()
                {
                    Description = $"注册失败!\n原因:{response.Error.GetInfo().Description}"
                });
            }
        }

        public async void TryEnterGame()
        {
            SceneManager.Instance.SwitchScene("GameScene");
        }
    }
}