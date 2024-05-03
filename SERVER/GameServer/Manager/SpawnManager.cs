using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 实体生成器
    /// </summary>
    public class Spawner
    {
        public SpawnDefine Define;
        public SpawnManager Manager;

        public Spawner(SpawnManager manager, SpawnDefine define)
        {
            Manager = manager;
            Define = define;
        }
    }


    /// <summary>
    /// 实体生成管理器
    /// </summary>
    public class SpawnManager
    {
        private Map _map;
        public List<Spawner> Rules = new();

        public SpawnManager(Map map)
        {
            _map = map;

            var rules = DataManager.Instance.SpawnDict.Values.Where(r => r.MapId == _map.MapId);
            foreach (var rule in rules)
            {
                Rules.Add(new(this, rule));
            }
        }

        private void Spawn()
        {


            //_map.MonsterManager.NewMonster();
        }
    }
}
