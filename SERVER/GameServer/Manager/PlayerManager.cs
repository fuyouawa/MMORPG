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
        private Map _map;
        private Dictionary<int, Player> _playerDict = new();
        
        public PlayerManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var player in _playerDict.Values)
            {
                player.Update();
            }
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <returns></returns>
        public Player NewPlayer(User user, int unitId, Vector3 pos, Vector3 dire, string name)
        {
            var player = new Player(EntityManager.Instance.NewEntityId(), unitId, _map, name, user)
            {
                Position = pos,
                Direction = dire,
            };
            EntityManager.Instance.AddEntity(player);

            lock (_playerDict)
            {
                _playerDict.Add(player.EntityId, player);
            }
            _map.EntityEnter(player);

            player.Start();
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
        /// 将消息广播给能够观察到sender的玩家，排除sender
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
                var list = _map.GetEntityFollowerList(sender, entity => entity.EntityType == EntityType.Player);
                foreach (var entity in list)
                {
                    var player = (Player)entity;
                    player.User.Channel.Send(msg);
                    //Log.Debug($"响应{sender.EntityId}的同步请求, 广播给:{player.EntityId}");
                }
            }
        }
    }
}
