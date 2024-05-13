using Common.Tool;
using GameServer.Tool;
using GameServer.Model;
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
    public class MapManager : Singleton<MapManager>
    {
        public readonly int InitMapId = 1;

        private Dictionary<int, Map> _mapDict;

        MapManager()
        {
            _mapDict = new();
            var noviceVillage = NewMap(InitMapId, "新手村");
        }

        public Map NewMap(int mapId, string name)
        {
            var map = new Map(mapId, name);
            lock (_mapDict)
            {
                _mapDict.Add(mapId, map);
            }
            return map;
        }

        public Map? GetMapById(int mapId)
        {
            lock (_mapDict)
            {
                _mapDict.TryGetValue(mapId, out var map);
                return map;
            }
        }

        public Map? GetMapByName(string mapName)
        {
            lock (_mapDict)
            {
                foreach (var map in _mapDict)
                {
                    if (map.Value.Name == mapName)
                    {
                        return map.Value;
                    }
                }
            }
            return null;
        }

    }
}
