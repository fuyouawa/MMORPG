using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;
using System.Collections.Generic;

namespace DuloGames.UI
{
    [ExecuteInEditMode, AddComponentMenu("UI/Effect Transition")]
    public class UIEffectTransition : MonoBehaviour, IEventSystemHandler, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum VisualState
        {
            Normal,
            Highlighted,
            Selected,
            Pressed,
            Active
        }

        #pragma warning disable 0649
        [SerializeField, Tooltip("Graphic that will have the selected transtion applied.")]
        private BaseMeshEffect m_TargetEffect;

        [SerializeField] private Color m_NormalColor = ColorBlock.defaultColorBlock.normalColor;
		[SerializeField] private Color m_HighlightedColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private Color m_SelectedColor = ColorBlock.defaultColorBlock.highlightedColor;
        [SerializeField] private Color m_PressedColor = ColorBlock.defaultColorBlock.pressedColor;
		[SerializeField] private float m_Duration = 0.1f;
		
        [SerializeField] private bool m_UseToggle = false;
        [SerializeField] private Toggle m_TargetToggle;
        [SerializeField] private Color m_ActiveColor = ColorBlock.defaultColorBlock.highlightedColor;
        #pragma warning restore 0649

        private bool m_Highlighted = false;
        private bool m_Selected = false;
        private bool m_Pressed = false;
        private bool m_Active = false;

        private Selectable m_Selectable;
        private bool m_GroupsAllowInteraction = true;

        // Tween controls
        [System.NonSerialized]
        private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected UIEffectTransition()
        {
            if (this.m_ColorTweenRunner == null)
                this.m_ColorTweenRunner = new TweenRunner<ColorTween>();

            this.m_ColorTweenRunner.Init(this);
        }

        protected void Awake()
        {
            if (this.m_UseToggle)
            {
                if (this.m_TargetToggle == null)
                    this.m_TargetToggle = this.gameObject.GetComponent<Toggle>();

                if (this.m_TargetToggle != null)
                    this.m_Active = this.m_TargetToggle.isOn;
            }

            this.m_Selectable = this.gameObject.GetComponent<Selectable>();
        }

        protected void OnEnable()
        {
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.AddListener(OnToggleValueChange);

            this.InternalEvaluateAndTransitionToNormalState(true);
        }

        protected void OnDisable()
        {
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);

            this.InstantClearState();
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            this.m_Duration = Mathf.Max(this.m_Duration, 0f);

            if (this.isActiveAndEnabled)
                this.InternalEvaluateAndTransitionToNormalState(true);
        }
#endif

        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        protected void OnCanvasGroupChanged()
        {
            // Figure out if parent groups allow interaction
            // If no interaction is alowed... then we need
            // to not do that :)
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction
                    // we need to break
                    if (!m_CanvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break
                    // as we should not consider parents
                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            if (groupAllowInteraction != this.m_GroupsAllowInteraction)
            {
                this.m_GroupsAllowInteraction = groupAllowInteraction;
                this.InternalEvaluateAndTransitionToNormalState(true);
            }
        }

        public virtual bool IsInteractable()
        {
            if (this.m_Selectable != null)
                return this.m_Selectable.IsInteractable() && this.m_GroupsAllowInteraction;

            return this.m_GroupsAllowInteraction;
        }

        protected void OnToggleValueChange(bool value)
        {
            if (!this.m_UseToggle || this.m_TargetToggle == null)
                return;

            this.m_Active = this.m_TargetToggle.isOn;
            
            if (!this.m_TargetToggle.isOn)
                this.DoStateTransition(this.m_Selected ? VisualState.Selected : VisualState.Normal, false);
        }

        /// <summary>
        /// Instantly clears the visual state.
        /// </summary>
        protected void InstantClearState()
        {
            this.SetEffectColor(Color.white);
        }

        /// <summary>
        /// Internally evaluates and transitions to normal state.
        /// </summary>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        private void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            this.DoStateTransition(this.m_Active ? VisualState.Active : VisualState.Normal, instant);
        }

        public void OnSelect(BaseEventData eventData)
        {
            this.m_Selected = true;

            if (this.m_Active)
                return;

            this.DoStateTransition(VisualState.Selected, false);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.m_Selected = false;

            if (this.m_Active)
                return;

            this.DoStateTransition((this.m_Highlighted ? VisualState.Highlighted : VisualState.Normal), false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.m_Highlighted = true;

            if (!this.m_Selected && !this.m_Pressed && !this.m_Active)
                this.DoStateTransition(VisualState.Highlighted, false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.m_Highlighted = false;

            if (!this.m_Selected && !this.m_Pressed && !this.m_Active)
                this.DoStateTransition(VisualState.Normal, false);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!this.m_Highlighted)
                return;

            this.m_Pressed = true;
            this.DoStateTransition(VisualState.Pressed, false);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            this.m_Pressed = false;

            VisualState newState = VisualState.Normal;

            if (this.m_Active)
            {
                newState = VisualState.Active;
            }
            else if (this.m_Selected)
            {
                newState = VisualState.Selected;
            }
            else if (this.m_Highlighted)
            {
                newState = VisualState.Highlighted;
            }

            this.DoStateTransition(newState, false);
        }

        /// <summary>
        /// Does the state transition.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        protected virtual void DoStateTransition(VisualState state, bool instant)
        {
            // Check if active in the scene
            if (!this.gameObject.activeInHierarchy)
                return;

            // Check if it's interactable
            if (!this.IsInteractable())
                state = VisualState.Normal;

            Color color = this.m_NormalColor;

            // Prepare the transition values
            switch (state)
            {
                case VisualState.Normal:
                    color = this.m_NormalColor;
                    break;
                case VisualState.Highlighted:
                    color = this.m_HighlightedColor;
                    break;
                case VisualState.Selected:
                    color = this.m_SelectedColor;
                    break;
                case VisualState.Pressed:
                    color = this.m_PressedColor;
                    break;
                case VisualState.Active:
                    color = this.m_ActiveColor;
                    break;
            }
            
            this.StartEffectColorTween(color, false);
        }
        
        private void StartEffectColorTween(Color targetColor, bool instant)
        {
            if (this.m_TargetEffect == null)
                return;

            if ((this.m_TargetEffect is Shadow) == false && (this.m_TargetEffect is Outline) == false)
                return;

            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                this.SetEffectColor(targetColor);
            }
            else
            {
                var colorTween = new ColorTween { duration = this.m_Duration, startColor = this.GetEffectColor(), targetColor = targetColor };
                colorTween.AddOnChangedCallback(SetEffectColor);
                colorTween.ignoreTimeScale = true;

                this.m_ColorTweenRunner.StartTween(colorTween);
            }
        }

        /// <summary>
		/// Sets the effect color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		private void SetEffectColor(Color targetColor)
        {
            if (this.m_TargetEffect == null)
                return;
            
            if (this.m_TargetEffect is Shadow)
            {
                (this.m_TargetEffect as Shadow).effectColor = targetColor;
            }
            else if (this.m_TargetEffect is Outline)
            {
                (this.m_TargetEffect as Outline).effectColor = targetColor;
            }
        }

        private Color GetEffectColor()
        {
            if (this.m_TargetEffect == null)
                return Color.white;

            if (this.m_TargetEffect is Shadow)
            {
                return (this.m_TargetEffect as Shadow).effectColor;
            }
            else if (this.m_TargetEffect is Outline)
            {
                return (this.m_TargetEffect as Outline).effectColor;
            }

            return Color.white;
        }
    }
}
