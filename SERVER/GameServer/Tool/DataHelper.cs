using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using Newtonsoft.Json;

namespace GameServer.Tool
{
    public static class DataHelper
    {
        /// <summary>
        /// 根据条件从UnitDict中查找Unit，返回Tid，无匹配结果返回0
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int GetUnitId(Predicate<UnitDefine> condition)
        {
            foreach (var unit in DataManager.Instance.UnitDict)
            {
                if (condition(unit.Value))
                {
                    return unit.Key;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据UnitId获取Define
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public static UnitDefine GetUnitDefine(int unitId)
        {
            return DataManager.Instance.UnitDict[unitId];
        }

        public static int GetMapId(Predicate<MapDefine> condition)
        {
            foreach (var map in DataManager.Instance.MapDict)
            {
                if (condition(map.Value))
                {
                    return map.Key;
                }
            }
            return 0;
        }

        public static MapDefine GetMapDefine(int mapId)
        {
            return DataManager.Instance.MapDict[mapId];
        }


        public static T? ParseJson<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

    }
}
