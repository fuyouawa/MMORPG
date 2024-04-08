using Common.Tool;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class SpaceManager : Singleton<SpaceManager>
    {
        public readonly int InitSpaceId = 1;

        private Dictionary<int, Space> _spaceSet = new();

        public SpaceManager()
        {
            var noviceVillage = new Space()
            {
                SpaceId = InitSpaceId,
                Name = "新手村",
                Description = "新手村",
                Music = 0,
            };
            _spaceSet[noviceVillage.SpaceId] = noviceVillage;
        }

        public Space? GetSpaceById(int spaceId)
        {
            return _spaceSet.GetValueOrDefault(spaceId, null);
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

    }
}
