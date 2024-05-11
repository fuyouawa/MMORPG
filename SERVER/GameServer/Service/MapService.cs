using Common.Network;
using Common.Proto.Player;
using Common.Proto.Event.Map;
using GameServer.Manager;
using GameServer.Model;
using GameServer.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Event;
using Serilog;

namespace Service
{
    public class MapService : ServiceBase<MapService>
    {
        public void OnConnect(NetChannel sender)
        {
        }

        public void OnChannelClosed(NetChannel sender)
        {
            var player = sender.User?.Player;
            if (player?.Map == null) return;
            player.Map.EntityLeave(player);
            player.Map.PlayerManager.RemovePlayer(player);
        }

        public void OnHandle(NetChannel sender, EntityTransformSyncRequest request)
        {
            Log.Debug($"{request.EntityId}请求同步: Pos:{request.Transform.Position} | Id:{request.StateId}");
            sender.User?.Player?.Map?.EntityTransformUpdate(request.EntityId, request.Transform, request.StateId, request.Data);
        }

    }
}
