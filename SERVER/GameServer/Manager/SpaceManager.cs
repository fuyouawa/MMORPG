using Common.Tool;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 地图管理器
    /// 负责管理游戏的所有地图
    /// 线程安全
    /// </summary>
    public class SpaceManager : Singleton<SpaceManager>
    {
        public readonly int InitSpaceId = 1;

        private Dictionary<int, Space> _spaceDict;

        public SpaceManager()
        {
            _spaceDict = new();
            var noviceVillage = NewSpace(InitSpaceId, "新手村");
        }

        public Space NewSpace(int spaceId, string name)
        {
            var space = new Space(spaceId, name);
            lock (_spaceDict)
            {
                _spaceDict.Add(spaceId, space);
            }
            return space;
        }

        public Space? GetSpaceById(int spaceId)
        {
            lock (_spaceDict)
            {
                return _spaceDict.GetValueOrDefault(spaceId, null);
            }
        }

        public Space? GetSpaceByName(string spaceName)
        {
            lock (_spaceDict)
            {
                foreach (var space in _spaceDict)
                {
                    if (space.Value.Name == spaceName)
                    {
                        return space.Value;
                    }
                }
            }
            return null;
        }

    }
}
