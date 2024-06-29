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
            
            this.RegisterEvent<InteractEvent>(e =>
            {
                if (e.Resp.Error != NetError.Success)
                {
                    return;
                }
                if (e.Resp.DialogueId == 0)
                {
                    if (DialoguePanel.gameObject.activeSelf)
                    {
                        ShowPlayerHub();
                        PanelHelper.FadeOut(DialoguePanel.gameObject, onComplete: () =>
                        {
                            DialoguePanel.ClearOptionButton();
                        });
                    }
                    return;
                }
                if (!DialoguePanel.gameObject.activeSelf)
                {
                    HidePlayerHub();
                    PanelHelper.FadeIn(DialoguePanel.gameObject, onComplete: () =>
                    {
                        DialoguePanel.OnInteract(e);
                    });
                }
                else
                {
                    DialoguePanel.OnInteract(e);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<MinePlayerDeathEvent>(OnMinePlayerDeath)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<MinePlayerReviveEvent>(OnMinePlayerRevive)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }


        private void OnMinePlayerDeath(MinePlayerDeathEvent e)
        {
            PanelHelper.FadeIn(RevivePanel.gameObject);
            RevivePanel.BeginRevive(e.Player.ReviveTime);
        }

        private void OnMinePlayerRevive(MinePlayerReviveEvent e)
        {
            PanelHelper.FadeOut(RevivePanel.gameObject);
            RevivePanel.EndRevive();
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameHubPanelData ?? new UIGameHubPanelData();
            // please add init code here
        }
        
        private void OnInteract(InputAction.CallbackContext obj)
        {
            DialoguePanel.Interact();
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

        private void HidePlayerHub()
        {
            PanelHelper.FadeOut(ChatPanel.gameObject);
            PanelHelper.FadeOut(PlayerPropertyPanel.gameObject);
            PanelHelper.FadeOut(SkillPanel.gameObject);
        }

        private void ShowPlayerHub()
        {
            PanelHelper.FadeIn(ChatPanel.gameObject);
            PanelHelper.FadeIn(PlayerPropertyPanel.gameObject);
            PanelHelper.FadeIn(SkillPanel.gameObject);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
