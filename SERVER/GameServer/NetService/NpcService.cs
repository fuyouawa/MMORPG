using MMORPG.Common.Network;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using MMORPG.Common.Proto.Player;
using GameServer.Db;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using MMORPG.Common.Proto.Npc;

namespace GameServer.NetService
{
    public class NpcService : ServiceBase<NpcService>
    {
        public void OnConnect(NetChannel sender)
        {
        }

        public void OnChannelClosed(NetChannel sender)
        {
        }

        public void OnHandle(NetChannel sender, InteractRequest req)
        {
            if (sender.User == null || sender.User.Player == null) return;
            var player = sender.User.Player;
            var npc = player.InteractingNpc;
            var res = new InteractResponse()
            {
                Error = NetError.InvalidEntity,
            };
            if (npc == null)
            {
                // 查找距离最近的Npc
                var entity = player.Map.GetEntityFollowingNearest(player, entity => entity.EntityType == EntityType.Npc);
                do
                {
                    if (entity == null) break;
                    npc = entity as NpcSystem.Npc;
                    if (npc == null) break;
                    var distance = Vector2.Distance(player.Position.ToVector2(), npc.Position.ToVector2());
                    if (distance > 1) break;
                    res.Error = NetError.Success;
                } while (false);
                if (res.Error != NetError.Success)
                {
                    sender.Send(res, null);
                    return;
                }
                player.InteractingNpc = npc;
                player.CurrentDialogueId = player.DialogueManager.GetDialogueId(npc.NpcDefine.ID);
            }
            else
            {
                res.Error = NetError.Success;
            }

            res.EntityId = npc.EntityId;
        
            var dialogueDefine = DataManager.Instance.DialogueDict[player.CurrentDialogueId];
            if (req.SelectIdx != 0)
            {
                var options = DataHelper.ParseJson<int[]>(dialogueDefine.Options);

                if (options== null || req.SelectIdx < 0 || req.SelectIdx > options.Length)
                {
                    Log.Error("客户端传入了错误的对话选择索引");
                    return;
                }

                // 选择了某项，将该项的跳转告知客户端
                dialogueDefine = DataManager.Instance.DialogueDict[options[req.SelectIdx]];
                player.CurrentDialogueId = dialogueDefine.Jump;
            }

            res.DialogueId = player.CurrentDialogueId;
            // 如果需要保存当前的对话进度
            if (dialogueDefine.SaveDialogueId != 0)
            {
                player.DialogueManager.SaveDialogueId(npc.NpcDefine.ID, dialogueDefine.SaveDialogueId);
            }

            if (req.SelectIdx != 0)
            {
                dialogueDefine = DataManager.Instance.DialogueDict[player.CurrentDialogueId];
            }

            // 转到下一段对话
            player.CurrentDialogueId = dialogueDefine.Jump;
            if (player.CurrentDialogueId == 0)
            {
                if (dialogueDefine.Options.Any())
                {
                    // 是选择项对话，等待选择
                    player.CurrentDialogueId = res.DialogueId;
                }
                else
                {
                    // 需要结束对话，从这里开始停止
                    player.InteractingNpc = null;
                }
            }
            sender.Send(res, null);
        }
    }
}
