using QFramework;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MMORPG.Game
{
    public interface IDataManagerSystem : ISystem
    {
        public MapDefine GetMapDefine(int mapId);
        public UnitDefine GetUnitDefine(int unitId);
        public ItemDefine GetItemDefine(int itemId);

    }


    public class DataManagerSystem : AbstractSystem, IDataManagerSystem
    {
        private Dictionary<int, MapDefine> _mapDict;
        private Dictionary<int, UnitDefine> _unitDict;
        private Dictionary<int, ItemDefine> _itemDict;
        private Dictionary<int, SkillDefine> _skillDict;

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
            _mapDict = Load<Dictionary<int, MapDefine>>("Json/MapDefine");
            _unitDict = Load<Dictionary<int, UnitDefine>>("Json/UnitDefine");
            _itemDict = Load<Dictionary<int, ItemDefine>>("Json/ItemDefine");
            _skillDict = Load<Dictionary<int, SkillDefine>>("Json/SkillDefine");
        }

        public MapDefine GetMapDefine(int mapId)
        {
            return _mapDict[mapId];
        }

        public UnitDefine GetUnitDefine(int unitId)
        {
            return _unitDict[unitId];
        }

        public SkillDefine GetSkillDefine(int skillId)
        {
            return _skillDict[skillId];
        }

        public ItemDefine GetItemDefine(int itemId)
        {
            return _itemDict[itemId];
        }

    }
}
