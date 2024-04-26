using Common.Proto.Entity;
using Common.Tool;
using GameServer.Db;
using GameServer.Tool;
using GameServer.Unit;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 角色管理器
    /// 负责管理地图内的所有角色
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
        /// 从地图中创建
        /// </summary>
        /// <returns></returns>
        public Player NewPlayer(User user, Vector3 pos, Vector3 dire, string name)
        {
            var player = new Player(_map, name, user)
            {
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Player,
                Position = pos,
                Direction = dire,

                Speed = 5,
            };
            EntityManager.Instance.AddEntity(player);

            lock (_playerDict)
            {
                _playerDict.Add(player.EntityId, player);
            }

            return player;
        }

        /// <summary>
        /// 从地图中删除
        /// </summary>
        /// <param name="player"></param>
        public void RemoveCharacter(Player player)
        {
            EntityManager.Instance.RemoveEntity(player);
            lock (_playerDict)
            {
                _playerDict.Remove(player.EntityId);
            }
            player.Map = null;
        }

        /// <summary>
        /// 将消息广播给sender周围的，排除sender
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
                    foreach (var character in _playerDict.Values)
                    {
                        if (sender != null && character.EntityId == sender.EntityId) continue;
                        character.User.Channel.Send(msg, null);
                    }
                }
            }
            else
            {
                var list = _map.GetEntityViewEntityList(sender, e => e.EntityType == EntityType.Player);
                foreach (var entity in list)
                {
                    var player = entity as Player;
                    player?.User.Channel.Send(msg, null);
                }
            }
        }
    }
}
