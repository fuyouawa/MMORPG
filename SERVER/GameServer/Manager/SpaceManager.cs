using Common.Tool;
using GameServer.Unit;
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

        private Dictionary<int, Space> _spaceDict = new();

        public SpaceManager()
        {
            var noviceVillage = NewSpace(InitSpaceId, "新手村");
        }

        public Space NewSpace(int spaceId, string name)
        {
            var space = new Space(spaceId, name);
            _spaceDict.Add(spaceId, space);
            return space;
        }

        public Space? GetSpaceById(int spaceId)
        {
            return _spaceDict.GetValueOrDefault(spaceId, null);
        }

        public Space? GetSpaceByName(string spaceName)
        {
            foreach (var space in _spaceDict)
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
