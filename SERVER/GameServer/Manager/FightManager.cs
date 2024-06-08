using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Fight;
using GameServer.Model;
using Serilog;

namespace GameServer.Manager
{
    public class FightManager
    {
        private Map _map;
        private Queue<CastInfo> _castQueue = new();
        private Queue<CastInfo> _backupCastQueue = new();

        public FightManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            lock (_castQueue)
            {
                (_backupCastQueue, _castQueue) = (_castQueue, _backupCastQueue);
            }

            while (_backupCastQueue.Any())
            {
                var req = _backupCastQueue.Dequeue();
                RunSpell(req);
            }

        }

        public void AddSkillCast(CastInfo info)
        {
            lock (_castQueue)
            {
                _castQueue.Enqueue(info);
            }
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
