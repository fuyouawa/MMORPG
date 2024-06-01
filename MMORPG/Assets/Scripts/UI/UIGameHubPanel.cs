using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.InputSystem;

namespace MMORPG.UI
{
	public class UIGameHubPanelData : UIPanelData
	{
	}
	public partial class UIGameHubPanel : UIPanel
    {
        private GameInputControls _inputControls;
        private bool _isInventoryOpen = false;

        private void Awake()
        {
            _inputControls = new();

            _inputControls.UI.Inventory.started += OnSwitchInventory;
        }

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameHubPanelData ?? new UIGameHubPanelData();
			// please add init code here
        }

        private void OnSwitchInventory(InputAction.CallbackContext obj)
        {
            if (_isInventoryOpen)
            {
                UIKit.ClosePanel<UIPlayerInventoryPanel>();
            }
            else
            {
                UIKit.OpenPanel<UIPlayerInventoryPanel>();
            }
        }

        private void OnEnable()
        {
            _inputControls.Enable();
        }

        private void OnDisable()
        {
            _inputControls.Disable();
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
