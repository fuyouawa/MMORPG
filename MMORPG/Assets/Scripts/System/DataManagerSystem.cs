using QFramework;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace MMORPG.Game
{
    public interface IDataManagerSystem : ISystem
    {
        public MapDefine GetMapDefine(int mapId);
        public UnitDefine GetUnitDefine(int unitId);
        public ItemDefine GetItemDefine(int itemId);
        public DialogueDefine GetDialogueDefine(int dialogueId);

        public SkillDefine[] GetUnitSkillsDefine(int unitId);
    }


    public class DataManagerSystem : AbstractSystem, IDataManagerSystem
    {
        private Dictionary<int, MapDefine> _mapDict;
        private Dictionary<int, UnitDefine> _unitDict;
        private Dictionary<int, ItemDefine> _itemDict;
        private Dictionary<int, SkillDefine> _skillDict;
        private Dictionary<int, DialogueDefine> _dialogueDict;

        private Dictionary<int, T> Load<T>(string jsonPath)
        {
            var jsonText = Resources.Load(jsonPath) as TextAsset;
            Debug.Assert(jsonText != null);
            var obj = JsonConvert.DeserializeObject<Dictionary<int, T>>(jsonText.text);
            Debug.Assert(obj != null);
            return obj;
        }

        protected override void OnInit()
        {
            _mapDict = Load<MapDefine>("Json/MapDefine");
            _unitDict = Load<UnitDefine>("Json/UnitDefine");
            _itemDict = Load<ItemDefine>("Json/ItemDefine");
            _skillDict = Load<SkillDefine>("Json/SkillDefine");
            _dialogueDict = Load<DialogueDefine>("Json/DialogueDefine");
        }

        protected override void OnDeinit()
        {
            _mapDict.Clear();
            _unitDict.Clear();
            _itemDict.Clear();
            _skillDict.Clear();
            _dialogueDict.Clear();
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

        public DialogueDefine GetDialogueDefine(int dialogueId)
        {
            return _dialogueDict[dialogueId];
        }

        public SkillDefine[] GetUnitSkillsDefine(int unitId)
        {
            return _skillDict
                .Where(x => x.Value.UnitID == unitId)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
