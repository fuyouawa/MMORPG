using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UIEnterOperationPanelData : UIPanelData
	{
	}
	public partial class UIEnterOperationPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIEnterOperationPanelData ?? new UIEnterOperationPanelData();
			// please add init code here

			BtnEnter.onClick.AddListener(() =>
			{

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
