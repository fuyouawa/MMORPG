using MMORPG.Command;
using MMORPG.Game;
using QFramework;

namespace MMORPG.UI
{
	public class UILoginPanelData : UIPanelData
	{
	}
	public partial class UILoginPanel : UIPanel, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        private void Awake()
        {
            BtnLogin.onClick.AddListener(() =>
            {
                this.SendCommand(new LoginCommand(
                    InputUsername.text,
                    InputPassword.text));
            });
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILoginPanelData ?? new UILoginPanelData();
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
