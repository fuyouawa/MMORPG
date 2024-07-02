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
using GameServer.EntitySystem;
using GameServer.Manager;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using MMORPG.Common.Proto.Npc;
using MMORPG.Common.Tool;
using Newtonsoft.Json;
using GameServer.NpcSystem;

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
            UpdateManager.Instance.AddTask(() =>
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
                    var entity =
                        player.Map.GetEntityFollowingNearest(player, entity => entity.EntityType == EntityType.Npc);
                    do
                    {
                        if (entity == null) break;
                        npc = entity as NpcSystem.Npc;
                        if (npc == null) break;
                        var distance = Vector2.Distance(player.Position, npc.Position);
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

                if (player.CurrentDialogueId == 0)
                {
                    // 需要结束对话
                    res.DialogueId = 0;
                    player.InteractingNpc = null;
                    sender.Send(res, null);
                    return;
                }

                res.EntityId = npc.EntityId;

                var dialogueDefine = DataManager.Instance.DialogueDict[player.CurrentDialogueId];
                if (req.SelectIdx != 0)
                {
                    var options = DataHelper.ParseIntegers(dialogueDefine.Options);

                    if (req.SelectIdx < 0 || req.SelectIdx > options.Length)
                    {
                        Log.Error("客户端传入了错误的对话选择索引");
                        return;
                    }

                    // 选择了某项，将该项的跳转告知客户端
                    dialogueDefine = DataManager.Instance.DialogueDict[options[req.SelectIdx - 1]];
                    player.CurrentDialogueId = dialogueDefine.Jump;
                    if (player.CurrentDialogueId == 0)
                    {
                        // 选项没有可继续跳转的对话，结束
                        res.DialogueId = 0;
                        player.InteractingNpc = null;
                        sender.Send(res, null);
                        return;
                    }
                }

                res.DialogueId = player.CurrentDialogueId;
                // 如果需要保存当前的对话进度
                if (dialogueDefine.SaveDialogueId != 0)
                {
                    player.DialogueManager.SaveDialogueId(npc.NpcDefine.ID, dialogueDefine.SaveDialogueId);
                }

                bool next = true;
                if (dialogueDefine.AcceptTask != "")
                {
                    // 接取任务
                    var tmp = JsonConvert.DeserializeObject<int[]>(dialogueDefine.AcceptTask);
                    if (!player.TaskManager.AcceptTask(tmp[0]))
                    {
                        player.CurrentDialogueId = 0;
                        res.DialogueId = tmp[1];
                        next = false;
                    }
                }
                
                if (dialogueDefine.SubmitTask != "")
                {
                    // 提交任务
                    var tmp = JsonConvert.DeserializeObject<int[]>(dialogueDefine.SubmitTask);
                    if (!player.TaskManager.SubmitTask(tmp[0]))
                    {
                        player.CurrentDialogueId = 0;
                        res.DialogueId = tmp[1];
                        next = false;
                    }
                }

                if (req.SelectIdx != 0 && player.CurrentDialogueId != 0)
                {
                    dialogueDefine = DataManager.Instance.DialogueDict[player.CurrentDialogueId];
                }

                if (next)
                {
                    // 转到下一段对话
                    player.CurrentDialogueId = dialogueDefine.Jump;
                }

                if (player.CurrentDialogueId == 0)
                {
                    if (dialogueDefine.Options.Any())
                    {
                        // 是选择项对话，等待选择
                        player.CurrentDialogueId = res.DialogueId;
                    }
                    else
                    {
                        // 需要结束对话，等待下一次停止响应时停止
                        // player.InteractingNpc = null;
                        player.CurrentDialogueId = 0;
                    }
                }

                sender.Send(res, null);
            });
        }

        public void OnHandle(NetChannel sender, QueryDialogueIdRequest req)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                if (sender.User == null || sender.User.Player == null) return;
                var player = sender.User.Player;

                var e = EntityManager.Instance.GetEntity(req.EntityId);
                var res = new QueryDialogueIdResponse()
                {
                    Error = NetError.Success,
                };
                if (e == null || !(e is Npc))
                {
                    res.Error = NetError.InvalidEntity;
                }
                else
                {
                    var npc = (Npc)e;
                    res.EntityId = e.EntityId;
                    res.DialogueId = player.DialogueManager.GetDialogueId(npc.NpcDefine.ID);
                }
                sender.Send(res, null);
            });
        }
    }
}
