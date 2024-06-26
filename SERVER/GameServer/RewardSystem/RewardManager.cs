using GameServer.EntitySystem;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using GameServer.PlayerSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace GameServer.RewardSystem
{
    public class RewardManager : Singleton<RewardManager>
    {
        private Random _random = new();

        private RewardManager() { }

        /// <summary>
        /// 发放奖励
        /// </summary>
        /// <param name="rewardId"></param>
        /// <param name="entity"></param>
        public void Distribute(int rewardId, Entity entity)
        {
            if (!DataManager.Instance.RewardDict.TryGetValue(rewardId, out var rewardDefine)) return;

            JArray rewardArr = JArray.Parse(rewardDefine.RewardList);

            if (rewardDefine.Type == "DropItem" || rewardDefine.Type == "InventoryItem")
            {
                foreach (JObject obj in rewardArr)
                {
                    var itemId = obj["ItemId"].Value<int>();
                    var number = obj["Number"].Value<int>();
                    var finalNumber = 0;
                    var probability = obj["Probability"].Value<float>();

                    if (probability < 1f)
                    {
                        for (int i = 0; i < number; i++)
                        {
                            if (_random.NextSingle() > probability) continue;
                            ++finalNumber;
                        }
                    }
                    else
                    {
                        finalNumber = number;
                    }

                    if (rewardDefine.Type == "Inventory")
                    {
                        var player = entity as Player;
                        if (player == null) continue;

                        finalNumber = player.Knapsack.AddItem(itemId, finalNumber);
                    }

                    if (finalNumber != 0) //rewardDefine.Type == "Drop")
                    {
                        entity.Map.DroppedItemManager.NewDroppedItemWithOffset(itemId, entity.Position.ToVector3(), Vector3.Zero, finalNumber, 1f);
                    }
                }
            }
            else if (rewardDefine.Type == "Buff")
            {
                foreach (JObject obj in rewardArr)
                {
                    var probability = obj["Probability"].Value<float>();
                    if (_random.NextSingle() > probability) continue;

                    var buffId = obj["BuffId"].Value<int>();
                    var actor = entity as Actor;
                    if (actor == null) continue;
                    actor.BuffManager.AddBuff(buffId);
                }
            }
        }
    }
}
