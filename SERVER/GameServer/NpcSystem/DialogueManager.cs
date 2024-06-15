using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using GameServer.PlayerSystem;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Npc;
using Serilog;

namespace GameServer.NpcSystem
{
    public class DialogueManager
    {
        public Player PlayerOwner;
        private Dictionary<int, DialogueRecord> _recordDict = new();        // key:NpcId
        private bool _hasChange = false;
        private DialogueInfo? _dialogueInfo;
        public DialogueManager(Player playerOwner)
        {
            PlayerOwner = playerOwner;
        }

        public DialogueInfo GetDialogueInfo()
        {
            if (_hasChange || _dialogueInfo == null)
            {
                _dialogueInfo = new();
                _dialogueInfo.DialogueArr.AddRange(_recordDict.Select(x => x.Value));
            }
            return _dialogueInfo;
        }

        public void LoadDialogueInfo(byte[]? dialogueInfoData)
        {
            if (dialogueInfoData == null)
            {
                return;
            }
            DialogueInfo info = DialogueInfo.Parser.ParseFrom(dialogueInfoData);
            foreach (var record in info.DialogueArr)
            {
                if (!DataManager.Instance.NpcDict.TryGetValue(record.NpcId, out var define))
                {
                    Log.Error($"NpcId不存在:{record.NpcId}");
                    continue;
                }
                _recordDict[record.NpcId] = record;
            }
        }
    }
}
