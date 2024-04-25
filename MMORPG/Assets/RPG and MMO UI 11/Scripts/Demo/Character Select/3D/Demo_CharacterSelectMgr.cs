using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace DuloGames.UI
{
    public class Demo_CharacterSelectMgr : MonoBehaviour
    {
        #region Singleton
        private static Demo_CharacterSelectMgr m_Mgr;
        public static Demo_CharacterSelectMgr instance
        {
            get { return m_Mgr; }
        }
        #endregion

        [System.Serializable]
        public class OnCharacterSelectedEvent : UnityEvent<Demo_CharacterInfo> { }

        [System.Serializable]
        public class OnCharacterDeleteEvent : UnityEvent<Demo_CharacterInfo> { }

        #pragma warning disable 0649
        [SerializeField] private int m_IngameSceneId = 0;

        [Header("Camera Properties")]
        [SerializeField] private Camera m_Camera;
        [SerializeField] private float m_CameraSpeed = 10f;
        [SerializeField] private float m_CameraDistance = 10f;
        [SerializeField] private Vector3 m_CameraDirection = Vector3.forward;

        [Header("Character Slots")]
        [SerializeField] private List<Transform> m_Slots;

        [Header("Selected Character Info")]
        [SerializeField] private GameObject m_InfoContainer;
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_LevelText;
        [SerializeField] private Text m_RaceText;
        [SerializeField] private Text m_ClassText;

        [Header("Demo Properties")]
        [SerializeField] private bool m_IsDemo = false;
        [SerializeField] private GameObject m_CharacterPrefab;
        [SerializeField] private int m_AddCharacters = 5;

        [Header("Events")]
        [SerializeField] private OnCharacterSelectedEvent m_OnCharacterSelected = new OnCharacterSelectedEvent();
        [SerializeField] private OnCharacterDeleteEvent m_OnCharacterDelete = new OnCharacterDeleteEvent();
        #pragma warning restore 0649

        private int m_SelectedIndex = -1;
        private Transform m_SelectedTransform;

        protected void Awake()
        {
            // Save a reference to the instance
            m_Mgr = this;

            // Get a camera if not set
            if (this.m_Camera == null) this.m_Camera = Camera.main;

            // Disable the info container
            if (this.m_InfoContainer != null) this.m_InfoContainer.SetActive(false);
        }

        protected void OnDestroy()
        {
            m_Mgr = null;
        }

        protected void Start()
        {
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

                    this.AddCharacter(info, this.m_CharacterPrefab, i);
                }
            }

            // Select the first character
            this.SelectFirstAvailable();
        }
        
        protected void Update()
        {
            if (this.isActiveAndEnabled && this.m_Slots.Count == 0)
                return;

            // Make sure we have a slot transform
            if (this.m_SelectedTransform != null)
            {
                Vector3 targetPos = this.m_SelectedTransform.position + (this.m_CameraDirection * this.m_CameraDistance);
                targetPos.y = this.m_Camera.transform.position.y;
                
                this.m_Camera.transform.position = Vector3.Lerp(this.m_Camera.transform.position, targetPos, Time.deltaTime * this.m_CameraSpeed);
            }
        }

        /// <summary>
        /// Adds a character to the slot at the specified index.
        /// </summary>
        /// <param name="info">The character info.</param>
        /// <param name="modelPrefab">The character model prefab.</param>
        /// <param name="index">Slot index.</param>
        public void AddCharacter(Demo_CharacterInfo info, GameObject modelPrefab, int index)
        {
            if (this.m_Slots.Count == 0 || this.m_Slots.Count < (index + 1))
                return;

            if (modelPrefab == null)
                return;

            // Get the slot
            Transform slotTrans = this.m_Slots[index];

            // Make sure we have a slot transform
            if (slotTrans == null)
                return;

            // Get the character script
            Demo_CharacterSelectSlot csc = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

            // Set the character info
            if (csc != null)
            {
                csc.info = info;
                csc.index = index;
            }

            // Remove any child objects
            foreach (Transform child in slotTrans)
            {
                Destroy(child.gameObject);
            }

            // Add the character model
            GameObject model = Instantiate<GameObject>(modelPrefab);
            model.layer = slotTrans.gameObject.layer;
            model.transform.SetParent(slotTrans, false);
            model.transform.localScale = modelPrefab.transform.localScale;
            model.transform.localPosition = modelPrefab.transform.localPosition;
            model.transform.localRotation = modelPrefab.transform.localRotation;
        }

        /// <summary>
        /// Selects the first available character if any.
        /// </summary>
        public void SelectFirstAvailable()
        {
            if (this.m_Slots.Count == 0)
                return;

            foreach (Transform trans in this.m_Slots)
            {
                if (trans == null)
                    continue;

                // Get the character script
                Demo_CharacterSelectSlot slot = trans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

                // Select the character
                if (slot != null && slot.info != null)
                {
                    this.SelectCharacter(slot);
                    break;
                }
            }
        }

        /// <summary>
        /// Selects the character slot at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void SelectCharacter(int index)
        {
            if (this.m_Slots.Count == 0)
                return;

            // Get the slot
            Transform slotTrans = this.m_Slots[index];

            if (slotTrans == null)
                return;

            // Get the character script
            Demo_CharacterSelectSlot slot = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

            // Select the character
            if (slot != null) this.SelectCharacter(slot);
        }

        /// <summary>
        /// Selects the character slot.
        /// </summary>
        /// <param name="slot">The character slot component.</param>
        public void SelectCharacter(Demo_CharacterSelectSlot slot)
        {
            // Check if already selected
            if (this.m_SelectedIndex == slot.index)
                return;
            
            // Deselect
            if (this.m_SelectedIndex > -1)
            {
                // Get the slot
                Transform selectedSlotTrans = this.m_Slots[this.m_SelectedIndex];

                if (selectedSlotTrans != null)
                {
                    // Get the character script
                    Demo_CharacterSelectSlot selectedSlot = selectedSlotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

                    // Deselect
                    if (selectedSlot != null) selectedSlot.OnDeselected();
                }
            }

            // Set the selected
            this.m_SelectedIndex = slot.index;
            this.m_SelectedTransform = slot.transform;

            if (slot.info != null)
            {
                if (this.m_InfoContainer != null) this.m_InfoContainer.SetActive(true);
                if (this.m_NameText != null) this.m_NameText.text = slot.info.name.ToUpper();
                if (this.m_LevelText != null) this.m_LevelText.text = slot.info.level.ToString();
                if (this.m_RaceText != null) this.m_RaceText.text = slot.info.raceString;
                if (this.m_ClassText != null) this.m_ClassText.text = slot.info.classString;

                // Invoke the on character selected event
                if (this.m_OnCharacterSelected != null)
                    this.m_OnCharacterSelected.Invoke(slot.info);
            }
            else
            {
                if (this.m_InfoContainer != null) this.m_InfoContainer.SetActive(false);
                if (this.m_NameText != null) this.m_NameText.text = "";
                if (this.m_LevelText != null) this.m_LevelText.text = "";
                if (this.m_RaceText != null) this.m_RaceText.text = "";
                if (this.m_ClassText != null) this.m_ClassText.text = "";
            }

            slot.OnSelected();
        }

        /// <summary>
        /// Gets the character in the specified direction (1 or -1).
        /// </summary>
        /// <param name="direction">The direction 1 or -1.</param>
        /// <returns>The character slot.</returns>
        public Demo_CharacterSelectSlot GetCharacterInDirection(float direction)
        {
            if (this.m_Slots.Count == 0)
                return null;

            if (this.m_SelectedTransform == null && this.m_Slots[0] != null)
                return this.m_Slots[0].gameObject.GetComponent<Demo_CharacterSelectSlot>();

            Demo_CharacterSelectSlot closest = null;
            float lastDistance = 0f;

            foreach (Transform trans in this.m_Slots)
            {
                // Skip the selected one
                if (trans.Equals(this.m_SelectedTransform))
                    continue;

                float curDirection = trans.position.x - this.m_SelectedTransform.position.x;

                // Check direction
                if (direction > 0f && curDirection > 0f || direction < 0f && curDirection < 0f)
                {
                    // Get the character component
                    Demo_CharacterSelectSlot slot = trans.GetComponent<Demo_CharacterSelectSlot>();

                    // Make sure we have slot component
                    if (slot == null)
                        continue;

                    // Make sure it's populated
                    if (slot.info == null)
                        continue;

                    // If we have no closest assigned yet
                    if (closest == null)
                    {
                        closest = slot;
                        lastDistance = Vector3.Distance(this.m_SelectedTransform.position, trans.position);
                        continue;
                    }

                    // Comapre distance
                    if (Vector3.Distance(this.m_SelectedTransform.position, trans.position) <= lastDistance)
                    {
                        closest = slot;
                        lastDistance = Vector3.Distance(this.m_SelectedTransform.position, trans.position);
                        continue;
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// Selects the next character slot.
        /// </summary>
        public void SelectNext()
        {
            Demo_CharacterSelectSlot next = this.GetCharacterInDirection(1f);

            if (next != null)
            {
                this.SelectCharacter(next);
            }
        }

        /// <summary>
        /// Selects the previous character slot.
        /// </summary>
        public void SelectPrevious()
        {
            Demo_CharacterSelectSlot prev = this.GetCharacterInDirection(-1f);

            if (prev != null)
            {
                this.SelectCharacter(prev);
            }
        }

        /// <summary>
        /// Remove the character at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveCharacter(int index)
        {
            if (this.m_Slots.Count == 0)
                return;

            // Get the slot
            Transform slotTrans = this.m_Slots[index];

            if (slotTrans == null)
                return;

            // Get the character script
            Demo_CharacterSelectSlot slot = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

            // Invoke the on character delete event
            if (this.m_OnCharacterDelete != null && slot.info != null)
                this.m_OnCharacterDelete.Invoke(slot.info);

            // Unset the character info
            if (slot != null) slot.info = null;

            // Deselect
            if (slot != null) slot.OnDeselected();

            // Remove the child objects
            foreach (Transform child in slotTrans)
            {
                Destroy(child.gameObject);
            }

            // Unset the character info texts
            if (index == this.m_SelectedIndex)
            {
                if (this.m_InfoContainer != null) this.m_InfoContainer.SetActive(false);
                if (this.m_NameText != null) this.m_NameText.text = "";
                if (this.m_LevelText != null) this.m_LevelText.text = "";
                if (this.m_RaceText != null) this.m_RaceText.text = "";
                if (this.m_ClassText != null) this.m_ClassText.text = "";

                this.SelectFirstAvailable();
            }
        }

        /// <summary>
        /// Deletes the selected character.
        /// </summary>
        public void DeleteSelected()
        {
            if (this.m_SelectedIndex > -1)
            {
                this.RemoveCharacter(this.m_SelectedIndex);
            }
        }
        
        public void OnPlayClick()
        {
            UILoadingOverlay loadingOverlay = UILoadingOverlayManager.Instance.Create();

            if (loadingOverlay != null)
                loadingOverlay.LoadScene(this.m_IngameSceneId);
        }
    }
}
