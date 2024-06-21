using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Fight;
using Serilog;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using Org.BouncyCastle.Ocsp;

namespace GameServer.FightSystem
{
    public class FightManager
    {
        private Map _map;
        private Queue<CastInfo> _castQueue = new();

        public FightManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var cast in _castQueue)
            {
                RunSpell(cast);
            }
        }

        public void AddSkillCast(CastInfo info)
        {
            _castQueue.Enqueue(info);
            
        }

        private void RunSpell(CastInfo info)
        {
            var caster = EntityManager.Instance.GetEntity(info.CasterId) as Actor;
            if (caster == null)
            {
                Log.Warning("[FightManager.Cast]: 释放者不存在.");
                return;
            }
            caster.Spell.Cast(info);
        }

    }
}
