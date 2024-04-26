using Common.Tool;
using GameServer.Tool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class DataManager : Singleton<DataManager>
    {
        public Dictionary<int, MapDefine> MapDict;
        public Dictionary<int, UnitDefine> UnitDict;
        public Dictionary<int, SpawnDefine> SpawnDict;
        public Dictionary<int, ItemDefine> ItemDict;
        public Dictionary<int, SkillDefine> SkillDict;

        DataManager()
        {
            MapDict = Load<Dictionary<int, MapDefine>>("Data/MapDefine.json");
            UnitDict = Load<Dictionary<int, UnitDefine>>("Data/UnitDefine.json");
            SpawnDict = Load<Dictionary<int, SpawnDefine>>("Data/SpawnDefine.json");
            ItemDict = Load<Dictionary<int, ItemDefine>>("Data/ItemDefine.json");
            SkillDict = Load<Dictionary<int, SkillDefine>>("Data/SkillDefine.json");
        }

        private T Load<T>(string jsonPath)
        {
            var content = JsonHelper.LoadJsonFromFile(jsonPath);
            Debug.Assert(content != null);
            var obj = JsonConvert.DeserializeObject<T>(content);
            Debug.Assert(obj != null);
            return obj;
        }
    }
}
