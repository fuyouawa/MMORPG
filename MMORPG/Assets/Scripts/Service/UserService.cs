using Common.Proto.User;
using TMPro;
using UnityEngine;

namespace Serivce {
    public class UserService : MonoSingleton<UserService>
    {
        public TMP_InputField LoginUsernameInput;
        public TMP_InputField LoginPasswordInput;
        public TMP_InputField RegisterUsernameInput;
        public TMP_InputField RegisterPasswordInput;
        public TMP_InputField RegisterVeriftyPasswordInput;

        public async void TryLogin()
        {
            //TODO ’À∫≈√‹¬ÎπÊ∑∂ºÏ≤È
            GameClient.Instance.Session.SendAsync(new UserLoginRequest()
            {
                Username = LoginUsernameInput.text,
                Password = LoginPasswordInput.text,
            }, null);

            var res = await GameClient.Instance.Session.ReceiveAsync<UserLoginResponse>();
            if (res.Status == LoginStatus.Error)
            {
            }
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
}