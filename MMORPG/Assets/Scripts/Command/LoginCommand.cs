using Common.Proto.Base;
using Common.Proto.User;
using Common.Tool;
using MMORPG.Model;
using QFramework;
using MMORPG.System;
using Serilog;
using UnityEngine.SceneManagement;

namespace MMORPG.Command
{
    public class LoginCommand : AbstractCommand
    {
        private string _username;
        private string _password;

        public LoginCommand(string username, string password)
        {
            _username = username;
            _password = password;
        }

        protected override async void OnExecute()
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
                Log.Information($"'{_username}'登录成功");
                var user = this.GetModel<IUserModel>();
                user.Username.Value = _username;
                SceneManager.LoadScene("CharacterSelectScene");
            }
            else
            {
                Log.Error($"'{_username}'登录失败:{response.Error.GetInfo().Description}");
                box.ShowMessage($"登录失败!\n原因:{response.Error.GetInfo().Description}");
            }
        }
    }
}
