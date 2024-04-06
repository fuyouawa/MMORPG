using Common.Proto.User;
using TMPro;
using UnityEngine;

namespace Serivce {
    public class UserService : MonoSingleton<UserService>
    {
        public GameObject AccountPanel;
        public GameObject LoginPanel;
        public GameObject RegisterPanel;

        public TMP_InputField LoginUsernameInput;
        public TMP_InputField LoginPasswordInput;
        public TMP_InputField RegisterUsernameInput;
        public TMP_InputField RegisterPasswordInput;
        public TMP_InputField RegisterVeriftyPasswordInput;

        private void Start()
        {
            AccountPanel.SetActive(true);
            LoginPanel.SetActive(false);
            RegisterPanel.SetActive(false);
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

        public async void TryLogin()
        {
            //TODO ’À∫≈√‹¬ÎπÊ∑∂ºÏ≤È
            GameClient.Instance.Session.SendAsync(new UserLoginRequest()
            {
                Username = LoginUsernameInput.text,
                Password = LoginPasswordInput.text,
            });

            var res = await GameClient.Instance.Session.ReceiveAsync<UserLoginResponse>();
            if (res.Status == LoginStatus.Error)
            {
                Popup.Instance.Show(res.Message);
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