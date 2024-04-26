using Common.Network;
using Common.Proto.Player;
using Common.Proto.Event.Map;
using GameServer.Manager;
using GameServer.Unit;
using GameServer.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Event;

namespace Service
{
    public class MapService : ServiceBase<MapService>
    {
        public void OnConnect(NetChannel sender)
        {
        }

        public void OnChannelClosed(NetChannel sender)
        {
            var character = sender.Player?.Character;
            if (character?.Map == null) return;
            character.Map.EntityLeave(character);
            character.Map.PlayerManager.RemoveCharacter(character);
        }

        public void OnHandle(NetChannel sender, EntityTransformSyncRequest request)
        {
            sender.Player?.Character?.Map?.EntityTransformUpdate(request.EntityId, request.Transform);
        }

    }
}
