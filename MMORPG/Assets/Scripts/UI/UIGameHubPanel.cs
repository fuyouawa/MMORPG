using MMORPG.Tool;
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
            if (PlayerKnapsackPanel.gameObject.activeSelf)
            {
                PanelHelper.FadeOut(PlayerKnapsackPanel.gameObject);
            }
            else
            {
                PanelHelper.FadeIn(PlayerKnapsackPanel.gameObject);
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
