using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class LoginPanelData : UIPanelData
	{
	}
	public partial class LoginPanel : UIPanel, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LoginPanelData ?? new LoginPanelData();
            // please add init code here

            mData = uiData as LoginPanelData ?? new LoginPanelData();

            BtnLogin.onClick.AddListener(() =>
            {
                this.SendCommand(new LoginCommand(
                    InputUsername.text,
                    InputPassword.text));
            });
        }
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
