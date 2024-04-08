using Common.Network;
using Common.Proto.Player;
using Common.Proto.Space;
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
        public readonly int InitSpaceId = 1;

        private Dictionary<int, Space> _spaceSet = new();

        public SpaceService()
        {
            var noviceVillage = new Space()
            {
                SpaceId = 1,
                Name = "新手村",
                Description = "新手村",
                Music = 0,
            };
            _spaceSet[noviceVillage.SpaceId] = noviceVillage;
        }

        public Space? GetSpaceById(int spaceId)
        {
            if (!_spaceSet.ContainsKey(spaceId))
            {
                return null;
            }
            return _spaceSet[spaceId];
        }

        public Space? GetSpaceByName(string spaceName)
        {
            foreach (var space in _spaceSet)
            {
                if (space.Value.Name == spaceName)
                {
                    return space.Value;
                }
            }
            return null;
        }

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            var space = GetSpaceById(sender.Player.Character.SpeedId);
            space?.PlayerLeave(sender.Player);
        }

        public void OnHandle(NetChannel sender, EntitySyncRequest request)
        {
            var space = GetSpaceById(sender.Player.Character.SpeedId);
            
            space?.EntityUpdate(request.EntitySync.Entity.ToEntity());
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
