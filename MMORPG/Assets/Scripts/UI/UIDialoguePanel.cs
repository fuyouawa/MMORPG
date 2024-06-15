using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using MMORPG.Command;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Serilog;
using UnityEngine.InputSystem;
using MMORPG.Game;
using MMORPG.Event;
using MMORPG.System;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Rendering;

namespace MMORPG.UI
{
	public class UIDialoguePanelData : UIPanelData
	{
	}
	public partial class UIDialoguePanel : UIPanel, IController
    {
        private const string OptionButtonResName = "UIDialogueOptionButton";

        public TextMeshProUGUI NpcName;
        public TextMeshProUGUI Content;
        public RectTransform OptionButtonBox;

        private UIDialogueOptionButton _optionButtonPrefab;


        private void Awake()
        {
            
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


        public void DialogueNext()
        {
            this.SendCommand(new InteractCommand(0));
        }

        public void OnInteract(InteractEvent e)
        {
            ResLoader resLoader = ResLoader.Allocate();
            _optionButtonPrefab = resLoader.LoadSync<UIDialogueOptionButton>(OptionButtonResName);

            var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
            var dialogueDefine = dataManagerSystem.GetDialogueDefine(e.Resp.DialogueId);
            var entityManagerSystem = this.GetSystem<IEntityManagerSystem>();
            var entity = entityManagerSystem.GetEntityById(e.Resp.EntityId);
            var unitDefine = dataManagerSystem.GetUnitDefine(entity.UnitId);
            NpcName.text = unitDefine.Name;
            Content.text = dialogueDefine.Content;

            if (dialogueDefine.Options.Any())
            {
                var options = JsonConvert.DeserializeObject<int[]>(dialogueDefine.Options);
                foreach (var option in options)
                {
                    var tmpDefine = dataManagerSystem.GetDialogueDefine(option);
                    var button = Instantiate(_optionButtonPrefab, OptionButtonBox.transform);
                    button.GetComponent<UIDialogueOptionButton>().Content.text = tmpDefine.Content;
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
