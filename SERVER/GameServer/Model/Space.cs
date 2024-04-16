using Common.Proto.Space;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Space
    {
        const int InvalidSpaceId = 0;

        public int SpaceId;
        public string Name;
        public string Description;
        public int Music;

        private Dictionary<int, Player> _playerSet = new();

        /// <summary>
        /// 角色进入场景
        /// </summary>
        public void PlayerEnter(Player player)
        {
            if (player.Character == null) return;
            Log.Information($"角色进入场景:[{player.Character.EntityId}]{player.Character.Name}");
            var res = new EntityEnterResponse();
            res.EntityList.Add(player.Character.ToNetEntity());

            lock (_playerSet)
            {
                // 广播新玩家加入
                foreach (var player_ in _playerSet)
                {
                    player_.Value.Channel.Send(res, null);
                }

                // 向新玩家投递已在场景中的所有玩家
                res.EntityList.Clear();
                foreach (var player_ in _playerSet)
                {
                    if (player_.Value.Character == null) return;
                    res.EntityList.Add(player_.Value.Character.ToNetEntity());
                }
                player.Channel.Send(res, null);
                _playerSet[player.Character.EntityId] = player;
            }
        }

        public void PlayerLeave(Player player)
        {
            if (player.Character == null) return;
            Log.Information($"角色离开场景:[{player.Character.EntityId}]{player.Character.Name}");

            var res = new EntityLeaveResponse();
            res.EntityId = player.Character.EntityId;

            lock (_playerSet) {
                _playerSet.Remove(res.EntityId);
                foreach (var player_ in _playerSet)
                {
                    player_.Value.Channel.Send(res, null);
                }
            }
            player.Character.SpeedId = InvalidSpaceId;
        }

        public void EntityUpdate(Entity entity)
        {
            var res = new EntitySyncResponse() { EntitySync = new() };
            // res.EntitySync.Status = ;
            res.EntitySync.Entity = entity.ToNetEntity();
            lock (_playerSet)
            {
                foreach (var player in _playerSet)
                {
                    if (player.Value.Character == null)
                    {
                        continue;
                    }
                    if (player.Value.Character.EntityId == entity.EntityId)
                    {
                        player.Value.Character.Position = res.EntitySync.Entity.Position.ToVector3();
                        player.Value.Character.Direction = res.EntitySync.Entity.Direction.ToVector3();
                    }
                    else
                    {
                        player.Value.Channel.Send(res, null);
                    }
                }
            }
        }
    }
}
