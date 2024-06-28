using UnityEngine;
using QFramework;
using MMORPG.Command;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.System;
using Newtonsoft.Json;

namespace MMORPG.UI
{
	public partial class DialoguePanel : ViewController
	{
        public UIDialogueOptionButton OptionButtonPrefab;

        void Start()
		{
			// Code Here
		}

        public void Interact()
        {
            // 没有选项按钮才允许点击面板继续对话
            if (GroupOptionBox.childCount == 0)
            {
                this.SendCommand(new InteractCommand(0));
            }
        }

        public void ClearOptionButton()
        {
            for (int i = GroupOptionBox.childCount - 1; i >= 0; i--)
            {
                var child = GroupOptionBox.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        public void OnInteract(InteractEvent e)
        {
            var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
            var dialogueDefine = dataManagerSystem.GetDialogueDefine(e.Resp.DialogueId);
            var entityManagerSystem = this.GetSystem<IEntityManagerSystem>();
            var entity = entityManagerSystem.EntityDict[e.Resp.EntityId];
            TextName.text = entity.UnitDefine.Name;
            Content.text = dialogueDefine.Content;
            ClearOptionButton();

            if (!string.IsNullOrEmpty(dialogueDefine.Options))
            {
                var options = JsonConvert.DeserializeObject<int[]>(dialogueDefine.Options);
                int j = 0;
                foreach (var option in options)
                {
                    var tmpDefine = dataManagerSystem.GetDialogueDefine(option);
                    var button = Instantiate(OptionButtonPrefab, GroupOptionBox);
                    var script = button.GetComponent<UIDialogueOptionButton>();
                    script.Content.text = tmpDefine.Content;
                    script.Idx = ++j;
                }
            }
        }
    }
}
