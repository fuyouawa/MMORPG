using System;
using Common.Proto.Character;
using UnityEngine;
using QFramework;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace MMORPG.Game
{
	public partial class UICharacterSelectItem : ViewController
    {
        public Toggle Toggle;
        public event Action<UICharacterSelectItem, bool> OnSelectionChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                TextName.SetText(value);
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                TextLevel.SetText($"等级:{value}");
            }
        }

        public long Exp
        {
            get => _exp;
            set
            {
                _exp = value;
                //TODO _exp
            }
        }

        public int MapId
        {
            get => _mapId;
            set
            {
                _mapId = value;
                //TODO _mapId
            }
        }

        public long CharacterId
        {
            get => _characterId;
            set
            {
                _characterId = value;
                //TODO _characterId
            }
        }

        public int Hp
        {
            get => _hp;
            set
            {
                _hp = value;
                TextHp.SetText($"HP:{value}");
            }
        }

        public int Mp
        {
            get => _mp;
            set
            {
                _mp = value;
                TextMp.SetText($"MP:{value}");
            }
        }

        private string _name;
        private int _level;
        private long _exp;
        private int _hp;
        private int _mp;
        private int _mapId;
        private long _characterId;


        public bool Selected => Toggle != null && Toggle.isOn;

        private void Awake()
        {
            Toggle.onValueChanged.AddListener(OnCreateCharacter);
        }

        public void SetCharacter(NetCharacter character)
        {
            Name = character.Name;
            Level = character.Level;
            Exp = character.Exp;
            Hp = character.Hp;
            Mp = character.Mp;
            MapId = character.MapId;
            CharacterId = character.CharacterId;
        }

        public void OnCreateCharacter(bool toggle)
        {
            OnSelectionChanged?.Invoke(this, toggle);
        }
	}
}
