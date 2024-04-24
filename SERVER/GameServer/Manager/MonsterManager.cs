using Common.Proto.Entity;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 怪物管理器
    /// 负责管理地图内的所有怪物
    /// 线程安全
    /// </summary>
    public class MonsterManager
    {
        private Dictionary<int, Monster> _monsterDict;
        private Space _space;

        public MonsterManager(Space space)
        {
            _monsterDict = new();
            _space = space;
        }

        public Monster NewMonster(Vector3 pos, Vector3 dire, string name)
        {
            var monster = new Monster(_space, name, pos)
            {
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Monster,
                Position = pos,
                Direction = dire,
                
                Speed = 5,
            };
            EntityManager.Instance.AddEntity(monster);
            return monster;
        }
    }
}
