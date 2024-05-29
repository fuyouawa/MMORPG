using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Model;

namespace GameServer.Manager
{
    public class FightManager
    {
        private Map _map;
        private Queue<SkillCastInfo> _castQueue = new();
        private Queue<SkillCastInfo> _backupCastQueue = new();

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
                var castInfo = _backupCastQueue.Dequeue();

            }
        }

        public void AddSkillCast(SkillCastInfo skillCast)
        {
            lock (_castQueue)
            {
                _castQueue.Enqueue(skillCast);
            }
        }
    }
}
