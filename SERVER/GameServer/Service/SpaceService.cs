using Common.Network;
using Common.Proto.Player;
using Common.Proto.Space;
using GameServer.Manager;
using GameServer.Model;
using GameServer.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SpaceService : ServiceBase<SpaceService>
    {
        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null) return;
            sender.Player.Space?.PlayerLeave(sender.Player);
        }

        public void OnHandle(NetChannel sender, EntitySyncRequest request)
        {
            if (sender.Player == null) return;
            sender.Player.Space?.EntityUpdate(request.EntitySync.Entity.ToEntity());
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
