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

namespace GameServer.RewardSystem
{
    public class RewardManager : Singleton<RewardManager>
    {
        private struct RewardFormat
        {
            public int ItemId;
            public int Number;
            public float Probability;
        }

        private Random _random = new();

        /// <summary>
        /// 发放奖励
        /// </summary>
        /// <param name="rewardId"></param>
        /// <param name="entity"></param>
        public void Distribute(int rewardId, Entity entity)
        {
            if (!DataManager.Instance.RewardDict.TryGetValue(rewardId, out var rewardDefine))
            {
                return;
            }
            var formatList = JsonConvert.DeserializeObject<RewardFormat[]>(rewardDefine.RewardList);
            if (formatList == null)
            {
                return;
            }

            int number = 0;
            foreach (var format in formatList)
            {
                if (format.Probability < 1f)
                {
                    for (int i = 0; i < format.Number; i++)
                    {
                        if (_random.NextSingle() >= format.Probability)
                        {
                            ++number;
                        }
                    }
                }
                else
                {
                    number = format.Number;
                }

                if (rewardDefine.Type == "Inventory")
                {
                    var player = entity as Player;
                    if (player == null)
                    {
                        continue;
                    }
                    number = player.Knapsack.AddItem(format.ItemId, number);
                }

                if (number != 0) //rewardDefine.Type == "Drop")
                {
                    entity.Map.DroppedItemManager.NewDroppedItemWithOffset(format.ItemId, entity.Position, Vector3.Zero, number, 1f);
                }
            }
        }
    }
}
