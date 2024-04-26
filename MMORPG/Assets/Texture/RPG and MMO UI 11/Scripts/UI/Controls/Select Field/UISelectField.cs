using System;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent, AddComponentMenu("UI/Select Field", 58), RequireComponent(typeof(Image))]
    public class UISelectField : Toggle
    {
        public enum Direction
        {
            Auto,
            Down,
            Up
        }

        public enum VisualState
        {
            Normal,
            Highlighted,
            Pressed,
            Active,
            ActiveHighlighted,
            ActivePressed,
            Disabled
        }

        public enum ListAnimationType
        {
            None,
            Fade,
            Animation
        }

        public enum OptionTextTransitionType
        {
            None,
            CrossFade
        }

        public enum OptionTextEffectType
        {
            None,
            Shadow,
            Outline
        }

        // Currently selected item
        [HideInInspector][SerializeField] private string m_SelectedItem;

        private List<UISelectField_Option> m_OptionObjects = new List<UISelectField_Option>();
        private VisualState m_CurrentVisualState = VisualState.Normal;
        private bool m_PointerWasUsedOnOption = false;

        private GameObject m_ListObject;
        private ScrollRect m_ScrollRect;
        private GameObject m_ListContentObject;
        private CanvasGroup m_ListCanvasGroup;
        private Vector2 m_LastListSize = Vector2.zero;

        private GameObject m_StartSeparatorObject;
        private Navigation.Mode m_LastNavigationMode;
        private GameObject m_LastSelectedGameObject;
        private GameObject m_Blocker;

        [SerializeField] private Direction m_Direction = Direction.Auto;

        /// <summary>
        /// The direction in which the list should pop.
        /// </summary>
        public Direction direction
        {
            get { return this.m_Direction; }
            set { this.m_Direction = value; }
        }

        /// <summary>
        /// Private list of the select options.
        /// </summary>
        [SerializeField, FormerlySerializedAs("options")]
        private List<string> m_Options = new List<string>();

        /// <summary>
        /// Gets the list of options.
        /// </summary>
        public List<string> options
        {
            get { return this.m_Options; }
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

        #pragma warning disable 0649
        // The label text
        [SerializeField] private Text m_LabelText;
        #pragma warning restore 0649

        // Select Field layout properties
        public new ColorBlockExtended colors = ColorBlockExtended.defaultColorBlock;
        public new SpriteStateExtended spriteState;
        public new AnimationTriggersExtended animationTriggers = new AnimationTriggersExtended();

        // List layout properties
        public Sprite listBackgroundSprite;
        public Image.Type listBackgroundSpriteType = Image.Type.Sliced;
        public Color listBackgroundColor = Color.white;
        public RectOffset listMargins;
        public RectOffset listPadding;
        public float listSpacing = 0f;
        public ListAnimationType listAnimationType = ListAnimationType.Fade;
        public float listAnimationDuration = 0.1f;
        public RuntimeAnimatorController listAnimatorController;
        public string listAnimationOpenTrigger = "Open";
        public string listAnimationCloseTrigger = "Close";

        // Scroll rect properties
        public bool allowScrollRect = true;
        public ScrollRect.MovementType scrollMovementType = ScrollRect.MovementType.Clamped;
        public float scrollElasticity = 0.1f;
        public bool scrollInertia = false;
        public float scrollDecelerationRate = 0.135f;
        public float scrollSensitivity = 1f;
        public int scrollMinOptions = 5;
        public float scrollListHeight = 512f;
        public GameObject scrollBarPrefab;
        public float scrollbarSpacing = 34f;

        // Option text layout properties
        public Font optionFont = FontData.defaultFontData.font;
        public int optionFontSize = FontData.defaultFontData.fontSize;
        public FontStyle optionFontStyle = FontData.defaultFontData.fontStyle;
        public Color optionColor = Color.white;
        public OptionTextTransitionType optionTextTransitionType = OptionTextTransitionType.CrossFade;
        public ColorBlockExtended optionTextTransitionColors = ColorBlockExtended.defaultColorBlock;
        public RectOffset optionPadding;

        // Option text effect properties
        public OptionTextEffectType optionTextEffectType = OptionTextEffectType.None;
        public Color optionTextEffectColor = new Color(0f, 0f, 0f, 128f);
        public Vector2 optionTextEffectDistance = new Vector2(1f, -1f);
        public bool optionTextEffectUseGraphicAlpha = true;

        // Option background properties
        public Sprite optionBackgroundSprite;
        public Color optionBackgroundSpriteColor = Color.white;
        public Image.Type optionBackgroundSpriteType = Image.Type.Sliced;
        public Selectable.Transition optionBackgroundTransitionType = Selectable.Transition.None;
        public ColorBlockExtended optionBackgroundTransColors = ColorBlockExtended.defaultColorBlock;
        public SpriteStateExtended optionBackgroundSpriteStates;
        public AnimationTriggersExtended optionBackgroundAnimationTriggers = new AnimationTriggersExtended();
        public RuntimeAnimatorController optionBackgroundAnimatorController;
        public Sprite optionHoverOverlay;
        public Color optionHoverOverlayColor = Color.white;
        public ColorBlock optionHoverOverlayColorBlock = ColorBlock.defaultColorBlock;
        public Sprite optionPressOverlay;
        public Color optionPressOverlayColor = Color.white;
        public ColorBlock optionPressOverlayColorBlock = ColorBlock.defaultColorBlock;

        // List separator properties
        public Sprite listSeparatorSprite;
        public Image.Type listSeparatorType = Image.Type.Simple;
        public Color listSeparatorColor = Color.white;
        public float listSeparatorHeight = 0f;
        public bool startSeparator = false;

        [Serializable]
        public class ChangeEvent : UnityEvent<int, string> { }
        [Serializable]
        public class TransitionEvent : UnityEvent<VisualState, bool> { }

        /// <summary>
        /// Event delegate triggered when the selected option changes.
        /// </summary>
        public ChangeEvent onChange = new ChangeEvent();

        /// <summary>
        /// Event delegate triggered when the select field transition to a visual state.
        /// </summary>
        public TransitionEvent onTransition = new TransitionEvent();

        // Tween controls
        [NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected UISelectField()
        {
            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_FloatTweenRunner.Init(this);
        }

        protected override void Awake()
        {
            base.Awake();

            // Get the background image
            if (this.targetGraphic == null)
                this.targetGraphic = this.GetComponent<Image>();
        }

        protected override void Start()
        {
            base.Start();

            // Prepare the toggle
            this.toggleTransition = ToggleTransition.None;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            // Make sure we always have a font
            if (this.optionFont == null)
                this.optionFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            // Hook the on change event
            this.onValueChanged.AddListener(OnToggleValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Unhook the on change event
            this.onValueChanged.RemoveListener(OnToggleValueChanged);

            // Close if open
            this.isOn = false;

            // Transition to the current state
            this.DoStateTransition(SelectionState.Disabled, true);
        }

        /// <summary>
        /// Open the select field list.
        /// </summary>
        public void Open() { this.isOn = true; }

        /// <summary>
        /// Closes the select field list.
        /// </summary>
        public void Close() { this.isOn = false; }

        /// <summary>
        /// Gets a value indicating whether the list is open.
        /// </summary>
        /// <value><c>true</c> if the list is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return this.isOn;
            }
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
            // If the list is open, use the toggle to select the option
            if (this.IsOpen)
            {
                UISelectField_Option option = this.m_OptionObjects[index];

                if (option != null)
                    option.isOn = true;
            }
            else // otherwise set as selected
            {
                // Set as selected
                this.m_SelectedItem = this.m_Options[index];

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
            {
                this.m_Options.Add(optionValue);
                this.OptionListChanged();
            }
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

            this.OptionListChanged();
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
                this.OptionListChanged();
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
                this.OptionListChanged();
                this.ValidateSelectedOption();
            }
        }

        /// <summary>
        /// Clears the option list.
        /// </summary>
        public void ClearOptions()
        {
            if (this.m_Options == null)
                return;

            this.m_Options.Clear();
            this.OptionListChanged();
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
        /// Raises the option select event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        /// <param name="option">Option.</param>
        public void OnOptionSelect(string option)
        {
            if (string.IsNullOrEmpty(option))
                return;

            // Save the current string to compare later
            string current = this.m_SelectedItem;

            // Save the string
            this.m_SelectedItem = option;

            // Trigger change event
            if (!current.Equals(this.m_SelectedItem))
                this.TriggerChangeEvent();

            // Close the list if it's opened and the pointer was used to select the option
            if (this.IsOpen && this.m_PointerWasUsedOnOption)
            {
                // Reset the value
                this.m_PointerWasUsedOnOption = false;

                // Close the list
                this.Close();

                // Deselect the toggle
                base.OnDeselect(new BaseEventData(EventSystem.current));
            }
        }

        /// <summary>
        /// Raises the option pointer up event (Used to close the list).
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnOptionPointerUp(BaseEventData eventData)
        {
            // Flag to close the list on selection
            this.m_PointerWasUsedOnOption = true;
        }

        /// <summary>
        /// Tiggers the change event.
        /// </summary>
        protected virtual void TriggerChangeEvent()
        {
            // Apply the string to the label componenet
            if (this.m_LabelText != null)
                this.m_LabelText.text = this.m_SelectedItem;

            // Invoke the on change event
            if (onChange != null)
                onChange.Invoke(this.selectedOptionIndex, this.m_SelectedItem);
        }

        /// <summary>
        /// Raises the toggle value changed event (used to toggle the list).
        /// </summary>
        /// <param name="state">If set to <c>true</c> state.</param>
        private void OnToggleValueChanged(bool state)
        {
            if (!Application.isPlaying)
                return;

            // Transition to the current state
            this.DoStateTransition(this.currentSelectionState, false);

            // Open / Close the list
            this.ToggleList(this.isOn);

            // Destroy the block on close
            if (!this.isOn && this.m_Blocker != null)
                Destroy(this.m_Blocker);
        }

        /// <summary>
        /// Raises the deselect event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnDeselect(BaseEventData eventData)
        {
            // Check if the mouse is over our options list
            if (this.m_ListObject != null)
            {
                UISelectField_List list = this.m_ListObject.GetComponent<UISelectField_List>();

                if (list.IsHighlighted(eventData))
                    return;
            }

            // Check if the mouse is over one of our options
            foreach (UISelectField_Option option in this.m_OptionObjects)
            {
                if (option.IsHighlighted(eventData))
                    return;
            }

            // When the select field loses focus
            // close the list by deactivating the toggle
            this.Close();

            // Pass to base
            base.OnDeselect(eventData);
        }

        /// <summary>
        /// Raises the move event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnMove(AxisEventData eventData)
        {
            // Handle navigation for opened list
            if (this.IsOpen)
            {
                int prevIndex = (this.selectedOptionIndex - 1);
                int nextIndex = (this.selectedOptionIndex + 1);

                // Highlight the new option
                switch (eventData.moveDir)
                {
                    case MoveDirection.Up:
                        {
                            if (prevIndex >= 0)
                            {
                                this.SelectOptionByIndex(prevIndex);
                            }
                            break;
                        }
                    case MoveDirection.Down:
                        {
                            if (nextIndex < this.m_Options.Count)
                            {
                                this.SelectOptionByIndex(nextIndex);
                            }
                            break;
                        }
                }

                // Use the event
                eventData.Use();
            }
            else
            {
                // Pass to base
                base.OnMove(eventData);
            }
        }

        /// <summary>
        /// Dos the state transition of the select field.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            if (!this.gameObject.activeInHierarchy)
                return;

            Color color = this.colors.normalColor;
            Sprite newSprite = null;
            string triggername = this.animationTriggers.normalTrigger;

            // Check if this is the disabled state before any others
            if (state == Selectable.SelectionState.Disabled)
            {
                this.m_CurrentVisualState = VisualState.Disabled;
                color = this.colors.disabledColor;
                newSprite = this.spriteState.disabledSprite;
                triggername = this.animationTriggers.disabledTrigger;
            }
            else
            {
                // Prepare the state values
                switch (state)
                {
                    case Selectable.SelectionState.Normal:
                        this.m_CurrentVisualState = (this.isOn) ? VisualState.Active : VisualState.Normal;
                        color = (this.isOn) ? this.colors.activeColor : this.colors.normalColor;
                        newSprite = (this.isOn) ? this.spriteState.activeSprite : null;
                        triggername = (this.isOn) ? this.animationTriggers.activeTrigger : this.animationTriggers.normalTrigger;
                        break;
                    case Selectable.SelectionState.Highlighted:
                    case Selectable.SelectionState.Selected:
                        this.m_CurrentVisualState = (this.isOn) ? VisualState.ActiveHighlighted : VisualState.Highlighted;
                        color = (this.isOn) ? this.colors.activeHighlightedColor : this.colors.highlightedColor;
                        newSprite = (this.isOn) ? this.spriteState.activeHighlightedSprite : this.spriteState.highlightedSprite;
                        triggername = (this.isOn) ? this.animationTriggers.activeHighlightedTrigger : this.animationTriggers.highlightedTrigger;
                        break;
                    case Selectable.SelectionState.Pressed:
                        this.m_CurrentVisualState = (this.isOn) ? VisualState.ActivePressed : VisualState.Pressed;
                        color = (this.isOn) ? this.colors.activePressedColor : this.colors.pressedColor;
                        newSprite = (this.isOn) ? this.spriteState.activePressedSprite : this.spriteState.pressedSprite;
                        triggername = (this.isOn) ? this.animationTriggers.activePressedTrigger : this.animationTriggers.pressedTrigger;
                        break;
                }
            }

            // Do the transition
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    this.StartColorTween(color * this.colors.colorMultiplier, (instant ? 0f : this.colors.fadeDuration));
                    break;
                case Selectable.Transition.SpriteSwap:
                    this.DoSpriteSwap(newSprite);
                    break;
                case Selectable.Transition.Animation:
                    this.TriggerAnimation(triggername);
                    break;
            }

            // Invoke the transition event
            if (this.onTransition != null)
            {
                this.onTransition.Invoke(this.m_CurrentVisualState, instant);
            }
        }

        /// <summary>
        /// Starts the color tween of the select field.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        private void StartColorTween(Color color, float duration)
        {
            if (this.targetGraphic == null)
                return;

            this.targetGraphic.CrossFadeColor(color, duration, true, true);
        }

        /// <summary>
        /// Does the sprite swap of the select field.
        /// </summary>
        /// <param name="newSprite">New sprite.</param>
        private void DoSpriteSwap(Sprite newSprite)
        {
            Image image = this.targetGraphic as Image;

            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        /// <summary>
        /// Triggers the animation of the select field.
        /// </summary>
        /// <param name="trigger">Trigger.</param>
        private void TriggerAnimation(string trigger)
        {
            if (this.animator == null || !this.animator.enabled || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || !this.animator.hasBoundPlayables || string.IsNullOrEmpty(trigger))
                return;

            this.animator.ResetTrigger(this.animationTriggers.normalTrigger);
            this.animator.ResetTrigger(this.animationTriggers.pressedTrigger);
            this.animator.ResetTrigger(this.animationTriggers.highlightedTrigger);
            this.animator.ResetTrigger(this.animationTriggers.activeTrigger);
            this.animator.ResetTrigger(this.animationTriggers.activeHighlightedTrigger);
            this.animator.ResetTrigger(this.animationTriggers.activePressedTrigger);
            this.animator.ResetTrigger(this.animationTriggers.disabledTrigger);
            this.animator.SetTrigger(trigger);
        }

        /// <summary>
        /// Toggles the list.
        /// </summary>
        /// <param name="state">If set to <c>true</c> state.</param>
        protected virtual void ToggleList(bool state)
        {
            if (!this.IsActive())
                return;

            // Check if the list is not yet created
            if (this.m_ListObject == null)
                this.CreateList();

            // Make sure the creating of the list was successful
            if (this.m_ListObject == null)
                return;

            // Make sure we have the canvas group
            if (this.m_ListCanvasGroup != null)
            {
                // Disable or enable list interaction
                this.m_ListCanvasGroup.blocksRaycasts = state;
            }

            // Make sure navigation is enabled in open state
            if (state)
            {
                this.m_LastNavigationMode = this.navigation.mode;
                this.m_LastSelectedGameObject = EventSystem.current.currentSelectedGameObject;

                Navigation newNav = this.navigation;
                newNav.mode = Navigation.Mode.Vertical;
                this.navigation = newNav;

                // Set the select field as selected
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
            else
            {
                Navigation newNav = this.navigation;
                newNav.mode = this.m_LastNavigationMode;
                this.navigation = newNav;

                if (!EventSystem.current.alreadySelecting && this.m_LastSelectedGameObject != null)
                    EventSystem.current.SetSelectedGameObject(this.m_LastSelectedGameObject);
            }

            // Bring to front
            if (state) UIUtility.BringToFront(this.m_ListObject);

            // Start the opening/closing animation
            if (this.listAnimationType == ListAnimationType.None || this.listAnimationType == ListAnimationType.Fade)
            {
                float targetAlpha = (state ? 1f : 0f);

                // Fade In / Out
                this.TweenListAlpha(targetAlpha, ((this.listAnimationType == ListAnimationType.Fade) ? this.listAnimationDuration : 0f), true);
            }
            else if (this.listAnimationType == ListAnimationType.Animation)
            {
                this.TriggerListAnimation(state ? this.listAnimationOpenTrigger : this.listAnimationCloseTrigger);
            }
        }

        /// <summary>
        /// Creates the list and it's options.
        /// </summary>
        protected void CreateList()
        {
            // Reset the last list size
            this.m_LastListSize = Vector2.zero;

            // Clear the option texts list
            this.m_OptionObjects.Clear();

            // Create the list game object with the necessary components
            this.m_ListObject = new GameObject("UISelectField - List", typeof(RectTransform));
            this.m_ListObject.layer = this.gameObject.layer;

            // Change the parent of the list
            this.m_ListObject.transform.SetParent(this.transform, false);

            // Get the select field list component
            UISelectField_List listComp = this.m_ListObject.AddComponent<UISelectField_List>();

            // Make sure it's the top-most element
            UIAlwaysOnTop aot = this.m_ListObject.AddComponent<UIAlwaysOnTop>();
            aot.order = UIAlwaysOnTop.SelectFieldOrder;

            // Get the list canvas group component
            this.m_ListCanvasGroup = this.m_ListObject.AddComponent<CanvasGroup>();

            // Change the anchor and pivot of the list
            RectTransform rect = (this.m_ListObject.transform as RectTransform);
            rect.localScale = new Vector3(1f, 1f, 1f);
            rect.localPosition = Vector3.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0f, 1f);

            // Prepare the position of the list
            rect.anchoredPosition = new Vector3(this.listMargins.left, (this.listMargins.top * -1f), 0f);

            // Prepare the width of the list
            float width = (this.transform as RectTransform).sizeDelta.x;
            if (this.listMargins.left > 0) width -= this.listMargins.left; else width += Math.Abs(this.listMargins.left);
            if (this.listMargins.right > 0) width -= this.listMargins.right; else width += Math.Abs(this.listMargins.right);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            // Hook the Dimensions Change event
            listComp.onDimensionsChange.AddListener(ListDimensionsChanged);

            // Apply the background sprite
            Image image = this.m_ListObject.AddComponent<Image>();
            if (this.listBackgroundSprite != null)
                image.sprite = this.listBackgroundSprite;
            image.type = this.listBackgroundSpriteType;
            image.color = this.listBackgroundColor;

            if (this.allowScrollRect && this.m_Options.Count >= this.scrollMinOptions)
            {
                // Set the list height
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.scrollListHeight);

                // Add scroll rect
                GameObject scrollRectGo = new GameObject("Scroll Rect", typeof(RectTransform));
                scrollRectGo.layer = this.m_ListObject.layer;
                scrollRectGo.transform.SetParent(this.m_ListObject.transform, false);

                RectTransform scrollRectRect = (scrollRectGo.transform as RectTransform);
                scrollRectRect.localScale = new Vector3(1f, 1f, 1f);
                scrollRectRect.localPosition = Vector3.zero;
                scrollRectRect.anchorMin = Vector2.zero;
                scrollRectRect.anchorMax = Vector2.one;
                scrollRectRect.pivot = new Vector2(0f, 1f);
                scrollRectRect.anchoredPosition = Vector2.zero;
                scrollRectRect.offsetMin = new Vector2(this.listPadding.left, this.listPadding.bottom);
                scrollRectRect.offsetMax = new Vector2(this.listPadding.right * -1f, this.listPadding.top * -1f);

                // Add scroll rect component
                this.m_ScrollRect = scrollRectGo.AddComponent<ScrollRect>();
                this.m_ScrollRect.horizontal = false;
                this.m_ScrollRect.movementType = this.scrollMovementType;
                this.m_ScrollRect.elasticity = this.scrollElasticity;
                this.m_ScrollRect.inertia = this.scrollInertia;
                this.m_ScrollRect.decelerationRate = this.scrollDecelerationRate;
                this.m_ScrollRect.scrollSensitivity = this.scrollSensitivity;

                // Create the viewport
                GameObject viewPortGo = new GameObject("View Port", typeof(RectTransform));
                viewPortGo.layer = this.m_ListObject.layer;
                viewPortGo.transform.SetParent(scrollRectGo.transform, false);

                RectTransform viewPortRect = (viewPortGo.transform as RectTransform);
                viewPortRect.localScale = new Vector3(1f, 1f, 1f);
                viewPortRect.localPosition = Vector3.zero;
                viewPortRect.anchorMin = Vector2.zero;
                viewPortRect.anchorMax = Vector2.one;
                viewPortRect.pivot = new Vector2(0f, 1f);
                viewPortRect.anchoredPosition = Vector2.zero;
                viewPortRect.offsetMin = Vector2.zero;
                viewPortRect.offsetMax = Vector2.zero;

                // Add image to the viewport
                Image viewImage = viewPortGo.AddComponent<Image>();
                viewImage.raycastTarget = false;

                // Add mask to the viewport
                Mask viewMask = viewPortGo.AddComponent<Mask>();
                viewMask.showMaskGraphic = false;

                // Create content
                this.m_ListContentObject = new GameObject("Content", typeof(RectTransform));
                this.m_ListContentObject.layer = this.m_ListObject.layer;
                this.m_ListContentObject.transform.SetParent(viewPortRect, false);

                RectTransform contentRect = (this.m_ListContentObject.transform as RectTransform);
                contentRect.localScale = new Vector3(1f, 1f, 1f);
                contentRect.localPosition = Vector3.zero;
                contentRect.anchorMin = new Vector2(0f, 1f);
                contentRect.anchorMax = new Vector2(0f, 1f);
                contentRect.pivot = new Vector2(0f, 1f);
                contentRect.anchoredPosition = Vector2.zero;
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x);

                // Add image to the content for easy scrolling
                Image contentImage = this.m_ListContentObject.AddComponent<Image>();
                contentImage.color = new Color(1f, 1f, 1f, 0f);

                // Get the select field list component
                UISelectField_List contentListComp = this.m_ListContentObject.AddComponent<UISelectField_List>();
                contentListComp.onDimensionsChange.AddListener(ScrollContentDimensionsChanged);

                // Set the content and viewport to the scroll rect
                this.m_ScrollRect.content = contentRect;
                this.m_ScrollRect.viewport = viewPortRect;

                // Prepare the scroll bar
                if (this.scrollBarPrefab != null)
                {
                    GameObject scrollBarGo = Instantiate(this.scrollBarPrefab, scrollRectGo.transform);

                    this.m_ScrollRect.verticalScrollbar = scrollBarGo.GetComponent<Scrollbar>();
                    this.m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                    this.m_ScrollRect.verticalScrollbarSpacing = this.scrollbarSpacing;
                }

                // Prepare the vertical layout group without list padding
                this.m_ListContentObject.AddComponent<VerticalLayoutGroup>();
            }
            else
            {
                // Use the list object as list content object
                this.m_ListContentObject = this.m_ListObject;

                // Prepare the vertical layout group with list padding
                VerticalLayoutGroup layoutGroup = this.m_ListContentObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.padding = this.listPadding;
                layoutGroup.spacing = this.listSpacing;
            }

            // Prepare the content size fitter
            ContentSizeFitter fitter = this.m_ListContentObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Get the list toggle group
            ToggleGroup toggleGroup = this.m_ListObject.AddComponent<ToggleGroup>();

            // Create the options
            for (int i = 0; i < this.m_Options.Count; i++)
            {
                if (i == 0 && this.startSeparator)
                    this.m_StartSeparatorObject = this.CreateSeparator(i - 1);

                // Create the option
                this.CreateOption(i, toggleGroup);

                // Create a separator if this is not the last option
                if (i < (this.m_Options.Count - 1))
                    this.CreateSeparator(i);
            }

            // Prepare the list for the animation
            if (this.listAnimationType == ListAnimationType.None || this.listAnimationType == ListAnimationType.Fade)
            {
                // Starting alpha should be zero
                this.m_ListCanvasGroup.alpha = 0f;
            }
            else if (this.listAnimationType == ListAnimationType.Animation)
            {
                // Attach animator component
                Animator animator = this.m_ListObject.AddComponent<Animator>();

                // Set the animator controller
                animator.runtimeAnimatorController = this.listAnimatorController;

                // Set the animation triggers so we can use them to detect when animations finish
                listComp.SetTriggers(this.listAnimationOpenTrigger, this.listAnimationCloseTrigger);

                // Hook a callback on the finish event
                listComp.onAnimationFinish.AddListener(OnListAnimationFinish);
            }

            // Check if the navigation is disabled
            if (this.navigation.mode == Navigation.Mode.None)
            {
                this.CreateBlocker();
            }

            // If we are using a scroll rect invoke the list dimensions change
            if (this.allowScrollRect && this.m_Options.Count >= this.scrollMinOptions)
            {
                this.ListDimensionsChanged();
            }
        }

        protected virtual void CreateBlocker()
        {
            // Create blocker GameObject.
            GameObject blocker = new GameObject("Blocker");

            // Setup blocker RectTransform to cover entire root canvas area.
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.SetParent(this.transform, false);
            blockerRect.localScale = Vector3.one;
            blockerRect.localPosition = Vector3.zero;

            // Add image since it's needed to block, but make it clear.
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(Close);

            // Make sure it's the top-most element
            UIAlwaysOnTop aot = blocker.AddComponent<UIAlwaysOnTop>();
            aot.order = UIAlwaysOnTop.SelectFieldBlockerOrder;

            UIUtility.BringToFront(blocker);

            blockerRect.anchoredPosition = Vector2.zero;
            blockerRect.pivot = new Vector2(0.5f, 0.5f);
            blockerRect.anchorMin = new Vector2(0f, 0f);
            blockerRect.anchorMax = new Vector2(1f, 1f);
            blockerRect.sizeDelta = new Vector2(0f, 0f);

            this.m_Blocker = blocker;
        }

        /// <summary>
        /// Creates a option.
        /// </summary>
        /// <param name="index">Index.</param>
        protected void CreateOption(int index, ToggleGroup toggleGroup)
        {
            if (this.m_ListContentObject == null)
                return;

            // Create the option game object with it's components
            GameObject optionObject = new GameObject("Option " + index.ToString(), typeof(RectTransform));
            optionObject.layer = this.gameObject.layer;

            // Change parents
            optionObject.transform.SetParent(this.m_ListContentObject.transform, false);
            optionObject.transform.localScale = new Vector3(1f, 1f, 1f);
            optionObject.transform.localPosition = Vector3.zero;

            // Get the option component
            UISelectField_Option optionComp = optionObject.AddComponent<UISelectField_Option>();

            // Prepare the option background
            if (this.optionBackgroundSprite != null)
            {
                Image image = optionObject.AddComponent<Image>();
                image.sprite = this.optionBackgroundSprite;
                image.type = this.optionBackgroundSpriteType;
                image.color = this.optionBackgroundSpriteColor;

                // Add the graphic as the option transition target
                optionComp.targetGraphic = image;
            }

            // Prepare the option for animation
            if (this.optionBackgroundTransitionType == Transition.Animation)
            {
                // Attach animator component
                Animator animator = optionObject.AddComponent<Animator>();

                // Set the animator controller
                animator.runtimeAnimatorController = this.optionBackgroundAnimatorController;
            }

            // Apply the option padding
            VerticalLayoutGroup vlg = optionObject.AddComponent<VerticalLayoutGroup>();
            vlg.padding = this.optionPadding;

            // Create the option text
            GameObject textObject = new GameObject("Label", typeof(RectTransform));

            // Change parents
            textObject.transform.SetParent(optionObject.transform, false);
            textObject.transform.localScale = Vector3.one;
            textObject.transform.localPosition = Vector3.zero;

            // Apply pivot
            (textObject.transform as RectTransform).pivot = new Vector2(0f, 1f);

            // Prepare the text
            Text text = textObject.AddComponent<Text>();
            text.font = this.optionFont;
            text.fontSize = this.optionFontSize;
            text.fontStyle = this.optionFontStyle;
            text.color = this.optionColor;

            if (this.m_Options != null)
                text.text = this.m_Options[index];

            // Apply normal state transition color
            if (this.optionTextTransitionType == OptionTextTransitionType.CrossFade)
                text.canvasRenderer.SetColor(this.optionTextTransitionColors.normalColor);

            // Add and prepare the text effect
            if (this.optionTextEffectType != OptionTextEffectType.None)
            {
                if (this.optionTextEffectType == OptionTextEffectType.Shadow)
                {
                    Shadow effect = textObject.AddComponent<Shadow>();
                    effect.effectColor = this.optionTextEffectColor;
                    effect.effectDistance = this.optionTextEffectDistance;
                    effect.useGraphicAlpha = this.optionTextEffectUseGraphicAlpha;
                }
                else if (this.optionTextEffectType == OptionTextEffectType.Outline)
                {
                    Outline effect = textObject.AddComponent<Outline>();
                    effect.effectColor = this.optionTextEffectColor;
                    effect.effectDistance = this.optionTextEffectDistance;
                    effect.useGraphicAlpha = this.optionTextEffectUseGraphicAlpha;
                }
            }

            // Prepare the option hover overlay
            if (this.optionHoverOverlay != null)
            {
                GameObject hoverOverlayObj = new GameObject("Hover Overlay", typeof(RectTransform));
                hoverOverlayObj.layer = this.gameObject.layer;
                hoverOverlayObj.transform.localScale = Vector3.one;
                hoverOverlayObj.transform.localPosition = Vector3.zero;

                // Add layout element
                LayoutElement hoverLayoutElement = hoverOverlayObj.AddComponent<LayoutElement>();
                hoverLayoutElement.ignoreLayout = true;

                // Change parents
                hoverOverlayObj.transform.SetParent(optionObject.transform, false);
                hoverOverlayObj.transform.localScale = new Vector3(1f, 1f, 1f);

                // Add image
                Image hoImage = hoverOverlayObj.AddComponent<Image>();
                hoImage.sprite = this.optionHoverOverlay;
                hoImage.color = this.optionHoverOverlayColor;
                hoImage.type = Image.Type.Sliced;

                // Apply pivot
                (hoverOverlayObj.transform as RectTransform).pivot = new Vector2(0f, 1f);

                // Apply anchors
                (hoverOverlayObj.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                (hoverOverlayObj.transform as RectTransform).anchorMax = new Vector2(1f, 1f);

                // Apply offsets
                (hoverOverlayObj.transform as RectTransform).offsetMin = new Vector2(0f, 0f);
                (hoverOverlayObj.transform as RectTransform).offsetMax = new Vector2(0f, 0f);

                // Add the highlight transition component
                UISelectField_OptionOverlay hoht = optionObject.AddComponent<UISelectField_OptionOverlay>();
                hoht.targetGraphic = hoImage;
                hoht.transition = UISelectField_OptionOverlay.Transition.ColorTint;
                hoht.colorBlock = this.optionHoverOverlayColorBlock;
                hoht.InternalEvaluateAndTransitionToNormalState(true);
            }

            // Prepare the option press overlay
            if (this.optionPressOverlay != null)
            {
                GameObject pressOverlayObj = new GameObject("Press Overlay", typeof(RectTransform));
                pressOverlayObj.layer = this.gameObject.layer;
                pressOverlayObj.transform.localScale = Vector3.one;
                pressOverlayObj.transform.localPosition = Vector3.zero;

                // Add layout element
                LayoutElement pressLayoutElement = pressOverlayObj.AddComponent<LayoutElement>();
                pressLayoutElement.ignoreLayout = true;

                // Change parents
                pressOverlayObj.transform.SetParent(optionObject.transform, false);
                pressOverlayObj.transform.localScale = new Vector3(1f, 1f, 1f);

                // Add image
                Image poImage = pressOverlayObj.AddComponent<Image>();
                poImage.sprite = this.optionPressOverlay;
                poImage.color = this.optionPressOverlayColor;
                poImage.type = Image.Type.Sliced;

                // Apply pivot
                (pressOverlayObj.transform as RectTransform).pivot = new Vector2(0f, 1f);

                // Apply anchors
                (pressOverlayObj.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                (pressOverlayObj.transform as RectTransform).anchorMax = new Vector2(1f, 1f);

                // Apply offsets
                (pressOverlayObj.transform as RectTransform).offsetMin = new Vector2(0f, 0f);
                (pressOverlayObj.transform as RectTransform).offsetMax = new Vector2(0f, 0f);

                // Add the highlight transition component
                UISelectField_OptionOverlay poht = optionObject.AddComponent<UISelectField_OptionOverlay>();
                poht.targetGraphic = poImage;
                poht.transition = UISelectField_OptionOverlay.Transition.ColorTint;
                poht.colorBlock = this.optionPressOverlayColorBlock;
                poht.InternalEvaluateAndTransitionToNormalState(true);
            }

            // Initialize the option component
            optionComp.Initialize(this, text);

            // Set active if it's the selected one
            if (index == this.selectedOptionIndex)
                optionComp.isOn = true;

            // Register to the toggle group
            if (toggleGroup != null)
                optionComp.group = toggleGroup;

            // Hook some events
            optionComp.onSelectOption.AddListener(OnOptionSelect);
            optionComp.onPointerUp.AddListener(OnOptionPointerUp);

            // Add it to the list
            if (this.m_OptionObjects != null)
                this.m_OptionObjects.Add(optionComp);
        }

        /// <summary>
        /// Creates a separator.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>The separator game object.</returns>
        protected GameObject CreateSeparator(int index)
        {
            if (this.m_ListContentObject == null || this.listSeparatorSprite == null)
                return null;

            GameObject separatorObject = new GameObject("Separator " + index.ToString(), typeof(RectTransform));

            // Change parent
            separatorObject.transform.SetParent(this.m_ListContentObject.transform, false);
            separatorObject.transform.localScale = Vector3.one;
            separatorObject.transform.localPosition = Vector3.zero;

            // Apply the sprite
            Image image = separatorObject.AddComponent<Image>();
            image.sprite = this.listSeparatorSprite;
            image.type = this.listSeparatorType;
            image.color = this.listSeparatorColor;

            // Apply preferred height
            LayoutElement le = separatorObject.AddComponent<LayoutElement>();
            le.preferredHeight = (this.listSeparatorHeight > 0f) ? this.listSeparatorHeight : this.listSeparatorSprite.rect.height;

            return separatorObject;
        }

        /// <summary>
        /// Does a list cleanup (Destroys the list and clears the option objects list).
        /// </summary>
        protected virtual void ListCleanup()
        {
            if (this.m_ListObject != null)
                Destroy(this.m_ListObject);

            this.m_OptionObjects.Clear();
        }

        /// <summary>
        /// Positions the list for the given direction (Auto is not handled in this method).
        /// </summary>
        /// <param name="direction">Direction.</param>
        public virtual void PositionListForDirection(Direction direction)
        {
            // Make sure the creating of the list was successful
            if (this.m_ListObject == null)
                return;

            // Get the list rect transforms
            RectTransform listRect = (this.m_ListObject.transform as RectTransform);

            // Determine the direction of the pop
            if (direction == Direction.Auto)
            {
                // Get the list world corners
                Vector3[] listWorldCorner = new Vector3[4];
                listRect.GetWorldCorners(listWorldCorner);

                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, listWorldCorner[0]);

                // Check if the list is going outside to the bottom
                if (screenPoint.y < 0f)
                {
                    direction = Direction.Up;
                }
                else
                {
                    direction = Direction.Down;
                }
            }

            // Handle up or down direction
            if (direction == Direction.Down)
            {
                listRect.SetParent(this.transform, true);
                listRect.pivot = new Vector2(0f, 1f);
                listRect.anchorMin = new Vector2(0f, 0f);
                listRect.anchorMax = new Vector2(0f, 0f);
                listRect.anchoredPosition = new Vector2(listRect.anchoredPosition.x, this.listMargins.top * -1f);

                UIUtility.BringToFront(listRect.gameObject);
            }
            else
            {
                listRect.SetParent(this.transform, true);
                listRect.pivot = new Vector2(0f, 0f);
                listRect.anchorMin = new Vector2(0f, 1f);
                listRect.anchorMax = new Vector2(0f, 1f);
                listRect.anchoredPosition = new Vector2(listRect.anchoredPosition.x, this.listMargins.bottom);

                if (this.m_StartSeparatorObject != null)
                    this.m_StartSeparatorObject.transform.SetAsLastSibling();

                UIUtility.BringToFront(listRect.gameObject);
            }
        }

        /// <summary>
        /// Event invoked when the list dimensions change.
        /// </summary>
        protected virtual void ListDimensionsChanged()
        {
            if (!this.IsActive() || this.m_ListObject == null)
                return;

            // Check if the list size has changed
            if (this.m_LastListSize.Equals((this.m_ListObject.transform as RectTransform).sizeDelta))
                return;

            // Update the last list size
            this.m_LastListSize = (this.m_ListObject.transform as RectTransform).sizeDelta;

            // Update the list direction
            this.PositionListForDirection(this.m_Direction);
        }

        /// <summary>
        /// Event invoked when the scroll rect content dimensions change.
        /// </summary>
        protected virtual void ScrollContentDimensionsChanged()
        {
            if (!this.IsActive() || this.m_ScrollRect == null)
                return;

            float contentHeight = (this.m_ScrollRect.content as RectTransform).sizeDelta.y;
            float optionHeight = contentHeight / (float)this.m_Options.Count;
            float optionPosition = optionHeight * (float)this.selectedOptionIndex;

            this.m_ScrollRect.content.anchoredPosition = new Vector2(this.m_ScrollRect.content.anchoredPosition.x, optionPosition);
        }

        /// <summary>
        /// Event invoked when the option list changes.
        /// </summary>
        protected virtual void OptionListChanged() { }

        /// <summary>
        /// Tweens the list alpha.
        /// </summary>
        /// <param name="targetAlpha">Target alpha.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
        private void TweenListAlpha(float targetAlpha, float duration, bool ignoreTimeScale)
        {
            if (this.m_ListCanvasGroup == null)
                return;

            float currentAlpha = this.m_ListCanvasGroup.alpha;

            if (currentAlpha.Equals(targetAlpha))
                return;

            var floatTween = new FloatTween { duration = duration, startFloat = currentAlpha, targetFloat = targetAlpha };
            floatTween.AddOnChangedCallback(SetListAlpha);
            floatTween.AddOnFinishCallback(OnListTweenFinished);
            floatTween.ignoreTimeScale = ignoreTimeScale;
            this.m_FloatTweenRunner.StartTween(floatTween);
        }

        /// <summary>
        /// Sets the list alpha.
        /// </summary>
        /// <param name="alpha">Alpha.</param>
        private void SetListAlpha(float alpha)
        {
            if (this.m_ListCanvasGroup == null)
                return;

            // Set the alpha
            this.m_ListCanvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Triggers the list animation.
        /// </summary>
        /// <param name="trigger">Trigger.</param>
        private void TriggerListAnimation(string trigger)
        {
            if (this.m_ListObject == null || string.IsNullOrEmpty(trigger))
                return;

            Animator animator = this.m_ListObject.GetComponent<Animator>();

            if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables)
                return;

            animator.ResetTrigger(this.listAnimationOpenTrigger);
            animator.ResetTrigger(this.listAnimationCloseTrigger);
            animator.SetTrigger(trigger);
        }

        /// <summary>
        /// Raises the list tween finished event.
        /// </summary>
        protected virtual void OnListTweenFinished()
        {
            // If the list is closed do a cleanup
            if (!this.IsOpen)
                this.ListCleanup();
        }

        /// <summary>
        /// Raises the list animation finish event.
        /// </summary>
        /// <param name="state">State.</param>
        protected virtual void OnListAnimationFinish(UISelectField_List.State state)
        {
            // If the list is closed do a cleanup
            if (state == UISelectField_List.State.Closed && !this.IsOpen)
                this.ListCleanup();
        }
    }
}
