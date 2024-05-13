using QFramework;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Common.Proto.Event.Map;
using Common.Proto.Event;
using MMORPG.System;
using Unity.VisualScripting;
using Google.Protobuf.WellKnownTypes;

namespace MMORPG.Game
{
    public interface IDataManagerSystem : ISystem
    {
        public MapDefine GetMapDefine(int mapId);
        public UnitDefine GetUnitDefine(int unitId);


    }


    public class DataManagerSystem : AbstractSystem, IDataManagerSystem
    {
        private Dictionary<int, MapDefine> _mapDict;
        private Dictionary<int, UnitDefine> _unitDict;
        public Dictionary<int, SpawnDefine> SpawnDict;
        public Dictionary<int, ItemDefine> ItemDict;
        public Dictionary<int, SkillDefine> SkillDict;

        private T Load<T>(string jsonPath)
        {
            TextAsset jsonText = Resources.Load(jsonPath) as TextAsset;
            Debug.Assert(jsonText != null);
            var obj = JsonConvert.DeserializeObject<T>(jsonText.text);
            Debug.Assert(obj != null);
            return obj;
        }

        protected override void OnInit()
        {
            _mapDict = Load<Dictionary<int, MapDefine>>("Json/MapDefine.json");
            _unitDict = Load<Dictionary<int, UnitDefine>>("Json/UnitDefine.json");
            SpawnDict = Load<Dictionary<int, SpawnDefine>>("Json/SpawnDefine.json");
            ItemDict = Load<Dictionary<int, ItemDefine>>("Json/ItemDefine.json");
            //SkillDict = Load<Dictionary<int, SkillDefine>>("Json/SkillDefine.json");
        }

        public MapDefine GetMapDefine(int mapId)
        {
            lock (_mapDict)
            {
                return _mapDict[mapId];
            }
        }

        public UnitDefine GetUnitDefine(int unitId)
        {
            lock (_unitDict)
            {
                return _unitDict[unitId];
            }
        }
    }
}
