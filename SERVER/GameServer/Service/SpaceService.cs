using Common.Network;
using Common.Proto.Player;
using Common.Proto.Space;
using GameServer.Mgr;
using GameServer.Model;
using GameServer.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class SpaceService : ServiceBase<SpaceService>
    {
        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            var space = SpaceMgr.Instance.GetSpaceById(sender.Player.Character.SpeedId);
            space?.PlayerLeave(sender.Player);
        }

        public void OnHandle(NetChannel sender, EntitySyncRequest request)
        {
            var space = SpaceMgr.Instance.GetSpaceById(sender.Player.Character.SpeedId);
            
            space?.EntityUpdate(request.EntitySync.Entity.ToEntity());
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
