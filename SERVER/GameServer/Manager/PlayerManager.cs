using Common.Proto.Entity;
using Common.Tool;
using GameServer.Db;
using GameServer.Tool;
using GameServer.Model;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace GameServer.Manager
{
    /// <summary>
    /// 角色管理器
    /// 负责管理地图内的所有玩家
    /// 线程安全
    /// </summary>
    public class PlayerManager
    {
        private Dictionary<int, Player> _playerDict = new();
        private Map _map;

        public PlayerManager(Map map)
        {
            _map = map;
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <returns></returns>
        public Player NewPlayer(User user, int unitId, Vector3 pos, Vector3 dire, string name)
        {
            var player = new Player(_map, name, user)
            {
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Player,
                UnitId = unitId,
                Position = pos,
                Direction = dire,
                ViewRange = Player.DefaultViewRange,
                Speed = 5,
            };
            EntityManager.Instance.AddEntity(player);

            lock (_playerDict)
            {
                _playerDict.Add(player.EntityId, player);
            }
            _map.EntityEnter(player);
            return player;
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(Player player)
        {
            EntityManager.Instance.RemoveEntity(player);
            lock (_playerDict)
            {
                _playerDict.Remove(player.EntityId);
            }
        }

        /// <summary>
        /// 将消息广播给sender周围的玩家，排除sender
        /// 没有sender则为全图广播
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sender"></param>
        public void Broadcast(Google.Protobuf.IMessage msg, Entity? sender = null)
        {
            if (sender == null)
            {
                lock (_playerDict)
                {
                    foreach (var player in _playerDict.Values)
                    {
                        if (sender != null && player.EntityId == sender.EntityId) continue;
                        player.User.Channel.Send(msg);
                    }
                }
            }
            else
            {
                var list = _map.GetEntityViewEntityList(sender, e => e.EntityType == EntityType.Player);
                foreach (var entity in list)
                {
                    var player = (Player)entity;
                    player.User.Channel.Send(msg);
                    Log.Debug($"响应{sender.EntityId}的同步请求, 广播给:{player.EntityId}");
                }
            }
        }
    }
}
