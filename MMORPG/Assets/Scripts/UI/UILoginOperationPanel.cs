using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UILoginOperationPanelData : UIPanelData
	{
	}
	public partial class UILoginOperationPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILoginOperationPanelData ?? new UILoginOperationPanelData();
			// please add init code here

			BtnLogin.onClick.AddListener(() =>
            {
                PanelHelper.FadeOut(gameObject, onComplete: CloseSelf);
                UIKit.OpenPanel<UILoginPanel>();
			});
            BtnRegister.onClick.AddListener(() =>
            {
                PanelHelper.FadeOut(gameObject, onComplete: CloseSelf);
                UIKit.OpenPanel<UIRegisterPanel>();
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
	}
}
