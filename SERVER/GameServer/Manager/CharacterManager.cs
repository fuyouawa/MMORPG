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
    public class CharacterManager
    {
        private Dictionary<int, Character> _characterDict = new();
        private Space _space;

        public CharacterManager(Space space)
        {
            _space = space;
        }

        /// <summary>
        /// 从地图中创建角色
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <param name="dire"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Character NewCharacter(Player player, Vector3 pos, Vector3 dire, string name)
        {
            var character = new Character()
            {
                Player = player,
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Character,
                Position = pos,
                Direction = dire,

                Name = name,
                Space = _space,
                Speed = 5,
            };
            EntityManager.Instance.AddEntity(character);

            lock (_characterDict)
            {
                _characterDict.Add(character.EntityId, character);
            }

            return character;
        }

        /// <summary>
        /// 从地图中删除角色
        /// </summary>
        /// <param name="character"></param>
        public void RemoveCharacter(Character character)
        {
            EntityManager.Instance.RemoveEntity(character);
            lock (_characterDict)
            {
                _characterDict.Remove(character.EntityId);
            }
            character.Space = null;
        }

        /// <summary>
        /// 将消息广播给sender周围的角色，排除sender
        /// 没有sender则为全图广播
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sender"></param>
        public void Broadcast(Google.Protobuf.IMessage msg, Entity sender = null)
        {
            if(sender == null)
            {
                lock (_characterDict)
                {
                    foreach (var character in _characterDict.Values)
                    {
                        if (sender != null && character.EntityId == sender.EntityId) continue;
                        character.Player.Channel.Send(msg, null);
                    }
                }
            }
            else
            {
                var list = _space.GetEntityViewEntityList(sender, EntityType.Character);
                foreach (var entity in list)
                {
                    var character = entity as Character;
                    character.Player.Channel.Send(msg, null);
                }
            }
        }
    }
}
