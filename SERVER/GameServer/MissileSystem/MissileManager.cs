using MMORPG.Common.Proto.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using GameServer.FightSystem;
using GameServer.Manager;

namespace GameServer.MissileSystem
{
    public class MissileManager
    {
        private Map _map;
        private Dictionary<int, Missile> _missileDict = new();

        public MissileManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var missile in _missileDict.Values)
            {
                missile.Update();
            }
        }

        public Missile NewMissile(int unitId, Vector3 pos, Vector3 dire, 
            float range, float speed, CastTarget castTarget, Action<Entity> hitCallback)
        {
            var missile = new Missile(EntityManager.Instance.NewEntityId(), DataManager.Instance.UnitDict[unitId], _map, 
                pos, dire, range, speed, castTarget, hitCallback);
            EntityManager.Instance.AddEntity(missile);

            lock (_missileDict)
            {
                _missileDict.Add(missile.EntityId, missile);
            }
            _map.EntityEnter(missile);

            missile.Start();
            return missile;
        }
    }
}
