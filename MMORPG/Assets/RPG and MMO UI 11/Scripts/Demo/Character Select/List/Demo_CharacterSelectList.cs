using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DuloGames.UI
{
    [RequireComponent(typeof(ToggleGroup))]
    public class Demo_CharacterSelectList : MonoBehaviour
    {
        [System.Serializable]
        public class OnCharacterSelectedEvent : UnityEvent<Demo_CharacterInfo> { }

        [System.Serializable]
        public class OnCharacterDeleteEvent : UnityEvent<Demo_CharacterInfo> { }

        #pragma warning disable 0649
        [SerializeField] private GameObject m_CharacterPrefab;
        [SerializeField] private Transform m_CharactersContainer;
        #pragma warning restore 0649

        [Header("Demo Properties")]
        [SerializeField] private bool m_IsDemo = false;
        [SerializeField] private int m_AddCharacters = 5;

        [Header("Events")]
        [SerializeField] private OnCharacterSelectedEvent m_OnCharacterSelected = new OnCharacterSelectedEvent();
        [SerializeField] private OnCharacterDeleteEvent m_OnCharacterDelete = new OnCharacterDeleteEvent();

        private ToggleGroup m_ToggleGroup;
        private Demo_CharacterSelectList_Character m_DeletingCharacter;

        protected void Awake()
        {
            this.m_ToggleGroup = this.gameObject.GetComponent<ToggleGroup>();
        }

        protected void Start()
        {
            // Clear the characters container
            if (this.m_CharactersContainer != null)
            {
                foreach (Transform t in this.m_CharactersContainer)
                    Destroy(t.gameObject);
            }

            // Add characters for the demo
            if (this.m_IsDemo && this.m_CharacterPrefab)
            {
                for (int i = 0; i < this.m_AddCharacters; i++)
                {
                    string[] names = new string[10] { "Annika", "Evita", "Herb", "Thad", "Myesha", "Lucile", "Sharice", "Tatiana", "Isis", "Allen" };
                    string[] races = new string[5] { "Human", "Elf", "Orc", "Undead", "Programmer" };
                    string[] classes = new string[5] { "Warrior", "Mage", "Hunter", "Priest", "Designer" };

                    Demo_CharacterInfo info = new Demo_CharacterInfo();
                    info.name = names[Random.Range(0, 10)];
                    info.raceString = races[Random.Range(0, 5)];
                    info.classString = classes[Random.Range(0, 5)];
                    info.level = (int)Random.Range(1, 61);

                    this.AddCharacter(info, (i == 0));
                }
            }
        }
        
        /// <summary>
        /// Adds a character to the character list.
        /// </summary>
        /// <param name="info">The character info.</param>
        /// <param name="selected">In the character should be selected.</param>
        public void AddCharacter(Demo_CharacterInfo info, bool selected)
        {
            if (this.m_CharacterPrefab == null || this.m_CharactersContainer == null)
                return;
            
            // Add the character
            GameObject model = Instantiate<GameObject>(this.m_CharacterPrefab);
            model.layer = this.m_CharactersContainer.gameObject.layer;
            model.transform.SetParent(this.m_CharactersContainer, false);
            model.transform.localScale = this.m_CharacterPrefab.transform.localScale;
            model.transform.localPosition = this.m_CharacterPrefab.transform.localPosition;
            model.transform.localRotation = this.m_CharacterPrefab.transform.localRotation;
            
            // Get the character component
            Demo_CharacterSelectList_Character character = model.GetComponent<Demo_CharacterSelectList_Character>();
            
            if (character != null)
            {
                // Set the info
                character.SetCharacterInfo(info);

                // Set the toggle group
                character.SetToggleGroup(this.m_ToggleGroup);

                // Set the selected state
                character.SetSelected(selected);

                // Add on select listener
                character.AddOnSelectListener(OnCharacterSelected);

                // Add on delete listener
                character.AddOnDeleteListener(OnCharacterDeleteRequested);
            }
        }

        /// <summary>
        /// Event invoked when when a character in the list is selected.
        /// </summary>
        /// <param name="character">The character.</param>
        private void OnCharacterSelected(Demo_CharacterSelectList_Character character)
        {
            if (this.m_OnCharacterSelected != null)
                this.m_OnCharacterSelected.Invoke(character.characterInfo);
        }

        /// <summary>
        /// Event invoked when when a character delete button is pressed.
        /// </summary>
        /// <param name="character">The character.</param>
        private void OnCharacterDeleteRequested(Demo_CharacterSelectList_Character character)
        {
            // Save the deleting character reference
            this.m_DeletingCharacter = character;

            // Create a modal box
            UIModalBox box = UIModalBoxManager.Instance.Create(this.gameObject);
            if (box != null)
            {
                box.SetText1("Do you really want to delete this character?");
                box.SetText2("You wont be able to reverse this operation and yourcharcater will be permamently removed.");
                box.SetConfirmButtonText("DELETE");
                //box.SetCancelButtonText(string.Empty);
                box.onConfirm.AddListener(OnCharacterDeleteConfirm);
                box.onCancel.AddListener(OnCharacterDeleteCancel);
                box.Show();
            }
        }

        /// <summary>
        /// Event invoked when a character deletion is confirmed.
        /// </summary>
        private void OnCharacterDeleteConfirm()
        {
            if (this.m_DeletingCharacter == null)
                return;

            // If this character is selected
            if (this.m_DeletingCharacter.isSelected && this.m_CharactersContainer != null)
            {
                // Find and select new character
                foreach (Transform t in this.m_CharactersContainer)
                {
                    Demo_CharacterSelectList_Character character = t.gameObject.GetComponent<Demo_CharacterSelectList_Character>();

                    // If the character is not the one we are deleting
                    if (!character.Equals(this.m_DeletingCharacter))
                    {
                        character.SetSelected(true);
                        break;
                    }
                }
            }

            // Invoke the on delete event
            if (this.m_OnCharacterDelete != null)
                this.m_OnCharacterDelete.Invoke(this.m_DeletingCharacter.characterInfo);

            // Delete the character game object
            Destroy(this.m_DeletingCharacter.gameObject);
        }

        /// <summary>
        /// Event invoked when a character deletion is canceled.
        /// </summary>
        private void OnCharacterDeleteCancel()
        {
            this.m_DeletingCharacter = null;
        }
    }
}
