using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Tool;
using GameServer.Db;
using GameServer.Tool;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using GameServer.Manager;
using GameServer.UserSystem;

namespace GameServer.PlayerSystem
{
    /// <summary>
    /// 角色管理器
    /// 负责管理地图内的所有玩家
    /// </summary>
    public class PlayerManager
    {
        public const float UpdateDbSecond = 10;

        private Map _map;
        private Dictionary<int, Player> _playerDict = new();
        private float _updateDbCountdown;
        private Queue<Player> _leavePlayerQueue = new();

        public PlayerManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {
            _updateDbCountdown = UpdateDbSecond;
        }

        public void Update()
        {
            _updateDbCountdown -= Time.DeltaTime;
            if (_updateDbCountdown <= 0)
            {
                var characters = new List<DbCharacter>();
                foreach (var player in _playerDict.Values)
                {
                    characters.Add(player.ToDbCharacter());
                }
                foreach (var player in _leavePlayerQueue)
                {
                    characters.Add(player.ToDbCharacter());
                }
                _leavePlayerQueue.Clear();

                SqlDb.Connection.Update<DbCharacter>()
                    .SetSource(characters)
                    .IgnoreColumns(c => new { c.Id, c.Name, c.UserId, c.UnitId })
                    .ExecuteAffrowsAsync();
                _updateDbCountdown = UpdateDbSecond;
            }
        }



        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <returns></returns>
        public Player NewPlayer(User user, DbCharacter dbCharacter, Vector3 pos, Vector3 dire)
        {
            var player = new Player(EntityManager.Instance.NewEntityId(), dbCharacter, 
                DataManager.Instance.UnitDict[dbCharacter.UnitId], _map, pos, dire, user, dbCharacter.Level);
            player.Start();

            EntityManager.Instance.AddEntity(player);
            
            _playerDict.Add(player.EntityId, player);
            
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
            _playerDict.Remove(player.EntityId);

            _leavePlayerQueue.Enqueue(player);
        }

        /// <summary>
        /// 将消息广播给能够观察到sender的玩家，排除sender
        /// 没有sender则为全图广播
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sender"></param>
        public void Broadcast(Google.Protobuf.IMessage msg, Entity sender, bool sendToFollower = true, bool excludeSender = true)
        {
            if (!sendToFollower)
            {
                foreach (var player in _playerDict.Values)
                {
                    if (excludeSender && player.EntityId == sender.EntityId) continue;
                    player.User.Channel.Send(msg);
                }
            }
            else
            {
                _map.ScanEntityFollower(sender, entity =>
                {
                    if (entity.EntityType != EntityType.Player) return;
                    var player = (Player)entity;
                    player.User.Channel.Send(msg);
                });
                if (!excludeSender)
                {
                    var player = sender as Player;
                    if (player != null)
                    {
                        player.User.Channel.Send(msg);
                    }
                }
            }
        }
    }
}
