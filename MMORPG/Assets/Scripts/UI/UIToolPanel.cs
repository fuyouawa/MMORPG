using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UIToolPanelData : UIPanelData
	{
	}
	public partial class UIToolPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIToolPanelData ?? new UIToolPanelData();
			// please add init code here
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
