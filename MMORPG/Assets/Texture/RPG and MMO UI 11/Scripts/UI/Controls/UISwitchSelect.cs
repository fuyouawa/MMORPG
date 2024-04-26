using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DuloGames.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent, AddComponentMenu("UI/Switch Select Field", 58)]
    public class UISwitchSelect : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Text m_Text;
        [SerializeField] private Button m_PrevButton;
        [SerializeField] private Button m_NextButton;

        // Currently selected item
		[HideInInspector][SerializeField] private string m_SelectedItem;
        #pragma warning restore 0649

        /// <summary>
		/// New line-delimited list of items.
		/// </summary>
		[SerializeField] private List<string> m_Options = new List<string>();

        /// <summary>
		/// The list of options.
		/// </summary>
        public List<string> options
        {
            get
            {
                return this.m_Options;
            }
        }

        /// <summary>
        /// Currently selected option.
        /// </summary>
        public string value
        {
            get
            {
                return this.m_SelectedItem;
            }
            set
            {
                this.SelectOption(value);
            }
        }

        /// <summary>
        /// Gets the index of the selected option.
        /// </summary>
        /// <value>The index of the selected option.</value>
        public int selectedOptionIndex
        {
            get
            {
                return this.GetOptionIndex(this.m_SelectedItem);
            }
        }

        [System.Serializable] public class ChangeEvent : UnityEvent<int, string> { }

        /// <summary>
        /// Event delegates triggered when the selected option changes.
        /// </summary>
        public ChangeEvent onChange = new ChangeEvent();

        protected void OnEnable()
        {
            // Hook the buttons click events
            if (this.m_PrevButton != null)
                this.m_PrevButton.onClick.AddListener(OnPrevButtonClick);
            if (this.m_NextButton != null)
                this.m_NextButton.onClick.AddListener(OnNextButtonClick);
        }

        protected void OnDisable()
        {
            // Unhook the buttons click events
            if (this.m_PrevButton != null)
                this.m_PrevButton.onClick.RemoveListener(OnPrevButtonClick);
            if (this.m_NextButton != null)
                this.m_NextButton.onClick.RemoveListener(OnNextButtonClick);
        }

        protected void OnPrevButtonClick()
        {
            int prevIndex = this.selectedOptionIndex - 1;

            // Check if the option index is valid
            if (prevIndex < 0)
                prevIndex = this.m_Options.Count - 1;

            if (prevIndex >= this.m_Options.Count)
                prevIndex = 0;

            // Select the new option
            this.SelectOptionByIndex(prevIndex);
        }

        protected void OnNextButtonClick()
        {
            int nextIndex = this.selectedOptionIndex + 1;

            // Check if the option index is valid
            if (nextIndex < 0)
                nextIndex = this.m_Options.Count - 1;

            if (nextIndex >= this.m_Options.Count)
                nextIndex = 0;

            // Select the new option
            this.SelectOptionByIndex(nextIndex);
        }

        /// <summary>
		/// Gets the index of the given option.
		/// </summary>
		/// <returns>The option index. (-1 if the option was not found)</returns>
		/// <param name="optionValue">Option value.</param>
		public int GetOptionIndex(string optionValue)
        {
            // Find the option index in the options list
            if (this.m_Options != null && this.m_Options.Count > 0 && !string.IsNullOrEmpty(optionValue))
                for (int i = 0; i < this.m_Options.Count; i++)
                    if (optionValue.Equals(this.m_Options[i], System.StringComparison.OrdinalIgnoreCase))
                        return i;

            // Default
            return -1;
        }

        /// <summary>
		/// Selects the option by index.
		/// </summary>
		/// <param name="optionIndex">Option index.</param>
		public void SelectOptionByIndex(int index)
        {
            // Check if the option index is valid
            if (index < 0 || index >= this.m_Options.Count)
                return;

            string newOption = this.m_Options[index];

            // If the options changes
            if (!newOption.Equals(this.m_SelectedItem))
            {
                // Set as selected
                this.m_SelectedItem = newOption;

                // Trigger change
                this.TriggerChangeEvent();
            }
        }

        /// <summary>
        /// Selects the option by value.
        /// </summary>
        /// <param name="optionValue">The option value.</param>
        public void SelectOption(string optionValue)
        {
            if (string.IsNullOrEmpty(optionValue))
                return;

            // Get the option
            int index = this.GetOptionIndex(optionValue);

            // Check if the option index is valid
            if (index < 0 || index >= this.m_Options.Count)
                return;

            // Select the option
            this.SelectOptionByIndex(index);
        }

        /// <summary>
        /// Adds an option.
        /// </summary>
        /// <param name="optionValue">Option value.</param>
        public void AddOption(string optionValue)
        {
            if (this.m_Options != null)
                this.m_Options.Add(optionValue);
        }

        /// <summary>
        /// Adds an option at given index.
        /// </summary>
        /// <param name="optionValue">Option value.</param>
        /// <param name="index">Index.</param>
        public void AddOptionAtIndex(string optionValue, int index)
        {
            if (this.m_Options == null)
                return;

            // Check if the index is outside the list
            if (index >= this.m_Options.Count)
            {
                this.m_Options.Add(optionValue);
            }
            else
            {
                this.m_Options.Insert(index, optionValue);
            }
        }

        /// <summary>
        /// Removes the option.
        /// </summary>
        /// <param name="optionValue">Option value.</param>
        public void RemoveOption(string optionValue)
        {
            if (this.m_Options == null)
                return;

            // Remove the option if exists
            if (this.m_Options.Contains(optionValue))
            {
                this.m_Options.Remove(optionValue);
                this.ValidateSelectedOption();
            }
        }

        /// <summary>
        /// Removes the option at the given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void RemoveOptionAtIndex(int index)
        {
            if (this.m_Options == null)
                return;

            // Remove the option if the index is valid
            if (index >= 0 && index < this.m_Options.Count)
            {
                this.m_Options.RemoveAt(index);
                this.ValidateSelectedOption();
            }
        }

        /// <summary>
        /// Validates the selected option and makes corrections if it's missing.
        /// </summary>
        public void ValidateSelectedOption()
        {
            if (this.m_Options == null)
                return;

            // Fix the selected option if it no longer exists
            if (!this.m_Options.Contains(this.m_SelectedItem))
            {
                // Select the first option
                this.SelectOptionByIndex(0);
            }
        }
        
        /// <summary>
		/// Tiggers the change event.
		/// </summary>
		protected virtual void TriggerChangeEvent()
        {
            // Apply the string to the label componenet
            if (this.m_Text != null)
                this.m_Text.text = this.m_SelectedItem;

            // Invoke the on change event
            if (onChange != null)
                onChange.Invoke(this.selectedOptionIndex, this.m_SelectedItem);
        }
    }
}
