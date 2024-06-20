using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using GameServer.Manager;

namespace GameServer.MonsterSystem
{
    /// <summary>
    /// 怪物管理器
    /// 负责管理地图内的所有怪物
    /// 线程安全
    /// </summary>
    public class MonsterManager
    {
        private Dictionary<int, Monster> _monsterDict = new();
        private Map _map;

        public MonsterManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var monster in _monsterDict.Values)
            {
                monster.Update();
            }
        }

        public Monster NewMonster(int unitId, Vector3 pos, Vector3 dire, string name)
        {
            var monster = new Monster(EntityManager.Instance.NewEntityId(), DataManager.Instance.UnitDict[unitId], _map, name, pos, dire);
            EntityManager.Instance.AddEntity(monster);

            lock (_monsterDict)
            {
                _monsterDict.Add(monster.EntityId, monster);
            }
            _map.EntityEnter(monster);

            monster.Start();
            return monster;
        }

    }
}
