using System;
using MMORPG.Tool;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.InputSystem;
using MMORPG.Command;
using MMORPG.Common.Proto.Base;
using MMORPG.Game;
using MMORPG.Event;
using MMORPG.System;

namespace MMORPG.UI
{
	public class UIGameHubPanelData : UIPanelData
	{
	}
	public partial class UIGameHubPanel : UIPanel, IController
    {
        private GameInputControls _inputControls;

        private void Awake()
        {
            _inputControls = new();

            _inputControls.UI.Inventory.started += OnSwitchInventory;
            _inputControls.Player.Interact.started += OnInteract;
        }

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameHubPanelData ?? new UIGameHubPanelData();
            // please add init code here

            this.RegisterEvent<InteractEvent>(e =>
            {
                if (e.Resp.Error != NetError.Success)
                {
                    return;
                }
                PanelHelper.FadeIn(DialoguePanel.gameObject);
                DialoguePanel.GetComponent<UIDialoguePanel>().OnInteract(e);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

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

        private void OnInteract(InputAction.CallbackContext obj)
        {
            this.SendCommand(new InteractCommand(0));
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

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
