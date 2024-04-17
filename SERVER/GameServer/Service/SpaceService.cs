using Common.Network;
using Common.Proto.Player;
using Common.Proto.Space;
using GameServer.Manager;
using GameServer.Unit;
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
            if (sender.Player == null || sender.Player.Character == null) return;
            var character = sender.Player.Character;
            character.Space?.EntityLeave(character);
            character.Space?.CharacterManager.RemoveCharacter(character);
        }

        public void OnHandle(NetChannel sender, EntitySyncRequest request)
        {
            if (sender.Player == null || sender.Player.Character == null) return;
            sender.Player.Character.Space?.EntityUpdate(request.EntitySync.Entity);
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
