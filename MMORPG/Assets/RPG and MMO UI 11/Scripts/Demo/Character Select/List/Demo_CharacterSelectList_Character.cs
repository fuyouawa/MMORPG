using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DuloGames.UI
{
    public class Demo_CharacterSelectList_Character : MonoBehaviour
    {
        [System.Serializable]
        public class OnCharacterSelectEvent : UnityEvent<Demo_CharacterSelectList_Character> { }

        [System.Serializable]
        public class OnCharacterDeleteEvent : UnityEvent<Demo_CharacterSelectList_Character> { }

        #pragma warning disable 0649
        [SerializeField] private Toggle m_Toggle;
        [SerializeField] private Button m_Delete;

        [Header("Texts")]
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_LevelText;
        [SerializeField] private Text m_RaceText;
        [SerializeField] private Text m_ClassText;
        [SerializeField] private Image m_Avatar;
        #pragma warning restore 0649

        private Demo_CharacterInfo m_CharacterInfo;
        private OnCharacterSelectEvent m_OnCharacterSelected;
        private OnCharacterDeleteEvent m_OnCharacterDelete;

        /// <summary>
        /// Gets the character info.
        /// </summary>
        public Demo_CharacterInfo characterInfo
        {
            get { return this.m_CharacterInfo; }
        }

        /// <summary>
        /// Gets a value indicating wheather this character is selected.
        /// </summary>
        public bool isSelected
        {
            get { return (this.m_Toggle != null ? this.m_Toggle.isOn : false); }
        }
        
        protected void Awake()
        {
            this.m_OnCharacterSelected = new OnCharacterSelectEvent();
            this.m_OnCharacterDelete = new OnCharacterDeleteEvent();
        }

        protected void OnEnable()
        {
            if (this.m_Toggle != null)
            {
                this.m_Toggle.isOn = false;
                this.m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }

            if (this.m_Delete != null)
                this.m_Delete.onClick.AddListener(OnDeleteClick);
        }

        protected void OnDisable()
        {
            if (this.m_Toggle != null)
                this.m_Toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

            if (this.m_Delete != null)
                this.m_Delete.onClick.RemoveListener(OnDeleteClick);
        }

        /// <summary>
        /// Sets the character info.
        /// </summary>
        /// <param name="info">The character info.</param>
        public void SetCharacterInfo(Demo_CharacterInfo info)
        {
            if (info == null)
                return;

            if (this.m_NameText != null) this.m_NameText.text = info.name.ToUpper();
            if (this.m_LevelText != null) this.m_LevelText.text = info.level.ToString();
            if (this.m_RaceText != null) this.m_RaceText.text = info.raceString;
            if (this.m_ClassText != null) this.m_ClassText.text = info.classString;

            // Set the character info
            this.m_CharacterInfo = info;
        }

        /// <summary>
        /// Sets the avatar sprite.
        /// </summary>
        /// <param name="sprite">The avatar sprite.</param>
        public void SetAvatar(Sprite sprite)
        {
            if (this.m_Avatar != null)
                this.m_Avatar.sprite = sprite;
        }

        /// <summary>
        /// Sets the toggle group.
        /// </summary>
        /// <param name="group">The toggle group.</param>
        public void SetToggleGroup(ToggleGroup group)
        {
            if (this.m_Toggle != null)
                this.m_Toggle.group = group;
        }

        /// <summary>
        /// Sets the selected state.
        /// </summary>
        /// <param name="selected">The selected state.</param>
        public void SetSelected(bool selected)
        {
            if (this.m_Toggle != null)
                this.m_Toggle.isOn = selected;
        }

        private void OnToggleValueChanged(bool value)
        {
            if (value && this.m_OnCharacterSelected != null)
                this.m_OnCharacterSelected.Invoke(this);
        }

        private void OnDeleteClick()
        {
            if (this.m_OnCharacterDelete != null)
                this.m_OnCharacterDelete.Invoke(this);
        }

        /// <summary>
        /// Adds on select listener.
        /// </summary>
        /// <param name="call">The on select listener.</param>
        public void AddOnSelectListener(UnityAction<Demo_CharacterSelectList_Character> call)
        {
            this.m_OnCharacterSelected.AddListener(call);
        }

        /// <summary>
        /// Removes on select listener.
        /// </summary>
        /// <param name="call">The on select listener.</param>
        public void RemoveOnSelectListener(UnityAction<Demo_CharacterSelectList_Character> call)
        {
            this.m_OnCharacterSelected.RemoveListener(call);
        }

        /// <summary>
        /// Adds on delete listener.
        /// </summary>
        /// <param name="call">The delete listener.</param>
        public void AddOnDeleteListener(UnityAction<Demo_CharacterSelectList_Character> call)
        {
            this.m_OnCharacterDelete.AddListener(call);
        }

        /// <summary>
        /// Removes on delete listener.
        /// </summary>
        /// <param name="call">The delete listener.</param>
        public void RemoveOnDeleteListener(UnityAction<Demo_CharacterSelectList_Character> call)
        {
            this.m_OnCharacterDelete.RemoveListener(call);
        }
    }
}
