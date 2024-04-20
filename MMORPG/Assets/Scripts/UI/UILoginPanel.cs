using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UILoginPanelData : UIPanelData
	{
	}
	public partial class UILoginPanel : UIPanel, IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILoginPanelData ?? new UILoginPanelData();
			// please add init code here

			BtnCancel.onClick.AddListener(() =>
            {
                PanelHelper.FadeOut(gameObject, onComplete: CloseSelf);
                UIKit.OpenPanel<UILoginOperationPanel>();
			});

			BtnConfirm.onClick.AddListener(() =>
			{
				this.SendCommand(new LoginCommand(
					InputUsername.inputText.text,
					InputPassword.inputText.text));
            });
		}
		
		protected override void OnOpen(IUIData uiData = null)
        {
            PanelHelper.FadeIn(gameObject);
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

        public IArchitecture GetArchitecture()
        {
			return GameApp.Interface;
        }
    }
}
