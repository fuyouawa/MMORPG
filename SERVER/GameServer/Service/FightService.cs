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
            if (sender.User?.Player == null) return;
            var player = sender.User.Player;
            if (req.Info.CasterId != player.EntityId)
            {
                Log.Debug($"{sender}施法者不匹配");
                sender.Send(new SpellFailResponse()
                {
                    CasterId = req.Info.CasterId,
                    SkillId = req.Info.SkillId,
                    Reason = CastResult.UnmatchedCaster
                });
                return;
            }
            player.Map.FightManager.AddSkillCast(req.Info);
        }
    }
}
