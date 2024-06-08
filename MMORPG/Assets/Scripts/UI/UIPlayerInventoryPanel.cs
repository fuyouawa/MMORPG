using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UIPlayerInventoryPanelData : UIPanelData
	{
	}
	public partial class UIPlayerInventoryPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIPlayerInventoryPanelData ?? new UIPlayerInventoryPanelData();
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
