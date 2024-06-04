using Common.Network;
using Common.Proto.Base;
using Common.Proto.User;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Fight;

namespace GameServer.Service
{
    public class FightService : ServiceBase<FightService>
    {
        public void OnHandle(NetChannel sender, SpellRequest req)
        {
            if (sender.User == null || sender.User.Player == null) return;
            var player = sender.User.Player;
            if (req.CasterId != player.EntityId)
            {
                Log.Debug("施法者不匹配");
                return;
            }
            player.Map.FightManager.AddSkillCast(req);
        }


    }
}
