using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Serilog;
using UnityEngine.InputSystem;

namespace MMORPG.UI
{
	public class UIDialoguePanelData : UIPanelData
	{
	}
	public partial class UIDialoguePanel : UIPanel
    {
        private GameInputControls _inputControls;

        private void Awake()
        {
            _inputControls = new();
            _inputControls.Player.Interact.started += OnInteract;
        }

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIDialoguePanelData ?? new UIDialoguePanelData();
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
        private void OnEnable()
        {
            _inputControls.Enable();
        }

        private void OnDisable()
        {
            _inputControls.Disable();
        }

        private void OnInteract(InputAction.CallbackContext obj)
        {

        }

        public void Click()
        {

        }
	}
}
