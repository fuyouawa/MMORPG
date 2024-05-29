using Common.Proto.Entity;
using GameServer.Model;
using GameServer.Tool;
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
        private Dictionary<int, Monster> _monsterDict = new ();
        private Map _map;

        public MonsterManager(Map map)
        {
            _map = map;
        }

        public Monster NewMonster(int unitId, Vector3 pos, Vector3 dire, string name)
        {
            var monster = new Monster(_map, name, pos)
            {
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Monster,
                UnitId = unitId,
                Position = pos,
                Direction = dire,

                //Speed = DataHelper.GetUnitDefine(unitId).Speed,
            };
            EntityManager.Instance.AddEntity(monster);

            lock (_monsterDict)
            {
                _monsterDict.Add(monster.EntityId, monster);
            }
            _map.EntityEnter(monster);

            monster.Start();
            return monster;
        }

        public void Start()
        {

        }

        public void Update()
        {

        }
    }
}
