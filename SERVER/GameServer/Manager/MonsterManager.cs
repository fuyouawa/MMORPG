using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class MonsterManager
    {
        private Dictionary<int, Monster> _monsterDict;
        private Space _space;

        public MonsterManager(Space space)
        {
            _space = space;
        }

        public Monster NewMonster(Vector3 pos, Vector3 dire, string name)
        {
            var monster = new Monster()
            {
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Monster,
                Position = pos,
                Direction = dire,
                
                Name = name,
                Space = _space,
                Speed = 5,

            };
            EntityManager.Instance.AddEntity(monster);
            return monster;
        }
    }
}
