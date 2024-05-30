using System;
using Common.Proto.Character;
using UnityEngine;
using QFramework;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace MMORPG.Game
{
	public partial class CharacterSelectItem : ViewController
    {
        public Toggle Toggle { get; private set; }
        public event Action<CharacterSelectItem> OnSelected;

        public bool Selected => Toggle != null && Toggle.isOn;

        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
            Toggle.onValueChanged.AddListener(OnCreateCharacter);
        }

        public void SetCharacter(NetCharacter character)
        {
            TextName.SetText(character.Name);
            TextLevel.SetText($"等级:{character.Level}");
        }

        public void OnCreateCharacter(bool toggle)
        {
            if (toggle)
            {
                OnSelected?.Invoke(this);
            }
        }
	}
}
