
using System.Linq;

using MMORPG.Command;
using UnityEngine;
using QFramework;
using MMORPG.Game;
using MMORPG.Event;
using MMORPG.System;
using TMPro;
using Newtonsoft.Json;

namespace MMORPG.UI
{
	public partial class UIDialoguePanel : MonoBehaviour, IController
    {
        private const string OptionButtonResName = "UIDialogueOptionButton";

        public TextMeshProUGUI NpcName;
        public TextMeshProUGUI Content;
        public RectTransform OptionButtonBox;

        private UIDialogueOptionButton _optionButtonPrefab;


        private void Awake()
        {
            
        }

        public void Interact()
        {
            // 没有选项按钮才允许点击面板继续对话
            if (OptionButtonBox.transform.childCount == 0)
            {
                this.SendCommand(new InteractCommand(0));
            }
        }

        public void ClearOptionButton()
        {
            for (int i = 0; i < OptionButtonBox.transform.childCount; i++)
            {
                Destroy(OptionButtonBox.GetChild(i).gameObject);
            }
        }

        public void OnInteract(InteractEvent e)
        {
            ResLoader resLoader = ResLoader.Allocate();
            _optionButtonPrefab = resLoader.LoadSync<UIDialogueOptionButton>(OptionButtonResName);

            var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
            var dialogueDefine = dataManagerSystem.GetDialogueDefine(e.Resp.DialogueId);
            var entityManagerSystem = this.GetSystem<IEntityManagerSystem>();
            var entity = entityManagerSystem.EntityDict[e.Resp.EntityId];
            NpcName.text = entity.UnitDefine.Name;
            Content.text = dialogueDefine.Content;
            ClearOptionButton();

            if (dialogueDefine.Options.Any())
            {
                var options = JsonConvert.DeserializeObject<int[]>(dialogueDefine.Options);
                int j = 0;
                foreach (var option in options)
                {
                    var tmpDefine = dataManagerSystem.GetDialogueDefine(option);
                    var button = Instantiate(_optionButtonPrefab, OptionButtonBox.transform);
                    var script = button.GetComponent<UIDialogueOptionButton>();
                    script.Content.text = tmpDefine.Content;
                    script.Idx = ++j;
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
