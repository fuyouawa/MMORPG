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
        private Space _space;

        public SpawnManager(Space space)
        {
            _space = space;
        }

        private void Spawn()
        {
            //_space.MonsterManager.NewMonster();
        }
    }
}
