using Common.Network;
using GameServer.Network;
using Common.Proto.EventLike;

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
            player.Map.PlayerManager.RemovePlayer(player);
            player.Map.EntityLeave(player);
        }

        public void OnHandle(NetChannel sender, EntityTransformSyncRequest request)
        {
            //Log.Debug($"{request.EntityId}请求同步: Pos:{request.Transform.Position} | Id:{request.StateId}");
            sender.User?.Player?.Map?.EntityTransformSync(request.EntityId, request.Transform, request.StateId, request.Data);
        }

    }
}
